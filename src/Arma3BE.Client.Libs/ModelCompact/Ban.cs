using Arma3BEClient.Common.Attributes;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable UnusedMember.Global

namespace Arma3BEClient.Libs.ModelCompact
{
    // ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
    public class Ban
    {
        public Ban()
        {
            CreateDate = DateTime.UtcNow;
            IsActive = true;
        }

        [Key]
        public int Id { get; set; }

        [Index]
        public Guid? PlayerId { get; set; }
        public int Num { get; set; }

        [Index]
        public Guid ServerId { get; set; }

        [Index]
        public string GuidIp { get; set; }

        [Index]
        public int Minutes { get; set; }

        [ShowInUi]
        [EnableCopy]
        [Index]
        public int MinutesLeft { get; set; }

        [ShowInUi]
        [EnableCopy]
        public string Reason { get; set; }

        [Index]
        public DateTime CreateDate { get; set; }

        [Index]
        public bool IsActive { get; set; }

        [ForeignKey("PlayerId")]
        public virtual Player Player { get; set; }

        [ForeignKey("ServerId")]
        public virtual ServerInfo ServerInfo { get; set; }
    }
}