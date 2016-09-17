using System;
using System.Threading;
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

        private void UpdatePlayers()
        {
            foreach (var server in _beService.ConnectedServers())
            {
                _aggregator.GetEvent<BEMessageEvent<BECommand>>()
                   .Publish(new BECommand(server, CommandType.Players));
            }
        }
        
        private void UpdateBans()
        {
            foreach (var server in _beService.ConnectedServers())
            {
                _aggregator.GetEvent<BEMessageEvent<BECommand>>()
                   .Publish(new BECommand(server, CommandType.Bans));
            }
        }
    }

    public class TimedAction : DisposeObject
    {
        private int _interval;
        private readonly Action _action;

        private Timer _timer;
        private bool _requiredCall;

        public TimedAction(int interval, Action action)
        {
            _interval = interval;
            _action = action;
            
            _timer = new Timer(Tick, null, Interval(), Timeout.Infinite);
        }

        public void Update()
        {
            _requiredCall = true;
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
            return Math.Max(1000, i*1000);
        }

        private void Tick(object state)
        {
            if (_requiredCall)
            {
                _requiredCall = false;
                _action();
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