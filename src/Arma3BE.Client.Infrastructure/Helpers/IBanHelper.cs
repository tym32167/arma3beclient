using Arma3BE.Client.Infrastructure.Helpers.Views;
using Arma3BE.Server.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Arma3BE.Client.Infrastructure.Helpers
{
    public interface IBanHelper
    {
        Task BanGUIDOfflineAsync(Guid serverId, string guid, string reason, long minutes, bool syncMode = false);

        Task BanGUIDOfflineAsync(Guid serverId, BanView[] bans, bool syncMode = false);

        Task BanGuidOnlineAsync(Guid serverId, string num, string guid, string reason, long minutes);
        Task<IEnumerable<BanView>> GetBanViewAsync(IEnumerable<Ban> list);
        Task KickAsync(Guid serverId, int playerNum, string playerGuid, string reason, bool isAuto = false);
        Task RegisterBans(IEnumerable<Ban> list, Guid currentServerId);
    }
}