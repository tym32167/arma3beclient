using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Arma3BEClient.Libs.EF.Model
{
    // ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
    public class Admin
    {
        [Key]
        // ReSharper disable once UnusedMember.Global
        public int Id { get; set; }

        public int Num { get; set; }
        public string IP { get; set; }
        public int Port { get; set; }
        public Guid ServerId { get; set; }

        [ForeignKey("ServerId")]
        // ReSharper disable once UnusedMember.Global
        public virtual ServerInfo ServerInfo { get; set; }
    }
}