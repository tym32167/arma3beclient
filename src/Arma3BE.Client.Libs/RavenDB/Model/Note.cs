using System;

namespace Arma3BEClient.Libs.RavenDB.Model
{
    public class Note
    {
        public string Id { get; set; }
        public string PlayerId { get; set; }
        public string Text { get; set; }
        public DateTime Date { get; set; }
    }
}