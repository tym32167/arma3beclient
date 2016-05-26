using Arma3BEClient.Common.Attributes;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
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
                    var item = new MenuItem();
                    item.Header = propertyInfo.Name;


                    var info = propertyInfo;
                    item.Click += (s, e) =>
                    {
                        var si = dgcGridControl.SelectedItem as T;

                        if (si != null)
                        {
                            try
                            {
                                var val = info.GetValue(si);

                                if (val != null)
                                {
                                    Clipboard.Clear();
                                    Clipboard.SetText(val.ToString());
                                }
                            }
                            catch (Exception)
                            {
                            }
                        }
                    };

                    root.Items.Add(item);
                }
            }

            return menu;
        }

        public static IEnumerable<Column> GenerateColumns<T>()
        {
            var list = new List<Column>();
            var type = typeof(T);
            var members = type.GetProperties();
            foreach (var propertyInfo in members)
            {
                var attr = Attribute.GetCustomAttribute(propertyInfo, typeof(ShowInUiAttribute));
                if (attr != null)
                {
                    var c = new Column();
                    c.FieldName = propertyInfo.Name;
                    c.AllowGroup = false;
                    c.Title = propertyInfo.Name;


                    if (propertyInfo.PropertyType == typeof(DateTime) ||
                        propertyInfo.PropertyType == typeof(DateTime?))
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

        public static ContextMenu DgGenerate<T>(this DataGrid dgcGridControl) where T : class
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
                    var item = new MenuItem();
                    item.Header = propertyInfo.Name;


                    var info = propertyInfo;
                    item.Click += (s, e) =>
                    {
                        var si = dgcGridControl.SelectedItem as T;

                        if (si != null)
                        {
                            try
                            {
                                var val = info.GetValue(si);

                                if (val != null)
                                {
                                    Clipboard.Clear();
                                    Clipboard.SetText(val.ToString());
                                }
                            }
                            catch (Exception)
                            {
                            }
                        }
                    };

                    root.Items.Add(item);
                }
            }

            return menu;
        }

        public static IEnumerable<DataGridColumn> DgGenerateColumns<T>()
        {
            var list = new List<DataGridColumn>();
            var type = typeof(T);
            var members = type.GetProperties();
            foreach (var propertyInfo in members)
            {
                var attr = Attribute.GetCustomAttribute(propertyInfo, typeof(ShowInUiAttribute));
                if (attr != null)
                {
                    var c = new DataGridTextColumn();

                    c.Header = propertyInfo.Name;

                    c.Binding = new Binding(propertyInfo.Name);


                    if (propertyInfo.PropertyType == typeof(DateTime) ||
                        propertyInfo.PropertyType == typeof(DateTime?))
                    {
                        c.Binding.StringFormat = @"{0:dd.MM.yy HH:mm:ss}";
                    }

                    list.Add(c);
                }
            }
            return list;
        }
    }
}