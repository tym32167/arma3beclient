using Arma3BE.Client.Infrastructure;
using Microsoft.Practices.Unity;
using Prism.Commands;
using Prism.Modularity;
using Prism.Regions;
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

        public void Initialize()
        {
            _regionManager.RegisterViewWithRegion(RegionNames.MenuFileToolsRegion, CreateExportViewItem);
            _regionManager.RegisterViewWithRegion(RegionNames.MenuFileToolsRegion, CreateImportViewItem);
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