using Arma3BE.Client.Infrastructure;
using Arma3BE.Client.Modules.ToolsModule.Virtual;
using Arma3BE.Server.Abstract;
using Microsoft.Practices.Unity;
using Prism.Commands;
using Prism.Modularity;
using Prism.Regions;
using System.Configuration;
using System.Windows.Controls;

namespace Arma3BE.Client.Modules.ToolsModule
{
    public class ToolsModuleInit : IModule
    {
        private static IUnityContainer _container;
        private readonly IRegionManager _regionManager;

        public ToolsModuleInit(IUnityContainer container, IRegionManager regionManager)
        {
            _container = container;
            _regionManager = regionManager;
        }

        private const string DebugServerKey = "DebugServerEnabled";

        public void Initialize()
        {
            if (ConfigurationManager.AppSettings[DebugServerKey] == bool.TrueString)
            {
                _container.RegisterType<IBattlEyeServerFactory, VirtualServerFactory>(new ContainerControlledLifetimeManager());
                _regionManager.RegisterViewWithRegion(RegionNames.MenuFileToolsRegion, CreateDebugWindow);

            }


            _regionManager.RegisterViewWithRegion(RegionNames.MenuFileToolsRegion, CreateExportViewItem);
            _regionManager.RegisterViewWithRegion(RegionNames.MenuFileToolsRegion, CreateImportViewItem);
        }

        private object CreateDebugWindow()
        {
            return new MenuItem
            {
                Header = "Debug server",
                Command = new DelegateCommand(() =>
                {
                    _container.Resolve<VirtualServerWindow>().Show();
                })
            };
        }

        private object CreateExportViewItem()
        {
            return new MenuItem
            {
                Header = "Export players",
                Command = new DelegateCommand(() =>
                {
                    _container.Resolve<Exporter>().Export();
                })
            };
        }

        private object CreateImportViewItem()
        {
            return new MenuItem
            {
                Header = "Import players",
                Command = new DelegateCommand(() =>
                {
                    _container.Resolve<Importer>().Import();
                })
            };
        }
    }
}