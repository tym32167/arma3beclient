using Arma3BE.Client.Infrastructure;
using Arma3BE.Client.Modules.MainModule.ViewModel;
using Microsoft.Practices.Unity;
using Prism.Regions;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Arma3BEClient.Libs.Core;
using Arma3BEClient.Libs.Core.Model;
using Arma3BEClient.Libs.EF.Repositories;
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
        private readonly IServerInfoRepository _infoRepository;

        public MainWindow(MainViewModel model, IUnityContainer container, IRegionManager regionManager,
            IServerInfoRepository infoRepository)
        {
            InitializeComponent();

            _model = model;
            _container = container;
            _regionManager = regionManager;
            _infoRepository = infoRepository;

            DataContext = _model;
        }

        private async Task OpenServerInfoAsync(ServerInfoDto serverInfo)
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

            await _model.ReloadAsync();

            region.Add(control, null, true);
            region.Activate(control);
        }

        private async void ServerClick(object sender, RoutedEventArgs e)
        {
            var orig = e.OriginalSource as FrameworkElement;
            if (orig?.DataContext is ServerInfoDto)
            {
                var serverInfo = (ServerInfoDto)orig.DataContext;
                await OpenServerInfoAsync(serverInfo);
            }
        }

        private void ExitClick(object sender, RoutedEventArgs e)
        {
            this.FindVisualAncestor<Window>().Close();
            Application.Current.Shutdown();
        }

        private async void LoadedWindow(object sender, RoutedEventArgs e)
        {
            var servers = await _infoRepository.GetActiveServerInfoAsync();
            foreach (var server in servers)
            {
                OpenServerInfoAsync(server);
            }
        }

        private void AboutClick(object sender, RoutedEventArgs e)
        {
            var window = new About();
            window.Owner = this.FindVisualAncestor<Window>();
            window.ShowDialog();
        }


        private void UpdateToLatestDevClick(object sender, RoutedEventArgs e)
        {
            var tempDirName = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            if (Directory.Exists(tempDirName) == false) Directory.CreateDirectory(tempDirName);

            var files = new[] { "Arma3BE.Client.Updater.exe", "Newtonsoft.Json.dll" };
            var currentfile = System.Reflection.Assembly.GetEntryAssembly().Location;
            var currentDir = Path.GetDirectoryName(currentfile);

            foreach (var file in files)
            {
                var from = Path.Combine(currentDir, file);
                var to = Path.Combine(tempDirName, file);

                File.Copy(from, to);
            }

            var exec = Path.Combine(tempDirName, "Arma3BE.Client.Updater.exe");
            Process.Start(exec, $"\"{currentDir}\" RESTART");
            Environment.Exit(0);
        }
    }
}