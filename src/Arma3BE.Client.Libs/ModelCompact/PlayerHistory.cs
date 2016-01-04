using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Arma3BEClient.Common.Attributes;

namespace Arma3BEClient.Libs.ModelCompact
{
    public class PlayerHistory
    {
        public PlayerHistory()
        {
            Date = DateTime.UtcNow;
        }

        [Key]
        public int Id { get; set; }

        [Required]
        public Guid PlayerId { get; set; }

        [ShowInUi]
        [EnableCopy]
        public string Name { get; set; }

        [ShowInUi]
        [EnableCopy]
        public string IP { get; set; }

        [ShowInUi]
        [EnableCopy]
        public DateTime Date { get; set; }

        public Guid ServerId { get; set; }

        [ForeignKey("PlayerId")]
        public virtual Player Player { get; set; }

        [ForeignKey("ServerId")]
        public virtual ServerInfo ServerInfo { get; set; }
    }
}