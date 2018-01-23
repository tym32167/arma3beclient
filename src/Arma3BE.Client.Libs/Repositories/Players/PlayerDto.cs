using System;
using System.ComponentModel.DataAnnotations;

namespace Arma3BEClient.Libs.Repositories.Players
{
    public class PlayerDto
    {
        public PlayerDto()
        {
            LastSeen = DateTime.UtcNow;
        }

        [Key]
        public Guid Id { get; set; }

        public string GUID { get; set; }

        public string SteamId { get; set; }

        public string Name { get; set; }
        public string Comment { get; set; }

        public string LastIp { get; set; }
        public DateTime LastSeen { get; set; }
    }
}