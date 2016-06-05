using Arma3BE.Client.Infrastructure;
using Arma3BE.Client.Infrastructure.Events.BE;
using Arma3BE.Server;
using Arma3BE.Server.Abstract;
using Arma3BEClient.Common.Logging;
using Arma3BEClient.Libs.ModelCompact;
using Microsoft.Practices.Unity;
using Prism.Events;
using System;
using System.Collections.Concurrent;

namespace Arma3BE.Client.Modules.BEServerModule
{
    public class BEService
    {
        private readonly IUnityContainer _container;
        private readonly ILog _log;
        private readonly IIpService _ipService;
        private readonly IEventAggregator _eventAggregator;
        private ConcurrentDictionary<Guid, ServerItem> _serverPool = new ConcurrentDictionary<Guid, ServerItem>();

        public BEService(IUnityContainer container, ILog log, IIpService ipService, IEventAggregator eventAggregator)
        {
            _container = container;
            _log = log;
            _ipService = ipService;
            _eventAggregator = eventAggregator;

            _eventAggregator.GetEvent<RunServerEvent>().Subscribe(CheckServer);
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

        private class ServerItem
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
            }

            private void BEServer_PlayerHandler(object sender, Server.BEClientEventArgs<System.Collections.Generic.IEnumerable<Server.Models.Player>> e)
            {
                _eventAggregator.GetEvent<BEMessageEvent<BEItemsMessage<Server.Models.Player>>>().Publish(new BEItemsMessage<Server.Models.Player>(e.Data, Info.Id));
            }
        }
    }
}