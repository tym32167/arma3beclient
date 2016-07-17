using Arma3BE.Client.Infrastructure;
using Arma3BE.Client.Modules.ChatModule.Chat;
using Arma3BE.Client.Modules.ChatModule.Models;
using Microsoft.Practices.Unity;
using Prism.Modularity;
using Prism.Regions;

namespace Arma3BE.Client.Modules.ChatModule
{
    public class ChatModuleInit : IModule
    {
        private readonly IUnityContainer _container;
        private readonly IRegionManager _regionManager;

        public ChatModuleInit(IUnityContainer container, IRegionManager regionManager)
        {
            _container = container;
            _regionManager = regionManager;
        }

        public void Initialize()
        {
            _regionManager.RegisterViewWithRegion(RegionNames.ServerSidePartRegion, CreateView);
        }

        private object CreateView()
        {
            var view = _container.Resolve<ChatControl>();
            var ctx = _regionManager.Regions[RegionNames.ServerSidePartRegion].Context;
            var vm = _container.Resolve<ServerMonitorChatViewModel>(new ParameterOverride("serverInfo", ctx));
            view.DataContext = vm;
            return view;
        }
    }
}