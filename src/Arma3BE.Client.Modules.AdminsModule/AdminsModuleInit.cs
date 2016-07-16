using Arma3BE.Client.Infrastructure;
using Arma3BE.Client.Infrastructure.Models;
using Arma3BE.Client.Modules.AdminsModule.Grids;
using Arma3BE.Client.Modules.AdminsModule.Models;
using Microsoft.Practices.Unity;
using Prism.Events;
using Prism.Modularity;
using Prism.Regions;

namespace Arma3BE.Client.Modules.AdminsModule
{
    public class AdminsModuleInit : IModule
    {
        private readonly IUnityContainer _container;
        private readonly IRegionManager _regionManager;

        public AdminsModuleInit(IUnityContainer container, IRegionManager regionManager)
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
            var view = _container.Resolve<AdminsControl>();
            var ctx = _regionManager.Regions[RegionNames.ServerTabPartRegion].Context;
            var vm = _container.Resolve<ServerMonitorAdminsViewModel>(new ParameterOverride("serverInfo", ctx));
            view.DataContext = vm;
            return view;
        }
    }
}