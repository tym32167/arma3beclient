namespace Arma3BEClient.Libs.RavenDB.Model
{
    public class ServerInfo
    {
        public string Id { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; }
        public int SteamPort { get; set; }
    }
}