using Arma3BE.Client.Infrastructure;
using Arma3BE.Client.Modules.MainModule.ViewModel;
using Arma3BEClient.Libs.ModelCompact;
using Arma3BEClient.Libs.Repositories;
using Microsoft.Practices.Unity;
using Prism.Regions;
using System.Linq;
using System.Windows;
using Xceed.Wpf.AvalonDock.Controls;

namespace Arma3BE.Client.Modules.MainModule
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    // ReSharper disable once RedundantExtendsListEntry
    public partial class MainWindow
    {
        private readonly IUnityContainer _container;
        private readonly MainViewModel _model;
        private readonly IRegionManager _regionManager;

        public MainWindow(MainViewModel model, IUnityContainer container, IRegionManager regionManager)
        {
            InitializeComponent();

            _model = model;
            _container = container;
            _regionManager = regionManager;

            DataContext = _model;
        }

        private void OpenServerInfo(ServerInfo serverInfo)
        {
            var region = _regionManager.Regions[RegionNames.ServerTabRegion];

            if (region.Views.OfType<ServerInfoControl>()
                .Select(x => x.DataContext as ServerMonitorModel)
                .Any(x => x?.CurrentServer.Id == serverInfo.Id))
                return;


            var control =
                _container.Resolve<ServerInfoControl>(
                    new ParameterOverride("currentServer", serverInfo).OnType<ServerMonitorModel>(),
                    new ParameterOverride("console", false).OnType<ServerMonitorModel>());

            _model.Reload();

            region.Add(control, null, true);
            region.Activate(control);
        }

        private void ServerClick(object sender, RoutedEventArgs e)
        {
            var orig = e.OriginalSource as FrameworkElement;
            if (orig?.DataContext is ServerInfo)
            {
                var serverInfo = (ServerInfo)orig.DataContext;
                OpenServerInfo(serverInfo);
            }
        }

        private void ExitClick(object sender, RoutedEventArgs e)
        {
            this.FindVisualAncestor<Window>().Close();
            Application.Current.Shutdown();
        }

        private void LoadedWindow(object sender, RoutedEventArgs e)
        {
            using (var r = new ServerInfoRepository())
            {
                var servers = r.GetActiveServerInfo();
                foreach (var server in servers)
                {
                    OpenServerInfo(server);
                }
            }
        }

        private void AboutClick(object sender, RoutedEventArgs e)
        {
            var window = new About();
            window.Owner = this.FindVisualAncestor<Window>();
            window.ShowDialog();
        }
    }
}