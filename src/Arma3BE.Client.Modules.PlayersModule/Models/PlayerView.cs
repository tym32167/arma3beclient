using Arma3BEClient.Common.Attributes;
using System;

namespace Arma3BE.Client.Modules.PlayersModule.Models
{
    public class PlayerView
    {
        public Guid Id { get; set; }

        [ShowInUi]
        [EnableCopy]
        public string Name { get; set; }

        [ShowInUi]
        [EnableCopy]
        public string Comment { get; set; }

        [ShowInUi]
        [EnableCopy]
        public string Guid { get; set; }

        [ShowInUi]
        [EnableCopy]
        public DateTime LastSeen { get; set; }

        [ShowInUi]
        [EnableCopy]
        public string LastIp { get; set; }

        [ShowInUi]
        [EnableCopy]
        public string SteamId { get; set; }
    }
}