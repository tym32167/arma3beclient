using Arma3BE.Client.Infrastructure;
using Arma3BE.Client.Infrastructure.Helpers;
using Arma3BEClient.Libs.ModelCompact;
using Arma3BEClient.Libs.Repositories;
using Microsoft.Practices.Unity;
using Prism.Modularity;
using Prism.Regions;

namespace Arma3BE.Client.Modules.IndicatorsModule
{
    public class IndicatorsModuleInit : IModule
    {
        private readonly IUnityContainer _container;
        private readonly IRegionManager _regionManager;

        public IndicatorsModuleInit(IUnityContainer container, IRegionManager regionManager)
        {
            _container = container;
            _regionManager = regionManager;
        }

        public void Initialize()
        {
            _regionManager.RegisterViewWithRegion(RegionNames.BottomUpdateIndicatorRegion, CreateView);
        }

        private object CreateView()
        {
            return ServerTabViewHelper.RegisterView<LastUpdateIndicator, ServerInfoDto, LastUpdateIndicatorViewModel>(_container,
                "serverInfo");
        }
    }
}