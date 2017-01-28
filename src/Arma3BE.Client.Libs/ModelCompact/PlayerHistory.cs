using Arma3BEClient.Common.Attributes;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Arma3BEClient.Libs.ModelCompact
{
    // ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
    public class PlayerHistory
    {
        public PlayerHistory()
        {
            Date = DateTime.UtcNow;
        }

        [Key]
        // ReSharper disable once UnusedMember.Global
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
        // ReSharper disable once MemberCanBePrivate.Global
        public DateTime Date { get; set; }

        public Guid ServerId { get; set; }

        [ForeignKey("PlayerId")]
        // ReSharper disable once UnusedMember.Global
        public virtual Player Player { get; set; }

        [ForeignKey("ServerId")]
        // ReSharper disable once UnusedMember.Global
        public virtual ServerInfo ServerInfo { get; set; }
    }
}