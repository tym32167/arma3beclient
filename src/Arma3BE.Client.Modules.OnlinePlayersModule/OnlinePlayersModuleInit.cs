using Arma3BE.Client.Infrastructure.Models;
using Arma3BE.Client.Modules.OnlinePlayersModule.Models;
using Microsoft.Practices.Unity;
using Prism.Events;
using Prism.Modularity;

namespace Arma3BE.Client.Modules.OnlinePlayersModule
{
    public class OnlinePlayersModuleInit : IModule
    {
        private static IUnityContainer _container;

        public OnlinePlayersModuleInit(IUnityContainer container)
        {
            _container = container;
        }

        public void Initialize()
        {
            _container.RegisterInstance(new OnlinePlayerService(_container.Resolve<IEventAggregator>()));
            _container.RegisterType<IServerMonitorPlayerViewModel, ServerMonitorPlayerViewModel>();
        }

        public static IUnityContainer Current => _container;
    }
}