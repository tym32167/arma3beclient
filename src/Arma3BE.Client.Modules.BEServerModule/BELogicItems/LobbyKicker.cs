using Arma3BE.Client.Infrastructure.Events.BE;
using Arma3BE.Client.Infrastructure.Settings;
using Arma3BE.Server;
using Arma3BE.Server.Models;
using Arma3BEClient.Common.Logging;
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


        private readonly ILog _logger = new Log();


        public LobbyKicker(IEventAggregator aggregator, ISettingsStoreSource settingsStoreSource)
        {
            _aggregator = aggregator;
            _settingsStoreSource = settingsStoreSource;
            _aggregator.GetEvent<BEMessageEvent<BEItemsMessage<Player>>>()
                .Subscribe(PlayersUpdate);
        }

        private void PlayersUpdate(BEItemsMessage<Player> data)
        {
            try
            {
                var settingsStore = _settingsStoreSource.GetCustomSettingsStore();
                var serversSettings = settingsStore.Load<ServerSettings>(data.ServerId.ToString());

                if (serversSettings?.IdleTimeInMins > 0 && serversSettings?.KickIdlePlayers == true)
                {
                    var idleTime = serversSettings.IdleTimeInMins;
                    if (!_servers.ContainsKey(data.ServerId)) _servers.Add(data.ServerId, new LobbyIdleStore());
                    var items = data.Items.ToArray();
                    _servers[data.ServerId].Update(items);

                    var span = TimeSpan.FromMinutes(idleTime);
                    var idlePlayers = _servers[data.ServerId].GetIdlePlayers(span, items);

                    var reason = serversSettings.IdleKickReason;

                    foreach (var idlePlayer in idlePlayers)
                    {
                        _aggregator.GetEvent<BEMessageEvent<BECommand>>()
                            .Publish(new BECommand(data.ServerId, CommandType.Kick, $"{idlePlayer} {reason}"));
                    }
                }
            }
            catch (Exception e)
            {
                _logger.Error(e);
            }
        }

        private class LobbyIdleStore
        {
            private Dictionary<int, DateTime> _idlePlayers = new Dictionary<int, DateTime>();

            public IEnumerable<int> GetIdlePlayers(TimeSpan span, Server.Models.Player[] players)
            {
                var now = DateTime.UtcNow;
                var target = now.Add(span.Negate());

                return players.Where(x => x.State == Player.PlayerState.Lobby).Where(x => _idlePlayers[x.Num] < target).Select(x => x.Num).ToArray();
            }

            public void Update(Server.Models.Player[] players)
            {
                var now = DateTime.UtcNow;
                var set = new HashSet<int>(players.Select(x => x.Num));
                var remove = _idlePlayers.Keys.Where(x => !set.Contains(x)).ToArray();
                foreach (var i in remove)
                {
                    _idlePlayers.Remove(i);
                }
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