using System;

namespace Arma3BEClient.Libs.ModelCompact
{
    public class Reason
    {
        public string Text { get; set; }
    }

    public class BanTime
    {
        public string Title { get; set; }
        public TimeSpan Time { get; set; }
    }
}