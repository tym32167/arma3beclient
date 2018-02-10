using System;

namespace Arma3BEClient.Libs.Core.Model
{
    public class ServerInfoDto
    {
        public Guid Id { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public int SteamPort { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; }
    }
}