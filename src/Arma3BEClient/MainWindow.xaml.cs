using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Serialization;
using Arma3BEClient.Grids;
using Arma3BEClient.Lib.Context;
using Arma3BEClient.Lib.ModelCompact;
using Arma3BEClient.Models.Export;
using Arma3BEClient.ViewModel;
using Microsoft.Win32;
using Xceed.Wpf.AvalonDock.Layout;
using Xceed.Wpf.Toolkit;
using MessageBox = Xceed.Wpf.Toolkit.MessageBox;

namespace Arma3BEClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainViewModel _model;

        public MainWindow()
        {
            InitializeComponent();

            _model = new MainViewModel();
            this.DataContext = _model;
        }

        private void NewClick(object sender, RoutedEventArgs e)
        {

        }

        private void OptionsClick(object sender, RoutedEventArgs e)
        {
            var w = new Options(new OptionsModel());
            w.Owner = this;
            w.ShowDialog();

            _model.Reload();
        }

        private void OpenServerInfo(ServerInfo serverInfo)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {

                var doc = new LayoutDocument();

                doc.Title = serverInfo.Name;

                var control = new ServerInfoControl(serverInfo);

                doc.Content = control;

                var firstDocumentPane = DockManager.Layout.Descendents().OfType<LayoutDocumentPane>().FirstOrDefault();
                if (firstDocumentPane != null)
                {
                    firstDocumentPane.Children.Add(doc);
                }

                doc.Closed += (s, a) =>
                {
                    control.Cleanup();
                    _model.SetActive(serverInfo.Id, false);
                    _model.Reload();
                };


                _model.SetActive(serverInfo.Id, true);

                _model.Reload();

                doc.IsActive = true;

            }));
        }

        private void ServerClick(object sender, RoutedEventArgs e)
        {
            var orig = e.OriginalSource as FrameworkElement;
            if (orig != null && orig.DataContext is ServerInfo)
            {
                var serverInfo = orig.DataContext as ServerInfo;

                OpenServerInfo(serverInfo);
            }
        }

        private void ExitClick(object sender, RoutedEventArgs e)
        {
            this.Close();
            Application.Current.Shutdown();
        }


        private void LoadedWindow(object sender, RoutedEventArgs e)
        {
            using (var dc = new Arma3BeClientContext())
            {
                var servers = dc.ServerInfo.Where(x => x.Active).ToList();

                Parallel.ForEach(servers, OpenServerInfo);

                //servers.ForEach(OpenServerInfo);

            }
        }


        private async void ExportClick(object sender, RoutedEventArgs e)
        {
            var dlg = new SaveFileDialog();
            dlg.DefaultExt = "*.xml";
            dlg.Filter = "*.xml|*.xml";
            dlg.Title = "Select file to save players";

            var res = dlg.ShowDialog();

            if (res.HasValue && res.Value)
            {
                await Task.Run(() => Export(dlg.FileName));
                MessageBox.Show("Export finished!");
            }
        }



        void Export(string fname)
        {
            using (var dc = new Arma3BeClientContext())
            {
                var list = dc.Player.ToList().GroupBy(x => x.GUID).Select(x => x.OrderByDescending(y => y.Name).First()).OrderBy(x => x.Name).Select(x =>
                    new PlayerXML
                    {
                        Guid = x.GUID,
                        LastIP = x.LastIp,
                        LastSeen = x.LastSeen,
                        Name = x.Name,
                        Comment = x.Comment
                    }).ToList();


                using (var sw = new StreamWriter(fname))
                {
                    var ser = new XmlSerializer(typeof(List<PlayerXML>));
                    ser.Serialize(sw, list);
                }
            }
        }





        void Import(string fname)
        {
            using (var dc = new Arma3BeClientContext())
            {

                var db = dc.Player.ToList().GroupBy(x=>x.GUID).Select(x=>x.OrderByDescending(y=>y.Name).First()).ToDictionary(x => x.GUID);
                var players = new List<PlayerXML>();

                using (var sr = new StreamReader(fname))
                {
                    var ser = new XmlSerializer(typeof (List<PlayerXML>));
                    players = (List<PlayerXML>) ser.Deserialize(sr);
                }

                var toadd = new List<Player>();
                
                foreach (var p in players)
                {
                    if (!db.ContainsKey(p.Guid))
                    {
                        toadd.Add(new Player
                        {
                            Comment = p.Comment,
                            GUID = p.Guid,
                            LastIp = p.LastIP,
                            LastSeen = p.LastSeen,
                            Name = p.Name,
                            Id = Guid.NewGuid()
                        });
                        
                    }
                    else
                    {
                        var lp = db[p.Guid];
                        if (string.IsNullOrEmpty(lp.Comment) && !string.IsNullOrEmpty(p.Comment))
                        {
                            lp.Comment = p.Comment;
                            
                        }
                    }
                }

                dc.Player.AddRange(toadd);

                dc.SaveChanges();
            }
        }




        private async void ImportClick(object sender, RoutedEventArgs e)
        {
            var ofd = new OpenFileDialog();

            ofd.DefaultExt = "*.xml";
            ofd.Filter = "*.xml|*.xml";
            ofd.Title = "Select file to import players";

            var res = ofd.ShowDialog();

            if (res.HasValue && res.Value)
            {
                await Task.Run(()=>Import(ofd.FileName));
                MessageBox.Show("Import finished!");
            }
        }
    }
}
