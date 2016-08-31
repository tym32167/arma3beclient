using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using Arma3BEClient.Common.Attributes;
using Arma3BEClient.Libs.Tools;

namespace Arma3BEClient.Libs.Context
{
    public class Arma3BeClientContextObsolete : DbContext
    {
        static Arma3BeClientContextObsolete()
        {
            Database.SetInitializer(new DbInitializer());
            using (var db = new Arma3BeClientContextObsolete())
                db.Database.Initialize(false);
        }

        public Arma3BeClientContextObsolete() : base(new ConnectionFactory().Create(), true)
        {
        }

        public DbSet<Dtos.ChatLog> ChatLog { get; set; }
        public DbSet<Dtos.Note> Comments { get; set; }
        public DbSet<Dtos.Player> Player { get; set; }
        public DbSet<Dtos.ServerInfo> ServerInfo { get; set; }
        public DbSet<Dtos.Settings> Settings { get; set; }
        public DbSet<Dtos.Ban> Bans { get; set; }
        public DbSet<Dtos.Admin> Admins { get; set; }
        public DbSet<Dtos.PlayerHistory> PlayerHistory { get; set; }


        internal class DbInitializer : CreateDatabaseIfNotExists<Arma3BeClientContext>
        {
        }

        internal class ObsoletteRepository
        {
            public void EnsureDatabase()
            {
                using (var dc = new Arma3BeClientContextObsolete())
                {
                    dc.Database.CreateIfNotExists();
                }
            }
        }
    }

    public class Dtos
    {
        public class ChatLog
        {
            [Key]
            public int Id { get; set; }

            public string Text { get; set; }
            public Guid ServerId { get; set; }
            public DateTime Date { get; set; }

            [ForeignKey("ServerId")]
            public virtual ServerInfo ServerInfo { get; set; }
        }

        public class Note
        {
            public Note()
            {
                Id = Guid.NewGuid();
                Date = DateTime.UtcNow;
            }

            [Key]
            public Guid Id { get; set; }

            public Guid PlayerId { get; set; }

            [ShowInUi]
            [EnableCopy]
            public string Text { get; set; }

            [ShowInUi]
            [EnableCopy]
            public DateTime Date { get; set; }

            [ForeignKey("PlayerId")]
            public virtual Player Player { get; set; }
        }


        public class PlayerDto
        {
            public PlayerDto()
            {
                LastSeen = DateTime.UtcNow;
            }

            [Key]
            public Guid Id { get; set; }

            public string GUID { get; set; }
            public string Name { get; set; }
            public string Comment { get; set; }

            public string LastIp { get; set; }
            public DateTime LastSeen { get; set; }
        }

        public class Player : PlayerDto
        {
            public Player()
            {
                Notes = new HashSet<Note>();
                PlayerHistory = new HashSet<PlayerHistory>();
                // this.Sessions = new HashSet<Sessions>();
            }

            public virtual ICollection<Note> Notes { get; set; }
            public virtual ICollection<Ban> Bans { get; set; }
            public virtual ICollection<PlayerHistory> PlayerHistory { get; set; }
        }

        public class ServerInfo
        {
            public ServerInfo()
            {
                ChatLog = new HashSet<ChatLog>();
                PlayerHistory = new HashSet<PlayerHistory>();
                Admins = new HashSet<Admin>();
            }

            [Key]
            public Guid Id { get; set; }

            [Required]
            public string Host { get; set; }

            [Required]
            public int Port { get; set; }

            [Required]
            public string Password { get; set; }

            [Required]
            public string Name { get; set; }

            [Required]
            public bool Active { get; set; }


            public virtual ICollection<ChatLog> ChatLog { get; set; }

            public virtual ICollection<Ban> Bans { get; set; }

            public virtual ICollection<Admin> Admins { get; set; }

            public virtual ICollection<PlayerHistory> PlayerHistory { get; set; }
        }


        public class Settings
        {
            //[Key]
            public int Id { get; set; }
            public string Value { get; set; }
        }

        public class Ban
        {
            public Ban()
            {
                CreateDate = DateTime.UtcNow;
                IsActive = true;
            }

            [Key]
            public int Id { get; set; }

            public Guid? PlayerId { get; set; }
            public int Num { get; set; }
            public Guid ServerId { get; set; }
            public string GuidIp { get; set; }
            public int Minutes { get; set; }

            [ShowInUi]
            [EnableCopy]
            public int MinutesLeft { get; set; }

            [ShowInUi]
            [EnableCopy]
            public string Reason { get; set; }

            public DateTime CreateDate { get; set; }
            public bool IsActive { get; set; }

            [ForeignKey("PlayerId")]
            public virtual Player Player { get; set; }

            [ForeignKey("ServerId")]
            public virtual ServerInfo ServerInfo { get; set; }
        }

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
}