using Arma3BE.Client.Infrastructure;
using Arma3BE.Client.Infrastructure.Events.BE;
using Arma3BE.Server;
using Arma3BE.Server.Abstract;
using Arma3BEClient.Common.Core;
using Arma3BEClient.Common.Logging;
using Arma3BEClient.Libs.ModelCompact;
using Microsoft.Practices.Unity;
using Prism.Events;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Arma3BE.Client.Modules.BEServerModule
{
    public class BEService : IBEService
    {
        private readonly IUnityContainer _container;
        private readonly ILog _log = LogFactory.Create(new StackTrace().GetFrame(0).GetMethod().DeclaringType);
        private readonly IIpService _ipService;
        private readonly IEventAggregator _eventAggregator;
        private ConcurrentDictionary<Guid, ServerItem> _serverPool = new ConcurrentDictionary<Guid, ServerItem>();

        public BEService(IUnityContainer container, IIpService ipService, IEventAggregator eventAggregator)
        {
            _container = container;
            _ipService = ipService;
            _eventAggregator = eventAggregator;

            _eventAggregator.GetEvent<RunServerEvent>().Subscribe(CheckServer, ThreadOption.BackgroundThread);
            _eventAggregator.GetEvent<CloseServerEvent>().Subscribe(CloseServer, ThreadOption.BackgroundThread);
        }

        public IEnumerable<Guid> ConnectedServers()
        {
            return _serverPool.Keys.ToArray();
        }

        private void CloseServer(ServerInfo info)
        {
            ServerItem item;

            if (_serverPool.TryRemove(info.Id, out item))
            {
                item.Dispose();
            }
        }

        private void CheckServer(ServerInfo info)
        {
            var item = _serverPool.GetOrAdd(info.Id, id => Create(info));
            if (!item.BEServer.Connected) item.BEServer.Connect();
        }

        private ServerItem Create(ServerInfo info)
        {
            return new ServerItem(info, CreateServer(info), _eventAggregator);
        }

        private IBEServer CreateServer(ServerInfo info)
        {
            var host = _ipService.GetIpAddress(info.Host);

            if (string.IsNullOrEmpty(host))
            {
                var message = $"Host {info.Host} is incorrect for server {info.Name}";
                _log.Error(message);
            }

            var server = _container.Resolve<BEServer>(new ParameterOverride("host", host),
                new ParameterOverride("port", info.Port),
                new ParameterOverride("password", info.Password));

            return server;
        }

        private class ServerItem : DisposeObject
        {
            private readonly IEventAggregator _eventAggregator;
            public ServerInfo Info { get; }
            public IBEServer BEServer { get; }

            public ServerItem(ServerInfo info, IBEServer beServer, IEventAggregator eventAggregator)
            {
                _eventAggregator = eventAggregator;
                Info = info;
                BEServer = beServer;

                BEServer.PlayerHandler += BEServer_PlayerHandler;
                BEServer.AdminHandler += BEServer_AdminHandler;
                BEServer.BanHandler += BEServer_BanHandler;
                BEServer.MissionHandler += BEServer_MissionHandler;

                BEServer.BanLog += BEServer_BanLog;
                BEServer.RConAdminLog += BEServer_RConAdminLog;
                BEServer.PlayerLog += BEServer_PlayerLog;

                BEServer.ChatMessageHandler += BEServer_ChatMessageHandler;

                BEServer.ConnectHandler += BEServer_ConnectHandler;
                BEServer.ConnectingHandler += BEServer_ConnectingHandler;
                BEServer.DisconnectHandler += BEServer_DisconnectHandler;

                BEServer.MessageHandler += BEServer_MessageHandler;

                _eventAggregator.GetEvent<BEMessageEvent<BECommand>>().Subscribe(Command);
            }

            private void BEServer_MessageHandler(object sender, EventArgs e)
            {
                _eventAggregator.GetEvent<BEMessageEvent<BEMessage>>()
                  .Publish(new BEMessage(Info.Id));
            }

            private void BEServer_DisconnectHandler(object sender, EventArgs e)
            {
                _eventAggregator.GetEvent<DisConnectServerEvent>()
                   .Publish(Info);
            }

            private void BEServer_ConnectingHandler(object sender, EventArgs e)
            {
                _eventAggregator.GetEvent<ConnectingServerEvent>()
                  .Publish(Info);
            }

            private void BEServer_ConnectHandler(object sender, EventArgs e)
            {
                _eventAggregator.GetEvent<ConnectServerEvent>()
                  .Publish(Info);
            }

            protected override void DisposeManagedResources()
            {
                base.DisposeManagedResources();

                BEServer.PlayerHandler -= BEServer_PlayerHandler;
                BEServer.AdminHandler -= BEServer_AdminHandler;
                BEServer.BanHandler -= BEServer_BanHandler;
                BEServer.MissionHandler -= BEServer_MissionHandler;

                BEServer.BanLog -= BEServer_BanLog;
                BEServer.RConAdminLog -= BEServer_RConAdminLog;
                BEServer.PlayerLog -= BEServer_PlayerLog;

                BEServer.ChatMessageHandler -= BEServer_ChatMessageHandler;

                BEServer.ConnectHandler -= BEServer_ConnectHandler;
                BEServer.ConnectingHandler -= BEServer_ConnectingHandler;
                BEServer.DisconnectHandler -= BEServer_DisconnectHandler;

                BEServer.MessageHandler -= BEServer_MessageHandler;

                _eventAggregator.GetEvent<BEMessageEvent<BECommand>>().Unsubscribe(Command);

                BEServer.Disconnect();
                BEServer.Dispose();
            }

            private void BEServer_ChatMessageHandler(object sender, Server.Models.ChatMessage e)
            {
                _eventAggregator.GetEvent<BEMessageEvent<BEChatMessage>>()
                   .Publish(new BEChatMessage(e, Info.Id));
            }

            private void BEServer_PlayerLog(object sender, Server.Models.LogMessage e)
            {
                _eventAggregator.GetEvent<BEMessageEvent<BEPlayerLogMessage>>()
                    .Publish(new BEPlayerLogMessage(e, Info.Id));
            }

            private void BEServer_RConAdminLog(object sender, Server.Models.LogMessage e)
            {
                _eventAggregator.GetEvent<BEMessageEvent<BEAdminLogMessage>>()
                    .Publish(new BEAdminLogMessage(e, Info.Id));
            }

            private void BEServer_BanLog(object sender, Server.Models.LogMessage e)
            {
                _eventAggregator.GetEvent<BEMessageEvent<BEBanLogMessage>>()
                    .Publish(new BEBanLogMessage(e, Info.Id));
            }

            private void Command(BECommand command)
            {
                var server = this.BEServer;

                if (this.Info.Id == command.ServerId && server != null && server.Connected)
                {
                    server.SendCommand(command.CommandType, command.Parameters);
                }
            }

            private void BEServer_MissionHandler(object sender, BEClientEventArgs<System.Collections.Generic.IEnumerable<Server.Models.Mission>> e)
            {
                _eventAggregator.GetEvent<BEMessageEvent<BEItemsMessage<Server.Models.Mission>>>().Publish(new BEItemsMessage<Server.Models.Mission>(e.Data, Info.Id));
            }

            private void BEServer_BanHandler(object sender, BEClientEventArgs<System.Collections.Generic.IEnumerable<Server.Models.Ban>> e)
            {
                _eventAggregator.GetEvent<BEMessageEvent<BEItemsMessage<Server.Models.Ban>>>().Publish(new BEItemsMessage<Server.Models.Ban>(e.Data, Info.Id));
            }

            private void BEServer_AdminHandler(object sender, BEClientEventArgs<System.Collections.Generic.IEnumerable<Server.Models.Admin>> e)
            {
                _eventAggregator.GetEvent<BEMessageEvent<BEItemsMessage<Server.Models.Admin>>>().Publish(new BEItemsMessage<Server.Models.Admin>(e.Data, Info.Id));
            }

            private void BEServer_PlayerHandler(object sender, Server.BEClientEventArgs<System.Collections.Generic.IEnumerable<Server.Models.Player>> e)
            {
                _eventAggregator.GetEvent<BEMessageEvent<BEItemsMessage<Server.Models.Player>>>().Publish(new BEItemsMessage<Server.Models.Player>(e.Data, Info.Id));
            }
        }
    }
}