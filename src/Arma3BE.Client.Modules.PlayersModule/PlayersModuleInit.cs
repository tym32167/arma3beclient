using Arma3BE.Client.Infrastructure;
using Arma3BE.Client.Infrastructure.Models;
using Arma3BE.Client.Modules.PlayersModule.Grids;
using Arma3BE.Client.Modules.PlayersModule.ViewModel;
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
            var view = _container.Resolve<PlayersControl>();
            var ctx = _regionManager.Regions[RegionNames.ServerTabPartRegion].Context;
            var vm = _container.Resolve<PlayerListModelView>(new ParameterOverride("serverInfo", ctx));
            view.DataContext = vm;
            return view;
        }

        public static IUnityContainer Current => _container;
    }
}