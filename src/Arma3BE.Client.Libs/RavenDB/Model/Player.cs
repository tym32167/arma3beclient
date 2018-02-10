using System;

namespace Arma3BEClient.Libs.RavenDB.Model
{
    public class Player
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string IP { get; set; }
        public string SteamId { get; set; }
        public string GUID { get; set; }
        public DateTime LastSeen { get; set; }
    }
}