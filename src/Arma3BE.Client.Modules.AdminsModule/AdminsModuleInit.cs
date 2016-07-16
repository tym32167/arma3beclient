using Arma3BE.Client.Infrastructure.Models;
using Arma3BE.Client.Modules.AdminsModule.Models;
using Microsoft.Practices.Unity;
using Prism.Events;
using Prism.Modularity;

namespace Arma3BE.Client.Modules.AdminsModule
{
    public class AdminsModuleInit : IModule
    {
        private readonly IUnityContainer _container;

        public AdminsModuleInit(IUnityContainer container)
        {
            _container = container;
        }

        public void Initialize()
        {
            
        }
    }
}