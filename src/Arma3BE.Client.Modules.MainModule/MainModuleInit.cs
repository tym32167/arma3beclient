using Arma3BE.Client.Infrastructure;
using Arma3BE.Client.Modules.MainModule.ViewModel;
using Arma3BE.Server.Abstract;
using Arma3BE.Server.ServerFactory;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using Prism.Modularity;
using Prism.Regions;
using System.Configuration;
using Xceed.Wpf.AvalonDock;

namespace Arma3BE.Client.Modules.MainModule
{
    public class MainModuleInit : IModule
    {
        private readonly IRegionManager _regionManager;
        private const string DebugServerKey = "DebugServerEnabled";

        public MainModuleInit(IUnityContainer container, IRegionManager regionManager)
        {
            var container1 = container;
            _regionManager = regionManager;

            container1.RegisterType<MainViewModel>();
            container1.RegisterType<MainWindow>();

            if (ConfigurationManager.AppSettings[DebugServerKey] != bool.TrueString)
                container1.RegisterType<IBattlEyeServerFactory, WatcherBEServerFactory>();
        }

        public void Initialize()
        {
            _regionManager.RegisterViewWithRegion(RegionNames.MainRegionRegion, typeof(MainWindow));
        }

        public static void CreateRegionAdapterMappings(RegionAdapterMappings mappings)
        {
            mappings.RegisterMapping(typeof(DockingManager),
               ServiceLocator.Current.GetInstance<AvalonDockRegionAdapter>());
        }
    }

}