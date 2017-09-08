using Arma3BE.Client.Infrastructure;
using Arma3BE.Client.Modules.PlayersModule.Grids;
using Arma3BE.Client.Modules.PlayersModule.ViewModel;
using Microsoft.Practices.Unity;
using Prism.Commands;
using Prism.Modularity;
using Prism.Regions;
using System.Linq;
using System.Windows.Controls;

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
            _regionManager.RegisterViewWithRegion(RegionNames.MenuToolsRegion, CreateView);
        }

        private object CreateView()
        {
            return new MenuItem()
            {
                Command = new DelegateCommand(() =>
                {
                    var vm = _container.Resolve<PlayerListModelView>();
                    var view = _container.Resolve<PlayersControl>();
                    view.DataContext = vm;
                    _regionManager.Regions[RegionNames.ServerTabRegion].Add(view, null, true);
                },
                    () =>
                        _regionManager.Regions[RegionNames.ServerTabRegion].Views.OfType<PlayersControl>().Any() ==
                        false),
                Header = PlayerListModelView.StaticTitle
            };
        }
    }
}