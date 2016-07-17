using Arma3BE.Client.Infrastructure;
using Arma3BE.Client.Infrastructure.Helpers;
using Arma3BE.Client.Modules.PlayersModule.Grids;
using Arma3BE.Client.Modules.PlayersModule.ViewModel;
using Arma3BEClient.Libs.ModelCompact;
using Microsoft.Practices.Unity;
using Prism.Modularity;
using Prism.Regions;

namespace Arma3BE.Client.Modules.PlayersModule
{
    public class PlayersModuleInit : IModule
    {
        private static IUnityContainer _container;
        private readonly IRegionManager _regionManager;

        public PlayersModuleInit(IUnityContainer container, IRegionManager regionManager)
        {
            _container = container;
            _regionManager = regionManager;
        }

        public void Initialize()
        {
            _container.RegisterInstance(new PlayerService(_container));
            _regionManager.RegisterViewWithRegion(RegionNames.ServerTabPartRegion, CreateView);
        }

        private object CreateView()
        {
            return ServerTabViewHelper.RegisterView<PlayersControl, ServerInfo, PlayerListModelView>(_container,
                "serverInfo");
        }

        public static IUnityContainer Current => _container;
    }
}