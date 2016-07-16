using System.Windows.Controls;
using Arma3BE.Client.Infrastructure;
using Arma3BE.Client.Infrastructure.Models;
using Arma3BE.Client.Modules.OnlinePlayersModule.Grids;
using Arma3BE.Client.Modules.OnlinePlayersModule.Models;
using Microsoft.Practices.Unity;
using Prism.Events;
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
            _regionManager.RegisterViewWithRegion(RegionNames.ServerTabPartRegion, CreatePlayersView);
        }

        private object CreatePlayersView()
        {
            var view = _container.Resolve<OnlinePlayers>();
            var ctx = _regionManager.Regions[RegionNames.ServerTabPartRegion].Context;
            var vm = _container.Resolve<ServerMonitorPlayerViewModel>(new ParameterOverride("serverInfo", ctx));
            view.DataContext = vm;
            return view;
        }

        public static IUnityContainer Current => _container;
    }
}