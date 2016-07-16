using Arma3BE.Client.Infrastructure;
using Arma3BE.Client.Infrastructure.Models;
using Arma3BE.Client.Modules.SteamModule.Grids;
using Arma3BE.Client.Modules.SteamModule.Models;
using Microsoft.Practices.Unity;
using Prism.Events;
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
            _regionManager.RegisterViewWithRegion(RegionNames.ServerTabPartRegion, CreateView);
        }

        private object CreateView()
        {
            var view = _container.Resolve<SteamQuery>();
            var ctx = _regionManager.Regions[RegionNames.ServerTabPartRegion].Context;
            var vm = _container.Resolve<ServerMonitorSteamQueryViewModel>(new ParameterOverride("serverInfo", ctx));
            view.DataContext = vm;
            return view;
        }
    }
}