using Arma3BE.Client.Infrastructure.Helpers;
using Arma3BE.Client.Infrastructure.Models;
using Arma3BE.Client.Modules.BanModule.Helpers;
using Arma3BE.Client.Modules.BanModule.Models;
using Microsoft.Practices.Unity;
using Prism.Events;
using Prism.Modularity;

namespace Arma3BE.Client.Modules.BanModule
{
    public class BanModuleInit : IModule
    {
        private readonly IUnityContainer _container;

        public BanModuleInit(IUnityContainer container)
        {
            _container = container;
        }

        public void Initialize()
        {
            _container.RegisterInstance(new BanService(_container, _container.Resolve<IEventAggregator>()));
            _container.RegisterType<IBanHelper, BanHelper>();
            _container.RegisterType<IServerMonitorBansViewModel, ServerMonitorBansViewModel>();
        }
    }
}