using Arma3BE.Client.Infrastructure.Events;
using Arma3BE.Client.Infrastructure.Events.BE;
using Arma3BE.Server;
using Arma3BEClient.Common.Core;
using Arma3BEClient.Common.Logging;
using Arma3BEClient.Libs.Tools;
using Prism.Events;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Arma3BE.Client.Modules.BEServerModule
{
    public class BEServiceLogic : DisposeObject
    {
        private readonly IEventAggregator _aggregator;
        private readonly IBEService _beService;
        private readonly ISettingsStoreSource _settingsStoreSource;
        private readonly IBELogic _beLogic;

        private readonly TimedAction _playersUpdater;
        private readonly TimedAction _bansUpdater;

        private readonly TimedAction _adminsUpdater;
        private readonly TimedAction _missionsUpdater;

        public BEServiceLogic(IEventAggregator aggregator, IBEService beService, ISettingsStoreSource settingsStoreSource, IBELogic beLogic)
        {
            _aggregator = aggregator;
            _beService = beService;
            _settingsStoreSource = settingsStoreSource;
            _beLogic = beLogic;

            _playersUpdater = new TimedAction(_settingsStoreSource.GetSettingsStore().PlayersUpdateSeconds, UpdatePlayers);
            _bansUpdater = new TimedAction(_settingsStoreSource.GetSettingsStore().BansUpdateSeconds, UpdateBans);

            _adminsUpdater = new TimedAction(3, UpdateAdmins); ;
            _missionsUpdater = new TimedAction(4, UpdateMissions); ;


            _aggregator.GetEvent<SettingsChangedEvent>()
                .Subscribe(SettingsChanged);


            _beLogic.ServerUpdateHandler += _beLogic_ServerUpdateHandler;
            _beLogic.ServerImmediateUpdateHandler += _beLogic_ServerImmediateUpdateHandler;

        }

        private void _beLogic_ServerImmediateUpdateHandler(object sender, ServerCommandEventArgs e)
        {
            _aggregator.GetEvent<BEMessageEvent<BECommand>>()
                       .Publish(e.Command);
        }

        private void _beLogic_ServerUpdateHandler(object sender, ServerCommandEventArgs e)
        {
            switch (e.Command.CommandType)
            {
                case CommandType.Players:
                    _playersUpdater.Update(e.Command.ServerId);
                    break;
                case CommandType.Bans:
                    _bansUpdater.Update(e.Command.ServerId);
                    break;

                case CommandType.Admins:
                    _adminsUpdater.Update(e.Command.ServerId);
                    break;
                case CommandType.Missions:
                    _missionsUpdater.Update(e.Command.ServerId);
                    break;

                default:
                    _aggregator.GetEvent<BEMessageEvent<BECommand>>()
                        .Publish(e.Command);
                    break;
            }
        }

        private void SettingsChanged(ISettingsStore settings)
        {
            _playersUpdater.SetTimer(_settingsStoreSource.GetSettingsStore().PlayersUpdateSeconds);
            _bansUpdater.SetTimer(_settingsStoreSource.GetSettingsStore().BansUpdateSeconds);
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


        private void UpdateAdmins(HashSet<Guid> servers)
        {
            foreach (var server in _beService.ConnectedServers())
            {
                if (servers.Contains(server))
                {
                    _aggregator.GetEvent<BEMessageEvent<BECommand>>()
                        .Publish(new BECommand(server, CommandType.Admins));
                }
            }
        }

        private void UpdateMissions(HashSet<Guid> servers)
        {
            foreach (var server in _beService.ConnectedServers())
            {
                if (servers.Contains(server))
                {
                    _aggregator.GetEvent<BEMessageEvent<BECommand>>()
                        .Publish(new BECommand(server, CommandType.Missions));
                }
            }
        }


        protected override void DisposeManagedResources()
        {
            base.DisposeManagedResources();

            _beLogic.ServerUpdateHandler -= _beLogic_ServerUpdateHandler;
            _playersUpdater?.Dispose();
            _bansUpdater?.Dispose();
        }
    }

    public class TimedAction : DisposeObject
    {
        private int _interval;
        private readonly Action<HashSet<Guid>> _action;
        private readonly ILog _log = LogFactory.Create(new StackTrace().GetFrame(0).GetMethod().DeclaringType);

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
            return Math.Max(5000, i * 1000);
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