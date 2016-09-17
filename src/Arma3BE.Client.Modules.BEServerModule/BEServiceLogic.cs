using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Arma3BE.Client.Infrastructure.Events;
using Arma3BE.Client.Infrastructure.Events.BE;
using Arma3BE.Server;
using Arma3BEClient.Common.Core;
using Arma3BEClient.Libs.Tools;
using Prism.Events;

namespace Arma3BE.Client.Modules.BEServerModule
{
    public class BEServiceLogic
    {
        private readonly IEventAggregator _aggregator;
        private readonly BEService _beService;

        private readonly TimedAction _playersUpdater;
        private readonly TimedAction _bansUpdater;


        public BEServiceLogic(IEventAggregator aggregator, BEService beService)
        {
            _aggregator = aggregator;
            _beService = beService;

            _playersUpdater = new TimedAction(SettingsStore.Instance.PlayersUpdateSeconds, UpdatePlayers);
            _bansUpdater = new TimedAction(SettingsStore.Instance.BansUpdateSeconds, UpdateBans);

            _aggregator.GetEvent<BEMessageEvent<BEPlayerLogMessage>>()
                .Subscribe(PlayerLogMessage);

            _aggregator.GetEvent<SettingsChangedEvent>()
                .Subscribe(SettingsChanged);

            _aggregator.GetEvent<BEMessageEvent<BECommand>>().Subscribe(ProcessCommand);
        }



        private void ProcessCommand(BECommand command)
        {
            if (command.CommandType == CommandType.RemoveBan || command.CommandType == CommandType.AddBan || command.CommandType == CommandType.Ban)
            {
                Task.Delay(2000).ContinueWith(t => _bansUpdater.Update(command.ServerId));
            }
        }


        private void SettingsChanged(SettingsStore settings)
        {
            _playersUpdater.SetTimer(SettingsStore.Instance.PlayersUpdateSeconds);
            _bansUpdater.SetTimer(SettingsStore.Instance.BansUpdateSeconds);
        }

        private void PlayerLogMessage(BEPlayerLogMessage message)
        {
            _aggregator.GetEvent<BEMessageEvent<BECommand>>()
                   .Publish(new BECommand(message.ServerId, CommandType.Players));
        }

        private void UpdatePlayers(HashSet<Guid> servers)
        {
            foreach (var server in _beService.ConnectedServers())
            {
                if (servers.Contains(server))
                {
                    _aggregator.GetEvent<BEMessageEvent<BECommand>>()
                        .Publish(new BECommand(server, CommandType.Players));
                }
            }
        }
        
        private void UpdateBans(HashSet<Guid> servers)
        {
            foreach (var server in _beService.ConnectedServers())
            {
                if (servers.Contains(server))
                {
                    _aggregator.GetEvent<BEMessageEvent<BECommand>>()
                        .Publish(new BECommand(server, CommandType.Bans));
                }
            }
        }
    }

    public class TimedAction : DisposeObject
    {
        private int _interval;
        private readonly Action<HashSet<Guid>> _action;

        private Timer _timer;

        private ConcurrentDictionary<Guid, byte> _servers = new ConcurrentDictionary<Guid, byte>();

        public TimedAction(int interval, Action<HashSet<Guid>> action)
        {
            _interval = interval;
            _action = action;
            
            _timer = new Timer(Tick, null, Interval(), Timeout.Infinite);
        }

        public void Update(Guid server)
        {
            _servers.AddOrUpdate(server, 0, (guid, b) => b);
        }

        public void SetTimer(int intervalInSeconds)
        {
            _timer?.Dispose();
            _timer = null;
            _interval = intervalInSeconds;
            _timer = new Timer(Tick, null, Interval(), Timeout.Infinite);
        }

        private int Interval()
        {
            var i = _interval;
            return Math.Max(5000, i*1000);
        }

        private void Tick(object state)
        {
            if (_servers.Any())
            {
                var set = new HashSet<Guid>(_servers.Keys.ToArray());
                _servers.Clear();
                _action(set);
            }

            _timer?.Change(Interval(), Timeout.Infinite);
        }

        protected override void DisposeManagedResources()
        {
            base.DisposeManagedResources();
            _timer?.Dispose();
            _timer = null;
        }
    }
}