using Arma3BE.Client.Infrastructure;
using Arma3BE.Client.Modules.MainModule.ViewModel;
using Arma3BE.Server.Abstract;
using Arma3BE.Server.ServerFactory;
using Microsoft.Practices.Unity;
using Prism.Modularity;
using Prism.Regions;

namespace Arma3BE.Client.Modules.MainModule
{
    public class MainModuleInit : IModule
    {
        private readonly IRegionManager _regionManager;

        public MainModuleInit(IUnityContainer container, IRegionManager regionManager)
        {
            var container1 = container;
            _regionManager = regionManager;

            container1.RegisterType<MainViewModel>();
            container1.RegisterType<MainWindow>();

            container1.RegisterType<IBattlEyeServerFactory, WatcherBEServerFactory>();
        }

        public void Initialize()
        {
            _regionManager.RegisterViewWithRegion(RegionNames.MainRegionRegion, typeof(MainWindow));
        }
    }

}