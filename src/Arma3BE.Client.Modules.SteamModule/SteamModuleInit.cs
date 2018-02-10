using Arma3BE.Client.Infrastructure;
using Arma3BE.Client.Infrastructure.Helpers;
using Arma3BE.Client.Modules.SteamModule.Grids;
using Arma3BE.Client.Modules.SteamModule.Models;
using Arma3BEClient.Libs.Tools;
using Microsoft.Practices.Unity;
using Prism.Commands;
using Prism.Modularity;
using Prism.Regions;
using System.Linq;
using System.Windows.Controls;
using Arma3BEClient.Libs.EF.Repositories;

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
            _container.RegisterType<ISteamService, Core.SteamService>(new ContainerControlledLifetimeManager());

            _regionManager.RegisterViewWithRegion(RegionNames.ServerTabPartRegion, CreateSteamQueryView);
            _regionManager.RegisterViewWithRegion(RegionNames.MenuToolsRegion, CreateSteamDiscoveryView);
            _regionManager.RegisterViewWithRegion(RegionNames.MenuToolsRegion, CreateSteamServiceView);
        }

        private object CreateSteamQueryView()
        {
            return ServerTabViewHelper.RegisterView<SteamQuery, ServerInfoDto, ServerMonitorSteamQueryViewModel>(_container,
                "serverInfo");
        }

        private object CreateSteamDiscoveryView()
        {
            return new MenuItem
            {
                Command = new DelegateCommand(() =>
                    {
                        var vm = _container.Resolve<SteamDiscoveryViewModel>();
                        var view = _container.Resolve<SteamDiscovery>();
                        view.DataContext = vm;
                        _regionManager.Regions[RegionNames.ServerTabRegion].Add(view, null, true);
                    },
                    () =>
                        _regionManager.Regions[RegionNames.ServerTabRegion].Views.OfType<SteamDiscovery>().Any() ==
                        false),
                Header = SteamDiscoveryViewModel.StaticTitle
            };
        }

        private object CreateSteamServiceView()
        {
            return new MenuItem
            {
                Command = new DelegateCommand(() =>
                    {
                        var vm = _container.Resolve<SteamServiceViewModel>();
                        var view = _container.Resolve<SteamService>();
                        view.DataContext = vm;
                        _regionManager.Regions[RegionNames.ServerTabRegion].Add(view, null, true);
                    },
                    () =>
                        _regionManager.Regions[RegionNames.ServerTabRegion].Views.OfType<SteamService>().Any() ==
                        false),
                Header = SteamServiceViewModel.StaticTitle
            };
        }
    }
}