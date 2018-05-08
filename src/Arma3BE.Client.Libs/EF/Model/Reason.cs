using System.ComponentModel.DataAnnotations;

// ReSharper disable UnusedMember.Global

namespace Arma3BEClient.Libs.EF.Model
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



    public class BadNickname
    {
        [Key]
        public int Id { get; set; }
        public string Text { get; set; }
    }

    public class ImportantWord
    {
        [Key]
        public int Id { get; set; }
        public string Text { get; set; }
    }
}