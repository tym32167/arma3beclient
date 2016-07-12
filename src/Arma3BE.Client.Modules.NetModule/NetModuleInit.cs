using Arma3BE.Client.Infrastructure;
using Microsoft.Practices.Unity;
using Prism.Modularity;

namespace Arma3BE.Client.Modules.NetModule
{
    public class NetModuleInit : IModule
    {
        private readonly IUnityContainer _container;

        public NetModuleInit(IUnityContainer container)
        {
            _container = container;
        }

        public void Initialize()
        {
            _container.RegisterType<IIpService, IpService>(new ContainerControlledLifetimeManager());
        }
    }
}