using Arma3BE.Client.Infrastructure;
using Arma3BE.Client.Modules.SyncModule.Grids;
using Arma3BE.Client.Modules.SyncModule.ViewModels;
using Microsoft.Practices.Unity;
using Prism.Commands;
using Prism.Modularity;
using Prism.Regions;
using System.Linq;
using System.Windows.Controls;

namespace Arma3BE.Client.Modules.SyncModule
{
    public class SyncModuleInit : IModule
    {
        private readonly IUnityContainer _container;
        private readonly IRegionManager _regionManager;

        public SyncModuleInit(IUnityContainer container, IRegionManager regionManager)
        {
            _container = container;
            _regionManager = regionManager;
        }

        public void Initialize()
        {
            _regionManager.RegisterViewWithRegion(RegionNames.MenuToolsRegion, CreateSyncView);
        }

        private object CreateSyncView()
        {
            return new MenuItem
            {
                Command = new DelegateCommand(() =>
                    {
                        var vm = _container.Resolve<SyncModuleViewModel>();
                        var view = _container.Resolve<SyncModuleView>();
                        view.DataContext = vm;
                        _regionManager.Regions[RegionNames.ServerTabRegion].Add(view, null, true);
                    },
                    () =>
                        _regionManager.Regions[RegionNames.ServerTabRegion].Views.OfType<SyncModuleView>().Any() ==
                        false),
                Header = SyncModuleViewModel.StaticTitle
            };
        }
    }
}