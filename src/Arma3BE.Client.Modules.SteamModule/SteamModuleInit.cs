using Arma3BE.Client.Infrastructure.Models;
using Arma3BE.Client.Modules.SteamModule.Models;
using Microsoft.Practices.Unity;
using Prism.Events;
using Prism.Modularity;

namespace Arma3BE.Client.Modules.SteamModule
{
    public class SteamModuleInit : IModule
    {
        private static IUnityContainer _container;

        public SteamModuleInit(IUnityContainer container)
        {
            _container = container;
        }

        public void Initialize()
        {
            _container.RegisterInstance(new SteamService(_container.Resolve<IEventAggregator>()));
            _container.RegisterType<IServerMonitorSteamQueryViewModel, ServerMonitorSteamQueryViewModel>();
        }
    }
}