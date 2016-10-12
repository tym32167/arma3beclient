using Arma3BE.Client.Infrastructure;
using Arma3BE.Client.Modules.ToolsModule.ViewModels;
using Arma3BE.Client.Modules.ToolsModule.Windows;
using Microsoft.Practices.Unity;
using Prism.Commands;
using Prism.Modularity;
using Prism.Regions;
using System.Windows;
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
                    var wnd = _container.Resolve<ExportWindow>();
                    var vm = _container.Resolve<ExportViewModel>();

                    wnd.Owner = Application.Current.MainWindow;
                    wnd.DataContext = vm;

                    wnd.ShowDialog();
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