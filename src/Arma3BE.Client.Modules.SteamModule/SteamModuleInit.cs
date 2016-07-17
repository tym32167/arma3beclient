using Arma3BE.Client.Infrastructure;
using Arma3BE.Client.Infrastructure.Helpers;
using Arma3BE.Client.Modules.SteamModule.Grids;
using Arma3BE.Client.Modules.SteamModule.Models;
using Arma3BEClient.Libs.ModelCompact;
using Microsoft.Practices.Unity;
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
            return ServerTabViewHelper.RegisterView<SteamQuery, ServerInfo, ServerMonitorSteamQueryViewModel>(_container,
                "serverInfo");
        }
    }
}