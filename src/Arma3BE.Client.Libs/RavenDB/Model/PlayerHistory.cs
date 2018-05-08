using System;

namespace Arma3BEClient.Libs.RavenDB.Model
{
    public class PlayerHistory
    {
        public string Id { get; set; }
        public string PlayerId { get; set; }

        public string Name { get; set; }
        public string IP { get; set; }
        public DateTime Date { get; set; }
        public string ServerId { get; set; }
    }
}