using Arma3BE.Client.Infrastructure.Helpers;
using Arma3BE.Client.Modules.CoreModule.Helpers;
using Arma3BE.Client.Modules.CoreModule.Services;
using Microsoft.Practices.Unity;
using Prism.Modularity;

namespace Arma3BE.Client.Modules.CoreModule
{
    public class CoreModuleInit : IModule
    {
        private readonly IUnityContainer _container;

        public CoreModuleInit(IUnityContainer container)
        {
            _container = container;
        }

        public void Initialize()
        {
            _container.RegisterType<IBanHelper, BanHelper>();
            _container.RegisterInstance(_container.Resolve<BECommandManagerService>());
        }
    }
}