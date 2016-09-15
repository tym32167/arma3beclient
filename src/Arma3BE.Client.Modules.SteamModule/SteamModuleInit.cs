using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Arma3BE.Client.Infrastructure;
using Arma3BE.Client.Infrastructure.Commands;
using Arma3BE.Client.Infrastructure.Helpers;
using Arma3BE.Client.Infrastructure.UI;
using Arma3BE.Client.Modules.SteamModule.Grids;
using Arma3BE.Client.Modules.SteamModule.Models;
using Arma3BEClient.Libs.ModelCompact;
using Microsoft.Practices.Unity;
using Prism.Commands;
using Prism.Modularity;
using Prism.Regions;

namespace Arma3BE.Client.Modules.SteamModule
{
    public class SteamModuleInit : IModule
    {
        private static IUnityContainer _container;
        private readonly IRegionManager _regionManager;

        public SteamModuleInit(IUnityContainer container, IRegionManager regionManager)
        {
            _container = container;
            _regionManager = regionManager;
        }

        public void Initialize()
        {
            _regionManager.RegisterViewWithRegion(RegionNames.ServerTabPartRegion, CreateSteamQueryView);
            _regionManager.RegisterViewWithRegion(RegionNames.MenuToolsRegion, CreateSteamDiscoveryView);


            _container.RegisterInstance(_container.Resolve<SteamDiscoveryViewModel>());
        }

        private object CreateSteamQueryView()
        {
            return ServerTabViewHelper.RegisterView<SteamQuery, ServerInfo, ServerMonitorSteamQueryViewModel>(_container,
                "serverInfo");
        }

        private object CreateSteamDiscoveryView()
        {
            var vm = _container.Resolve<SteamDiscoveryViewModel>();


            return new MenuItem()
            {
                Command = new DelegateCommand(() =>
                    {
                        var view = _container.Resolve<SteamDiscovery>();
                        view.DataContext = vm;
                        _regionManager.Regions[RegionNames.ServerTabRegion].Add(view, null, true);
                    },
                    () =>
                        _regionManager.Regions[RegionNames.ServerTabRegion].Views.OfType<SteamDiscovery>().Any() ==
                        false),
                Header = vm.Title
            };
        }
    }
}