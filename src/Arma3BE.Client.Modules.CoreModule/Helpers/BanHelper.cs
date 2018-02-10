using Arma3BE.Client.Infrastructure.Events.BE;
using Arma3BE.Client.Infrastructure.Helpers;
using Arma3BE.Client.Infrastructure.Helpers.Views;
using Arma3BE.Server;
using Arma3BE.Server.Models;
using Arma3BEClient.Common.Extensions;
using Arma3BEClient.Common.Logging;
using Arma3BEClient.Libs.Tools;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Arma3BEClient.Libs.Core;
using Arma3BEClient.Libs.Core.Settings;
using Arma3BEClient.Libs.EF.Repositories;

namespace Arma3BE.Client.Modules.CoreModule.Helpers
{
    public class BanHelper : StateHelper<Ban>, IBanHelper
    {
        private readonly ILog _log = LogFactory.Create(new StackTrace().GetFrame(0).GetMethod().DeclaringType);
        private readonly IEventAggregator _eventAggregator;
        private readonly IPlayerRepository _playerRepository;
        private readonly ISettingsStoreSource _settingsStoreSource;
        private readonly Regex _replace = new Regex(@"\[[^\]^\[]*\]", RegexOptions.Compiled | RegexOptions.Multiline);
        private readonly MessageHelper _messageHelper;

        public BanHelper(IEventAggregator eventAggregator, IPlayerRepository playerRepository, ISettingsStoreSource settingsStoreSource)
        {
            _eventAggregator = eventAggregator;
            _playerRepository = playerRepository;
            _settingsStoreSource = settingsStoreSource;
            _messageHelper = new MessageHelper();
        }

        public Task RegisterBans(IEnumerable<Ban> list, Guid currentServerId)
        {
            return Task.Run(() => RegisterBansInternal(list, currentServerId));
        }

        private async Task<bool> RegisterBansInternal(IEnumerable<Ban> list, Guid currentServerId)
        {
            var bans = list.ToList();

            if (!HaveChanges(bans, x => x.Num, (x, y) => x.GuidIp == y.GuidIp && x.Reason == y.Reason && x.Num == y.Num))
                return false;


            using (var banRepository = new BanRepository())
            {

                var db =
                    (await banRepository.GetActiveBansAsync(currentServerId)).ToArray();

                var ids = bans.Select(x => x.GuidIp).ToList();

                ids.AddRange(db.Select(x => x.GuidIp).ToList());
                ids = ids.Distinct().ToList();

                var players = (await _playerRepository.GetPlayersAsync(ids.ToArray())).ToArray();

                var bansToUpdate = new List<Arma3BEClient.Libs.EF.Model.Ban>();
                var bansToAdd = new List<Arma3BEClient.Libs.EF.Model.Ban>();
                var playersToUpdateComments = new Dictionary<Guid, string>();

                foreach (var ban in db)
                {
                    var needUpdate = false;

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

                        var newBan = new Arma3BEClient.Libs.EF.Model.Ban
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

                        var comm = _replace.Replace(ban.Reason, string.Empty).Trim();
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

                await banRepository.AddOrUpdateAsync(bansToAdd);
                await banRepository.AddOrUpdateAsync(bansToUpdate);
                await _playerRepository.UpdateCommentAsync(playersToUpdateComments);
            }


            return true;
        }

        public async Task<IEnumerable<BanView>> GetBanViewAsync(IEnumerable<Ban> list)
        {
            using (_log.Time("GetBanViewAsync"))
            {
                var bans = list as Ban[] ?? list.ToArray();
                var guids = bans.Select(x => x.GuidIp).ToArray();
                var players = await _playerRepository.GetPlayersAsync(guids);

                return bans.Select(x =>
                {
                    var p = players.FirstOrDefault(y => y.GUID == x.GuidIp);

                    var ban = new BanView
                    {
                        Num = x.Num,
                        GuidIp = x.GuidIp,
                        Reason = x.Reason,
                        Minutesleft = x.Minutesleft,
                    };

                    if (p != null)
                    {
                        ban.PlayerComment = p.Comment;
                        ban.PlayerName = p.Name;
                        ban.SteamId = p.SteamId;
                    }
                    return ban;
                }).ToList();
            }
        }

        private void SendCommand(Guid serverId, CommandType commandType, string parameters = null)
        {
            _eventAggregator.GetEvent<BEMessageEvent<BECommand>>()
                .Publish(new BECommand(serverId, commandType, parameters));
        }


        public async Task KickAsync(Guid serverId, int playerNum, string playerGuid, string reason, bool isAuto = false)
        {
            var totalreason = _messageHelper.GetKickMessage(_settingsStoreSource.GetSettingsStore(), reason);

            SendCommand(serverId, CommandType.Kick,
                $"{playerNum} {totalreason}");

            if (!isAuto)
            {

                var user = await _playerRepository.GetPlayerAsync(playerGuid);
                if (user != null)
                {
                    await _playerRepository.AddNotesAsync(user.Id, $"Kicked with reason: {totalreason}");
                    user.Comment = $"{user.Comment} | {reason}";
                    await _playerRepository.UpdatePlayerCommentAsync(user.GUID, user.Comment);
                }

            }
        }

        public async Task BanGUIDOfflineAsync(Guid serverId, string guid, string reason, long minutes, bool syncMode = false)
        {
            if (!syncMode)
            {
                var totalreason = _messageHelper.GetBanMessage(_settingsStoreSource.GetSettingsStore(), reason, minutes);

                SendCommand(serverId, CommandType.AddBan,
                    $"{guid} {minutes} {totalreason}");

                var user = await _playerRepository.GetPlayerAsync(guid);
                if (user != null)
                {
                    await _playerRepository.AddNotesAsync(user.Id, $"Baned with reason: {totalreason}");
                    user.Comment = $"{user.Comment} | {reason}";
                    await _playerRepository.UpdatePlayerCommentAsync(user.GUID, user.Comment);
                }


            }
            else
            {
#pragma warning disable 4014
                SendCommand(serverId, CommandType.AddBan,
#pragma warning restore 4014
                    $"{guid} {minutes} {reason}");
            }
        }

        public async Task BanGUIDOfflineAsync(Guid serverId, BanView[] bans, bool syncMode = false)
        {
            foreach (var ban in bans)
            {
                await BanGUIDOfflineAsync(serverId, ban.GuidIp, ban.Reason, ban.Minutesleft, syncMode);
            }

        }

        public async Task BanGuidOnlineAsync(Guid serverId, string num, string guid, string reason, long minutes)
        {
            var totalreason = _messageHelper.GetBanMessage(_settingsStoreSource.GetSettingsStore(), reason, minutes);


            SendCommand(serverId, CommandType.Ban,
                $"{num} {minutes} {totalreason}");



            var user = await _playerRepository.GetPlayerAsync(guid);
            if (user != null)
            {
                await _playerRepository.AddNotesAsync(user.Id, $"Baned with reason: {totalreason}");
                user.Comment = $"{user.Comment} | {reason}";
                await _playerRepository.UpdatePlayerCommentAsync(user.GUID, user.Comment);
            }

        }
    }
}