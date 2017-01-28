using Arma3BEClient.Common.Attributes;
using Arma3BEClient.Common.Logging;
using Arma3BEClient.Libs.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Xml.Serialization;
using Xceed.Wpf.DataGrid;

namespace Arma3BE.Client.Infrastructure.Extensions
{
    public static class GridHelper
    {
        public static ContextMenu Generate<T>(this DataGridControl dgcGridControl) where T : class
        {
            var menu = new ContextMenu();
            var root = new MenuItem { Header = "Copy" };
            menu.Items.Add(root);

            var type = typeof(T);
            var members = type.GetProperties();
            foreach (var propertyInfo in members)
            {
                var attr = Attribute.GetCustomAttribute(propertyInfo, typeof(EnableCopyAttribute));
                if (attr != null)
                {
                    var item = new MenuItem { Header = propertyInfo.Name };

                    var info = propertyInfo;
                    item.Click += (s, e) =>
                    {
                        var si = dgcGridControl.SelectedItem as T;

                        if (si != null)
                            try
                            {
                                var val = info.GetValue(si);

                                if (val != null)
                                {
                                    Clipboard.Clear();
                                    Clipboard.SetText(val.ToString());
                                }
                            }
                            catch (Exception ex)
                            {
                                var log = LogFactory.Create(typeof(GridHelper));
                                log.Error(ex);
                            }
                    };

                    root.Items.Add(item);
                }
            }

            return menu;
        }

        public static void LoadState<T>(this DataGridControl source, string key)
        {
            foreach (var generateColumn in GenerateColumns<T>())
                source.Columns.Add(generateColumn);

            source.Load(key);


            source.Unloaded += (s, e) =>
            {
                var columns = source.Columns.OfType<CustomColumn>().ToArray();

                if (columns.Any(c => c.HasChanges))
                {
                    source.Save(key);

                    foreach (var customColumn in columns)
                    {
                        customColumn.Reset();
                    }
                }
            };
        }

        private static IEnumerable<Column> GenerateColumns<T>()
        {
            var list = new List<Column>();
            var type = typeof(T);
            var members = type.GetProperties();
            foreach (var propertyInfo in members)
            {
                var attr = Attribute.GetCustomAttribute(propertyInfo, typeof(ShowInUiAttribute));
                if (attr != null)
                {
                    var c = new CustomColumn();
                    c.FieldName = propertyInfo.Name;
                    c.AllowGroup = false;
                    c.Title = propertyInfo.Name;


                    if ((propertyInfo.PropertyType == typeof(DateTime)) ||
                        (propertyInfo.PropertyType == typeof(DateTime?)))
                    {
                        var newTextBlock = new FrameworkElementFactory(typeof(TextBlock));
                        newTextBlock.SetBinding(TextBlock.TextProperty,
                            new Binding(".") { StringFormat = @"{0:dd.MM.yy HH:mm:ss}" });
                        var newDataTemplate = new DataTemplate { VisualTree = newTextBlock };

                        c.CellContentTemplate = newDataTemplate;
                    }

                    list.Add(c);
                }
            }
            return list;
        }


        private static void Load(this DataGridControl dgcGridControl, string key)
        {
            var cols = GetColumns(key)?.ToArray();
            if (cols == null) return;
            var colums = dgcGridControl.Columns.ToDictionary(x => x.FieldName);

            foreach (var source in cols.OrderBy(x => x.Order))
                if (colums.ContainsKey(source.OriginalName))
                {
                    var column = colums[source.OriginalName];

                    column.VisiblePosition = source.Order;
                    column.Width = source.Width;
                    column.Visible = source.Visible;

                    (column as CustomColumn)?.Reset();
                }
        }

        private static IEnumerable<ColumnInfo> GetColumns(string key)
        {
            try
            {
                var store = new SettingsStoreSource().GetCustomSettingsStore();
                var data = store.Load(key);
                if (string.IsNullOrEmpty(data)) return null;
                var ser = new XmlSerializer(typeof(ColumnInfo[]));
                using (var sr = new StringReader(data))
                {
                    return ser.Deserialize(sr) as ColumnInfo[];
                }
            }
            catch (Exception e)
            {
                var log = LogFactory.Create(typeof(GridHelper));
                log.Error(e);
                return null;
            }
        }


        private static void Save(this DataGridControl dgcGridControl, string key)
        {
            var cols =
                dgcGridControl.Columns.Select(
                    (c, i) =>
                        new ColumnInfo
                        {
                            Width = c.ActualWidth,
                            Order = c.VisiblePosition,
                            OriginalName = c.FieldName,
                            Visible = c.Visible
                        }).ToArray();
            SaveColumns(key, cols);
        }

        private static void SaveColumns(string key, IEnumerable<ColumnInfo> infos)
        {
            try
            {
                var array = infos.ToArray();
                var store = new SettingsStoreSource().GetCustomSettingsStore();
                var ser = new XmlSerializer(typeof(ColumnInfo[]));

                var sb = new StringBuilder();

                using (var sw = new StringWriter(sb))
                {
                    ser.Serialize(sw, array);
                }

                store.Save(key, sb.ToString());
            }
            catch (Exception e)
            {
                var log = LogFactory.Create(typeof(GridHelper));
                log.Error(e);
            }
        }

        private class CustomColumn : Column
        {
            public CustomColumn()
            {
                HasChanges = false;
                Changed += (s, e) => { HasChanges = true; };
            }

            public void Reset()
            {
                HasChanges = false;
            }

            public bool HasChanges { get; private set; }
        }
    }

    public class ColumnInfo
    {
        public string OriginalName { get; set; }
        public int Order { get; set; }
        public double Width { get; set; }
        public bool Visible { get; set; }
    }
}