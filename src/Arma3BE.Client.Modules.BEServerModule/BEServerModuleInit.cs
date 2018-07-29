﻿using Arma3BE.Client.Modules.BEServerModule.BELogicItems;
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
            _container.RegisterInstance<IBELogic>(_container.Resolve<BELogic>());
            _container.RegisterInstance<IBEService>(_container.Resolve<BEService>());
            _container.RegisterInstance(_container.Resolve<BEServiceLogic>());


            _container.RegisterInstance(_container.Resolve<LobbyKicker>());
        }
    }
}