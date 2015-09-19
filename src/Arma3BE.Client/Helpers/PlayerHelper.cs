using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Arma3BE.Server;
using Arma3BEClient.Common.Logging;
using Arma3BEClient.Helpers.Views;
using Arma3BEClient.Libs.Context;
using Arma3BEClient.Libs.ModelCompact;
using Arma3BEClient.Libs.Tools;
using Player = Arma3BE.Server.Models.Player;

namespace Arma3BEClient.Helpers
{
    public class PlayerHelper : StateHelper<Player>
    {
        private readonly ILog _log;
        private readonly Guid _serverId;
        private readonly UpdateClient _updateClient;

        private readonly Regex NameRegex = new Regex("[A-Za-zА-Яа-я0-9]+",
            RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

        public PlayerHelper(ILog log, Guid serverId, UpdateClient updateClient)
        {
            _log = log;
            _serverId = serverId;
            _updateClient = updateClient;
        }

        public bool RegisterPlayers(IEnumerable<Player> list)
        {
            var players = list.ToList();

            if (!HaveChanges(players, x => x.Num))
            {
                return false;
            }

            using (var context = new Arma3BeClientContext())
            {
                var guids = players.Select(x => x.Guid).ToList();

                var playersInDb = context.Player.Where(x => guids.Contains(x.GUID)).ToList();
                var dbGuids = playersInDb.Select(x => x.GUID).ToList();

                foreach (var player in playersInDb)
                {
                    var p = players.FirstOrDefault(x => x.Guid == player.GUID);
                    if (p != null)
                    {
                        if (player.Name != p.Name || player.LastIp != p.IP)
                        {
                            context.PlayerHistory.Add(new PlayerHistory
                            {
                                IP = player.LastIp,
                                Name = player.Name,
                                PlayerId = player.Id,
                                ServerId = _serverId
                            });

                            player.Name = p.Name;
                            player.LastIp = p.IP;
                        }

                        player.LastSeen = DateTime.UtcNow;
                    }
                }

                var newplayers = players.Where(x => !dbGuids.Contains(x.Guid)).ToList();

                if (newplayers.Any())
                {
                    foreach (var p in newplayers)
                    {
                        var np = new Libs.ModelCompact.Player
                        {
                            GUID = p.Guid,
                            Name = p.Name,
                            Id = Guid.NewGuid(),
                            LastIp = p.IP
                        };

                        context.Player.Add(np);

                        context.PlayerHistory.Add(new PlayerHistory
                        {
                            IP = np.LastIp,
                            Name = np.Name,
                            PlayerId = np.Id,
                            ServerId = _serverId
                        });
                    }
                }

                context.SaveChanges();
            }

            return true;
        }

        public IEnumerable<PlayerView> GetPlayerView(IEnumerable<Player> list)
        {
            using (var context = new Arma3BeClientContext())
            {
                var players = list.ToList();
                var guids = players.Select(x => x.Guid).ToList();

                var playersInDb = context.Player.Where(x => guids.Contains(x.GUID)).ToList();

                var result = players.Select(x => new PlayerView
                {
                    Guid = x.Guid,
                    IP = x.IP,
                    Name = x.Name,
                    Num = x.Num,
                    Ping = x.Ping,
                    State = x.State,
                    Port = x.Port
                }).ToList();

                result.ForEach(x =>
                {
                    var p = playersInDb.FirstOrDefault(y => y.GUID == x.Guid);
                    if (p != null)
                    {
                        x.Id = p.Id;
                        x.Comment = p.Comment;
                    }
                });

                var filterUsers = result.FirstOrDefault(x => !NameRegex.IsMatch(x.Name));
                if (filterUsers != null)
                {
                    KickAsync(filterUsers, "bot: Fill Nickname");
                }


                return result;
            }
        }

        public async Task KickAsync(PlayerView player, string reason, bool isAuto = false)
        {
            var totalreason = string.Format("[{0}][{1}] {2}", SettingsStore.Instance.AdminName,
                DateTime.UtcNow.ToString("dd.MM.yy HH:mm:ss"), reason);

            await _updateClient.SendCommandAsync(UpdateClient.CommandType.Kick,
                string.Format("{0} {1}", player.Num, totalreason));

            if (!isAuto)
            {
                using (var context = new Arma3BeClientContext())
                {
                    var user = context.Player.FirstOrDefault(x => x.GUID == player.Guid);
                    if (user != null)
                    {
                        user.Notes.Add(new Note
                        {
                            PlayerId = user.Id,
                            Text = string.Format("Kicked with reason: {0}", totalreason)
                        });

                        user.Comment = string.Format("{0} | {1}", user.Comment, reason);
                        context.SaveChanges();
                    }
                }
                await _updateClient.SendCommandAsync(UpdateClient.CommandType.Players);
            }
        }

        public async void BanGUIDOffline(string guid, string reason, long minutes, bool syncMode = false)
        {
            if (!syncMode)
            {
                var totalreason = string.Format("[{0}][{1}] {2}", SettingsStore.Instance.AdminName,
                    DateTime.UtcNow.ToString("dd.MM.yy HH:mm:ss"), reason);


                await _updateClient.SendCommandAsync(UpdateClient.CommandType.AddBan,
                    string.Format("{0} {1} {2}", guid, minutes, totalreason));


                using (var context = new Arma3BeClientContext())
                {
                    var user = context.Player.FirstOrDefault(x => x.GUID == guid);
                    if (user != null)
                    {
                        user.Notes.Add(new Note
                        {
                            PlayerId = user.Id,
                            Text = string.Format("Baned with reason: {0}", totalreason)
                        });

                        user.Comment = string.Format("{0} | {1}", user.Comment, reason);

                        context.SaveChanges();
                    }
                }


                _updateClient.SendCommandAsync(UpdateClient.CommandType.Bans);
            }
            else
            {
                _updateClient.SendCommandAsync(UpdateClient.CommandType.AddBan,
                    string.Format("{0} {1} {2}", guid, minutes, reason));
            }
        }

        public async void BanGUIDOnline(string num, string guid, string reason, long minutes)
        {
            var totalreason = string.Format("[{0}][{1}] {2}", SettingsStore.Instance.AdminName,
                DateTime.UtcNow.ToString("dd.MM.yy HH:mm:ss"), reason);


            await _updateClient.SendCommandAsync(UpdateClient.CommandType.Ban,
                string.Format("{0} {1} {2}", num, minutes, totalreason));


            using (var context = new Arma3BeClientContext())
            {
                var user = context.Player.FirstOrDefault(x => x.GUID == guid);
                if (user != null)
                {
                    user.Notes.Add(new Note
                    {
                        PlayerId = user.Id,
                        Text = string.Format("Baned with reason: {0}", totalreason)
                    });

                    user.Comment = string.Format("{0} | {1}", user.Comment, reason);

                    context.SaveChanges();
                }
            }


            _updateClient.SendCommandAsync(UpdateClient.CommandType.Players);
            _updateClient.SendCommandAsync(UpdateClient.CommandType.Bans);
        }
    }
}