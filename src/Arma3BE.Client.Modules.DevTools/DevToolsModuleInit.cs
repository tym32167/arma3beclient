using System.Diagnostics;
using System.Windows.Controls;
using Arma3BE.Client.Infrastructure;
using Arma3BE.Client.Modules.DevTools.Models;
using Arma3BE.Client.Modules.DevTools.Windows;
using Microsoft.Practices.Unity;
using Prism.Commands;
using Prism.Modularity;
using Prism.Regions;

namespace Arma3BE.Client.Modules.DevTools
{
    public class DevToolsModuleInit : IModule
    {
        private static IUnityContainer _container;
        private readonly IRegionManager _regionManager;

        public DevToolsModuleInit(IUnityContainer container, IRegionManager regionManager)
        {
            _container = container;
            _regionManager = regionManager;
        }

        public void Initialize()
        {
            InitDebug();
        }

        [Conditional("DEBUG")]
        private void InitDebug()
        {
            _regionManager.RegisterViewWithRegion(RegionNames.MenuToolsRegion, CreateSteamDiscoveryView);
        }

        private object CreateSteamDiscoveryView()
        {
            return new MenuItem()
            {
                Header = "DevTools",
                ItemsSource = new[]
                {
                    new MenuItem()
                    {
                        Header = "Log viewer",
                        Command = new DelegateCommand(() =>
                        {
                            var vm = _container.Resolve<LogViewerViewModel>();
                            var w = _container.Resolve<LogViewer>();
                            w.DataContext = vm;
                            w.Show();
                        })
                    },
                }
            };
        }
    }
}