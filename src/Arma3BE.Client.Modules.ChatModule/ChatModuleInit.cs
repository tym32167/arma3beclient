﻿using Arma3BE.Client.Infrastructure;
using Arma3BE.Client.Infrastructure.Helpers;
using Arma3BE.Client.Modules.ChatModule.Chat;
using Arma3BE.Client.Modules.ChatModule.Models;
using Arma3BEClient.Libs.Core.Model;
using Arma3BEClient.Libs.EF.Repositories;
using Microsoft.Practices.Unity;
using Prism.Modularity;
using Prism.Regions;

namespace Arma3BE.Client.Modules.ChatModule
{
    public class ChatModuleInit : IModule
    {
        private readonly IUnityContainer _container;
        private readonly IRegionManager _regionManager;

        public ChatModuleInit(IUnityContainer container, IRegionManager regionManager)
        {
            _container = container;
            _regionManager = regionManager;
        }

        public void Initialize()
        {
            _regionManager.RegisterViewWithRegion(RegionNames.ServerSidePartRegion, CreateView);
        }

        private object CreateView()
        {
            return ServerTabViewHelper.RegisterView<ChatControl, ServerInfoDto, ServerMonitorChatViewModel>(_container,
                "serverInfo");
        }
    }
}