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
        private readonly IBEServer _beServer;
        private readonly ILog _log;
        private readonly Guid _serverId;

        private readonly Regex NameRegex = new Regex("[A-Za-zА-Яа-я0-9]+",
            RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

        public PlayerHelper(ILog log, Guid serverId, IBEServer beServer)
        {
            _log = log;
            _serverId = serverId;
            _beServer = beServer;
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
#pragma warning disable 4014
                    KickAsync(filterUsers, "bot: Fill Nickname");
#pragma warning restore 4014
                }


                return result;
            }
        }

        public async Task KickAsync(PlayerView player, string reason, bool isAuto = false)
        {
            var totalreason =
                $"[{SettingsStore.Instance.AdminName}][{DateTime.UtcNow.ToString("dd.MM.yy HH:mm:ss")}] {reason}";

            await _beServer.SendCommandAsync(CommandType.Kick,
                $"{player.Num} {totalreason}");

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
                            Text = $"Kicked with reason: {totalreason}"
                        });

                        user.Comment = $"{user.Comment} | {reason}";
                        context.SaveChanges();
                    }
                }
                await _beServer.SendCommandAsync(CommandType.Players);
            }
        }

        public async void BanGUIDOffline(string guid, string reason, long minutes, bool syncMode = false)
        {
            if (!syncMode)
            {
                var totalreason =
                    $"[{SettingsStore.Instance.AdminName}][{DateTime.UtcNow.ToString("dd.MM.yy HH:mm:ss")}] {reason}";


                await _beServer.SendCommandAsync(CommandType.AddBan,
                    $"{guid} {minutes} {totalreason}");


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


#pragma warning disable 4014
                _beServer.SendCommandAsync(CommandType.Bans);
#pragma warning restore 4014
            }
            else
            {
#pragma warning disable 4014
                _beServer.SendCommandAsync(CommandType.AddBan,
#pragma warning restore 4014
                    $"{guid} {minutes} {reason}");
            }
        }

        public async void BanGUIDOnline(string num, string guid, string reason, long minutes)
        {
            var totalreason =
                $"[{SettingsStore.Instance.AdminName}][{DateTime.UtcNow.ToString("dd.MM.yy HH:mm:ss")}] {reason}";


            await _beServer.SendCommandAsync(CommandType.Ban,
                $"{num} {minutes} {totalreason}");


            using (var context = new Arma3BeClientContext())
            {
                var user = context.Player.FirstOrDefault(x => x.GUID == guid);
                if (user != null)
                {
                    user.Notes.Add(new Note
                    {
                        PlayerId = user.Id,
                        Text = $"Baned with reason: {totalreason}"
                    });

                    user.Comment = $"{user.Comment} | {reason}";

                    context.SaveChanges();
                }
            }


#pragma warning disable 4014
            _beServer.SendCommandAsync(CommandType.Players);

            _beServer.SendCommandAsync(CommandType.Bans);
        }
    }
}