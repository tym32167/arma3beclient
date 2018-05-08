using System;

namespace Arma3BEClient.Libs.RavenDB.Model
{
    public class Ban
    {
        public string Id { get; set; }
        public string PlayerId { get; set; }
        public int Num { get; set; }
        public string ServerId { get; set; }
        public string GuidIp { get; set; }
        public int Minutes { get; set; }
        public int MinutesLeft { get; set; }
        public string Reason { get; set; }
        public DateTime CreateDate { get; set; }
        public bool IsActive { get; set; }
    }
}