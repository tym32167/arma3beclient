using System.ComponentModel.DataAnnotations;

namespace Arma3BEClient.Libs.ModelCompact
{
    public class BanReason
    {
        [Key]
        public int Id { get; set; }
        public string Text { get; set; }
    }

    public class KickReason
    {
        [Key]
        public int Id { get; set; }
        public string Text { get; set; }
    }

    public class BanTime
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public int TimeInMinutes { get; set; }
    }
}