using Arma3BE.Client.Infrastructure;
using Arma3BE.Client.Infrastructure.Helpers;
using Arma3BE.Client.Infrastructure.Models;
using Arma3BE.Client.Modules.BanModule.Helpers;
using Arma3BE.Client.Modules.BanModule.Models;
using Microsoft.Practices.Unity;
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
            _container.RegisterType<IBanService, BanService>();
            _container.RegisterType<IBanHelper, BanHelper>();
            _container.RegisterType<IServerMonitorBansViewModel, ServerMonitorBansViewModel>();
        }
    }
}