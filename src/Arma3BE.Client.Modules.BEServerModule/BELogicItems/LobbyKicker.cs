using Arma3BE.Client.Infrastructure.Events.BE;
using Arma3BE.Server;
using Arma3BE.Server.Models;
using Arma3BEClient.Libs.Tools;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Arma3BE.Client.Modules.BEServerModule.BELogicItems
{
    public class LobbyKicker : IBELogicItem
    {
        private readonly IEventAggregator _aggregator;
        private readonly ISettingsStoreSource _settingsStoreSource;

        private Dictionary<Guid, LobbyIdleStore> _servers = new Dictionary<Guid, LobbyIdleStore>();

        public LobbyKicker(IEventAggregator aggregator, ISettingsStoreSource settingsStoreSource)
        {
            _aggregator = aggregator;
            _settingsStoreSource = settingsStoreSource;
            _aggregator.GetEvent<BEMessageEvent<BEItemsMessage<Player>>>()
                .Subscribe(PlayersUpdate);
        }

        private void PlayersUpdate(BEItemsMessage<Player> data)
        {
            var settings = _settingsStoreSource.GetSettingsStore();
            var idleTime = settings.IdleTimeInMins;

            if (idleTime > 0)
            {
                if (!_servers.ContainsKey(data.ServerId)) _servers.Add(data.ServerId, new LobbyIdleStore());
                var items = data.Items.ToArray();
                _servers[data.ServerId].Update(items);

                var span = TimeSpan.FromMinutes(idleTime);
                var idlePlayers = _servers[data.ServerId].GetIdlePlayers(span, items);

                var reason = settings.IdleKickText;

                foreach (var idlePlayer in idlePlayers)
                {
                    _aggregator.GetEvent<BEMessageEvent<BECommand>>()
                        .Publish(new BECommand(data.ServerId, CommandType.Kick, $"{idlePlayer} {reason}"));
                }
            }
        }

        private class LobbyIdleStore
        {
            private Dictionary<int, DateTime> _idlePlayers = new Dictionary<int, DateTime>();

            public IEnumerable<int> GetIdlePlayers(TimeSpan span, Server.Models.Player[] players)
            {
                var now = DateTime.UtcNow;
                var target = now.Add(span.Negate());

                return players.Where(x => _idlePlayers[x.Num] < target).Select(x => x.Num).ToArray();
            }

            public void Update(Server.Models.Player[] players)
            {
                var now = DateTime.UtcNow;
                foreach (var p in players)
                {
                    if (!_idlePlayers.ContainsKey(p.Num))
                    {
                        _idlePlayers.Add(p.Num, now);
                    }
                    else
                    {
                        if (p.State != Player.PlayerState.Lobby)
                        {
                            _idlePlayers[p.Num] = now;
                        }
                    }
                }
            }

        }
    }
}