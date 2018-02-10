using Arma3BE.Client.Infrastructure;
using Arma3BE.Client.Infrastructure.Helpers;
using Arma3BE.Client.Modules.OnlinePlayersModule.Grids;
using Arma3BE.Client.Modules.OnlinePlayersModule.Models;
using Arma3BEClient.Libs.EF.Repositories;
using Microsoft.Practices.Unity;
using Prism.Modularity;
using Prism.Regions;

namespace Arma3BE.Client.Modules.OnlinePlayersModule
{
    public class OnlinePlayersModuleInit : IModule
    {
        private static IUnityContainer _container;
        private readonly IRegionManager _regionManager;

        public OnlinePlayersModuleInit(IUnityContainer container, IRegionManager regionManager)
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
            return ServerTabViewHelper.RegisterView<OnlinePlayers, ServerInfoDto, ServerMonitorPlayerViewModel>(_container,
                "serverInfo");
        }

        public static IUnityContainer Current => _container;
    }
}