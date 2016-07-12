using Arma3BE.Client.Infrastructure.Models;
using Arma3BE.Client.Modules.ChatModule.Models;
using Microsoft.Practices.Unity;
using Prism.Events;
using Prism.Modularity;

namespace Arma3BE.Client.Modules.ChatModule
{
    public class ChatModuleInit : IModule
    {
        private readonly IUnityContainer _container;

        public ChatModuleInit(IUnityContainer container)
        {
            _container = container;
        }

        public void Initialize()
        {
            _container.RegisterInstance(new ChatService(_container.Resolve<IEventAggregator>()));
            _container.RegisterType<IServerMonitorChatViewModel, ServerMonitorChatViewModel>();
        }
    }
}