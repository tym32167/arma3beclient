using Arma3BE.Client.Infrastructure;
using Arma3BE.Client.Infrastructure.Helpers;
using Arma3BE.Client.Modules.ManageServerModule.Grids;
using Arma3BE.Client.Modules.ManageServerModule.Models;
using Arma3BEClient.Libs.EF.Repositories;
using Microsoft.Practices.Unity;
using Prism.Modularity;
using Prism.Regions;

namespace Arma3BE.Client.Modules.ManageServerModule
{
    public class ManageServerModuleInit : IModule
    {
        private static IUnityContainer _container;
        private readonly IRegionManager _regionManager;

        public ManageServerModuleInit(IUnityContainer container, IRegionManager regionManager)
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
            return ServerTabViewHelper.RegisterView<ManageServer, ServerInfoDto, ServerMonitorManageServerViewModel>(_container,
                "serverInfo");
        }
    }
}