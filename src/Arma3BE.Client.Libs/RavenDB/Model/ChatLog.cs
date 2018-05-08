using System;

namespace Arma3BEClient.Libs.RavenDB.Model
{
    public class ChatLog
    {
        public string Id { get; set; }
        public string ServerId { get; set; }
        public string Text { get; set; }
        public DateTime Date { get; set; }
    }
}