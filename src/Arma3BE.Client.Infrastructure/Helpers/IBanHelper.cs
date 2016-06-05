using Arma3BE.Client.Infrastructure.Helpers.Views;
using Arma3BE.Server.Models;
using System;
using System.Collections.Generic;

namespace Arma3BE.Client.Infrastructure.Helpers
{
    public interface IBanHelper
    {
        void BanGUIDOffline(Guid serverId, string guid, string reason, long minutes, bool syncMode = false);
        void BanGuidOnline(Guid serverId, string num, string guid, string reason, long minutes);
        IEnumerable<BanView> GetBanView(IEnumerable<Ban> list);
        void Kick(Guid serverId, int playerNum, string playerGuid, string reason, bool isAuto = false);
        void RegisterBans(IEnumerable<Ban> list, Guid currentServerId);
    }
}