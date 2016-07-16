using Arma3BE.Client.Infrastructure.Models;
using Arma3BE.Client.Modules.ManageServerModule.Models;
using Microsoft.Practices.Unity;
using Prism.Events;
using Prism.Modularity;

namespace Arma3BE.Client.Modules.ManageServerModule
{
    public class ManageServerModuleInit : IModule
    {
        private static IUnityContainer _container;

        public ManageServerModuleInit(IUnityContainer container)
        {
            _container = container;
        }

        public void Initialize()
        {
        }
    }
}