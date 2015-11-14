using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arma3BEClient.Libs.ModelCompact
{
    public class Admin
    {
        [Key]
        public int Id { get; set; }

        public int Num { get; set; }
        public string IP { get; set; }
        public int Port { get; set; }
        public Guid ServerId { get; set; }

        [ForeignKey("ServerId")]
        public virtual ServerInfo ServerInfo { get; set; }
    }
}