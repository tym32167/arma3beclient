using Microsoft.Practices.Unity;
using Prism.Modularity;

namespace Arma3BE.Client.Modules.BEServerModule
{
    public class BEServerModuleInit : IModule
    {
        private readonly IUnityContainer _container;

        public BEServerModuleInit(IUnityContainer container)
        {
            _container = container;
        }

        public void Initialize()
        {
        }
    }
}