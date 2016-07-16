using Arma3BE.Client.Infrastructure;
using Arma3BE.Client.Infrastructure.Helpers;
using Arma3BE.Client.Infrastructure.Models;
using Arma3BE.Client.Modules.BanModule.Grids;
using Arma3BE.Client.Modules.BanModule.Helpers;
using Arma3BE.Client.Modules.BanModule.Models;
using Microsoft.Practices.Unity;
using Prism.Events;
using Prism.Modularity;
using Prism.Regions;

namespace Arma3BE.Client.Modules.BanModule
{
    public class BanModuleInit : IModule
    {
        private readonly IUnityContainer _container;
        private readonly IRegionManager _regionManager;

        public BanModuleInit(IUnityContainer container, IRegionManager regionManager)
        {
            _container = container;
            _regionManager = regionManager;
        }

        public void Initialize()
        {
            _container.RegisterInstance(new BanService(_container, _container.Resolve<IEventAggregator>()));
            _container.RegisterType<IBanHelper, BanHelper>();
        
            _regionManager.RegisterViewWithRegion(RegionNames.ServerTabPartRegion, CreateView);
        }

        private object CreateView()
        {
            var view = _container.Resolve<BansControl>();
            var ctx = _regionManager.Regions[RegionNames.ServerTabPartRegion].Context;
            var vm = _container.Resolve<ServerMonitorBansViewModel>(new ParameterOverride("serverInfo", ctx));
            view.DataContext = vm;
            return view;
        }
    }
}