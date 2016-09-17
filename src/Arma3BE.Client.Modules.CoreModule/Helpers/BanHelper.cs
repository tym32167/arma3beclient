using Arma3BE.Client.Infrastructure.Events.BE;
using Arma3BE.Client.Infrastructure.Helpers;
using Arma3BE.Client.Infrastructure.Helpers.Views;
using Arma3BE.Server;
using Arma3BE.Server.Models;
using Arma3BEClient.Common.Logging;
using Arma3BEClient.Libs.Repositories;
using Arma3BEClient.Libs.Tools;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Arma3BE.Client.Modules.BanModule.Helpers
{
    public class BanHelper : StateHelper<Ban>, IBanHelper
    {
        private readonly ILog _log;
        private readonly IEventAggregator _eventAggregator;
        private readonly Regex replace = new Regex(@"\[[^\]^\[]*\]", RegexOptions.Compiled | RegexOptions.Multiline);

        public BanHelper(ILog log, IEventAggregator eventAggregator)
        {
            _log = log;
            _eventAggregator = eventAggregator;
        }

        public void RegisterBans(IEnumerable<Ban> list, Guid currentServerId)
        {
            Task.Run(() => RegisterBansInternal(list, currentServerId));
        }

        private bool RegisterBansInternal(IEnumerable<Ban> list, Guid currentServerId)
        {
            var bans = list.ToList();

            if (!HaveChanges(bans, x => x.Num, (x, y) => x.GuidIp == y.GuidIp && x.Reason == y.Reason && x.Num == y.Num))
                return false;


            using (var banRepository = new BanRepository())
            {
                using (var playerRepository = PlayerRepositoryFactory.Create())
                {
                    var db =
                        banRepository.GetActiveBans(currentServerId);

                    var ids = bans.Select(x => x.GuidIp).ToList();

                    ids.AddRange(db.Select(x => x.GuidIp).ToList());
                    ids = ids.Distinct().ToList();

                    var players = playerRepository.GetPlayers(ids.ToArray());

                    var bansToUpdate = new List<Arma3BEClient.Libs.ModelCompact.Ban>();
                    var bansToAdd = new List<Arma3BEClient.Libs.ModelCompact.Ban>();
                    var playersToUpdateComments = new Dictionary<Guid, string>();

                    foreach (var ban in db)
                    {
                        bool needUpdate = false;

                        var actual = bans.FirstOrDefault(x => x.GuidIp == ban.GuidIp);
                        if (actual == null)
                        {
                            ban.IsActive = false;
                            needUpdate = true;
                        }
                        else
                        {
                            if (ban.MinutesLeft != actual.Minutesleft)
                            {
                                ban.MinutesLeft = actual.Minutesleft;
                                needUpdate = true;
                            }

                            if (ban.PlayerId == null)
                            {
                                var player = players.FirstOrDefault(x => x.GUID == ban.GuidIp);
                                if (player != null && !string.IsNullOrEmpty(ban.Reason))
                                {
                                    ban.PlayerId = player.Id;

                                    var comm = ban.Reason;

                                    var ind1 = comm.IndexOf('[');
                                    var ind2 = comm.LastIndexOf(']');

                                    if (ind1 > -1 && ind2 > ind1) comm = comm.Remove(ind1, ind2 - ind1 + 1).Trim();

                                    if (string.IsNullOrEmpty(player.Comment) || !player.Comment.Contains(comm))
                                    {
                                        player.Comment = $"{player.Comment} | {comm}";

                                        if (!playersToUpdateComments.ContainsKey(player.Id))
                                            playersToUpdateComments.Add(player.Id, player.Comment);
                                        else
                                            playersToUpdateComments[player.Id] = player.Comment;
                                    }

                                    needUpdate = true;
                                }
                            }
                        }

                        if (needUpdate)
                            bansToUpdate.Add(ban);
                    }

                    foreach (var ban in bans)
                    {
                        var bdb = db.FirstOrDefault(x => x.ServerId == currentServerId && x.GuidIp == ban.GuidIp);
                        if (bdb == null)
                        {
                            var player = players.FirstOrDefault(x => x.GUID == ban.GuidIp);

                            var newBan = new Arma3BEClient.Libs.ModelCompact.Ban
                            {
                                CreateDate = DateTime.UtcNow,
                                GuidIp = ban.GuidIp,
                                IsActive = true,
                                Minutes = ban.Minutesleft,
                                MinutesLeft = ban.Minutesleft,
                                Num = ban.Num,
                                Reason = ban.Reason,
                                ServerId = currentServerId,
                                PlayerId = player?.Id
                            };

                            bansToAdd.Add(newBan);

                            var comm = replace.Replace(ban.Reason, string.Empty).Trim();
                            if (player != null &&
                                (string.IsNullOrEmpty(player.Comment) || !player.Comment.Contains(comm)))
                            {
                                player.Comment = $"{player.Comment} | {comm}";

                                if (!playersToUpdateComments.ContainsKey(player.Id))
                                    playersToUpdateComments.Add(player.Id, player.Comment);
                                else
                                    playersToUpdateComments[player.Id] = player.Comment;
                            }
                        }
                    }

                    banRepository.AddOrUpdate(bansToAdd);
                    banRepository.AddOrUpdate(bansToUpdate);
                    playerRepository.UpdateCommant(playersToUpdateComments);
                }
            }

            return true;
        }

        public IEnumerable<BanView> GetBanView(IEnumerable<Ban> list)
        {
            using (var context = PlayerRepositoryFactory.Create())
            {
                var bans = list as Ban[] ?? list.ToArray();
                var guids = bans.Select(x => x.GuidIp).ToArray();
                var players = context.GetPlayers(guids);

                return bans.Select(x =>
                {
                    var p = players.FirstOrDefault(y => y.GUID == x.GuidIp);

                    var ban = new BanView
                    {
                        Num = x.Num,
                        GuidIp = x.GuidIp,
                        Reason = x.Reason,
                        Minutesleft = x.Minutesleft
                    };

                    if (p != null)
                    {
                        ban.PlayerComment = p.Comment;
                        ban.PlayerName = p.Name;
                    }
                    return ban;
                }).ToList();
            }
        }

        private void SendCommand(Guid serverId, CommandType commandType, string parameters = null)
        {
            _eventAggregator.GetEvent<BEMessageEvent<BECommand>>().Publish(new BECommand(serverId, commandType, parameters));
        }

        public void Kick(Guid serverId, int playerNum, string playerGuid, string reason, bool isAuto = false)
        {
            var totalreason =
                $"[{SettingsStore.Instance.AdminName}][{DateTime.UtcNow.ToString("dd.MM.yy HH:mm:ss")}] {reason}";

            SendCommand(serverId, CommandType.Kick,
                $"{playerNum} {totalreason}");

            if (!isAuto)
            {
                using (var context = PlayerRepositoryFactory.Create())
                {
                    var user = context.GetPlayer(playerGuid);
                    if (user != null)
                    {
                        context.AddNotes(user.Id, $"Kicked with reason: {totalreason}");
                        user.Comment = $"{user.Comment} | {reason}";
                        context.UpdatePlayerComment(user.GUID, user.Comment);
                    }
                }
                SendCommand(serverId, CommandType.Players);
            }
        }

        public void BanGUIDOffline(Guid serverId, string guid, string reason, long minutes, bool syncMode = false)
        {
            if (!syncMode)
            {
                var totalreason =
                    $"[{SettingsStore.Instance.AdminName}][{DateTime.UtcNow.ToString("dd.MM.yy HH:mm:ss")}] {reason}";


                SendCommand(serverId, CommandType.AddBan,
                    $"{guid} {minutes} {totalreason}");


                using (var context = PlayerRepositoryFactory.Create())
                {
                    var user = context.GetPlayer(guid);
                    if (user != null)
                    {
                        context.AddNotes(user.Id, $"Baned with reason: {totalreason}");
                        user.Comment = $"{user.Comment} | {reason}";
                        context.UpdatePlayerComment(user.GUID, user.Comment);
                    }
                }


#pragma warning disable 4014
                SendCommand(serverId, CommandType.Bans);
#pragma warning restore 4014
            }
            else
            {
#pragma warning disable 4014
                SendCommand(serverId, CommandType.AddBan,
#pragma warning restore 4014
                            $"{guid} {minutes} {reason}");
            }
        }

        public async void BanGuidOnline(Guid serverId, string num, string guid, string reason, long minutes)
        {
            var totalreason =
                $"[{SettingsStore.Instance.AdminName}][{DateTime.UtcNow.ToString("dd.MM.yy HH:mm:ss")}] {reason}";


            SendCommand(serverId, CommandType.Ban,
                $"{num} {minutes} {totalreason}");


            using (var context = PlayerRepositoryFactory.Create())
            {
                var user = context.GetPlayer(guid);
                if (user != null)
                {
                    context.AddNotes(user.Id, $"Baned with reason: {totalreason}");
                    user.Comment = $"{user.Comment} | {reason}";
                    context.UpdatePlayerComment(user.GUID, user.Comment);
                }
            }


#pragma warning disable 4014
            SendCommand(serverId, CommandType.Players);
            SendCommand(serverId, CommandType.Bans);
        }
    }
}