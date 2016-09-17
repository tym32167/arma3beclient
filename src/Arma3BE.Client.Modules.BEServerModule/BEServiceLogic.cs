using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Arma3BE.Client.Infrastructure.Events;
using Arma3BE.Client.Infrastructure.Events.BE;
using Arma3BE.Server;
using Arma3BEClient.Common.Core;
using Arma3BEClient.Common.Logging;
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


        public BEServiceLogic(IEventAggregator aggregator, BEService beService, ILog log)
        {
            _aggregator = aggregator;
            _beService = beService;

            _playersUpdater = new TimedAction(SettingsStore.Instance.PlayersUpdateSeconds, UpdatePlayers, log);
            _bansUpdater = new TimedAction(SettingsStore.Instance.BansUpdateSeconds, UpdateBans, log);

            _aggregator.GetEvent<SettingsChangedEvent>()
                .Subscribe(SettingsChanged);

            _aggregator.GetEvent<BEMessageEvent<BECommand>>().Subscribe(ProcessCommand, ThreadOption.BackgroundThread);

            _aggregator.GetEvent<BEMessageEvent<BEPlayerLogMessage>>().Subscribe(Proc_BEPlayerLogMessage, ThreadOption.BackgroundThread);
        }



        private void ProcessCommand(BECommand command)
        {
            if (command.CommandType == CommandType.RemoveBan || command.CommandType == CommandType.AddBan || command.CommandType == CommandType.Ban)
            {
                Task.Delay(2000).ContinueWith(t => _bansUpdater.Update(command.ServerId));
            }
        }

        private void Proc_BEPlayerLogMessage(BEPlayerLogMessage message)
        {
            _playersUpdater.Update(message.ServerId);
        }


        private void SettingsChanged(SettingsStore settings)
        {
            _playersUpdater.SetTimer(SettingsStore.Instance.PlayersUpdateSeconds);
            _bansUpdater.SetTimer(SettingsStore.Instance.BansUpdateSeconds);
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
        private readonly ILog _log;

        private Timer _timer;

        private ConcurrentDictionary<Guid, byte> _servers = new ConcurrentDictionary<Guid, byte>();

        public TimedAction(int interval, Action<HashSet<Guid>> action, ILog log)
        {
            _interval = interval;
            _action = action;
            _log = log;

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
            try
            {
                if (_servers.Any())
                {
                    var set = new HashSet<Guid>(_servers.Keys.ToArray());
                    _servers.Clear();
                    _action(set);
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }
            finally
            {
                _timer?.Change(Interval(), Timeout.Infinite);
            }
        }

        protected override void DisposeManagedResources()
        {
            base.DisposeManagedResources();
            _timer?.Dispose();
            _timer = null;
        }
    }
}