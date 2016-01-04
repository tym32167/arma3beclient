using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Linq.Expressions;
using Arma3BEClient.Common.Core;
using Arma3BEClient.Libs.Context;
using Arma3BEClient.Libs.ModelCompact;

namespace Arma3BEClient.Libs.Repositories
{
    public class PlayerRepository : DisposeObject
    {
        public IEnumerable<PlayerDto> GetAllPlayers()
        {
            using (var dc = new Arma3BeClientContext())
            {
                return dc.Player.ToList();
            }
        }

        public void AddPlayers(IEnumerable<PlayerDto> players)
        {
            var playerList = players.Select(Map).ToArray();

            using (var dc = new Arma3BeClientContext())
            {
                dc.Player.AddRange(playerList);
                dc.SaveChanges();
            }
        }

        public IEnumerable<PlayerDto> GetPlayers(Expression<Func<Player, bool>> expression)
        {
            using (var dc = new Arma3BeClientContext())
            {
                return dc.Player.Where(expression).ToList();
            }
        }

        public IEnumerable<PlayerDto> GetPlayers(IEnumerable<string> guids)
        {
            using (var dc = new Arma3BeClientContext())
            {
                return dc.Player.Where(x => guids.Contains(x.GUID)).ToList();
            }
        }

        public PlayerDto GetPlayer(string guid)
        {
            using (var dc = new Arma3BeClientContext())
            {
                return dc.Player.FirstOrDefault(x => x.GUID == guid);
            }
        }

        public Player GetPlayerInfo(string guid)
        {
            using (var dc = new Arma3BeClientContext())
            {
                return
                    dc.Player.Where(x => x.GUID == guid)
                        .Include(x => x.Bans)
                        .Include(x => x.Bans.Select(b => b.ServerInfo))
                        .Include(x => x.Notes)
                        .Include(x => x.PlayerHistory)
                        .FirstOrDefault();
            }
        }

        public void UpdatePlayerComment(string guid, string comment)
        {
            using (var dc = new Arma3BeClientContext())
            {
                var dbp = dc.Player.FirstOrDefault(x => x.GUID == guid);
                if (dbp != null)
                {
                    dbp.Comment = comment;
                    dc.SaveChanges();
                }
            }
        }

        public void UpdateCommant(Dictionary<Guid, string> playersToUpdateComments)
        {
            var ids = playersToUpdateComments.Keys.ToArray();

            using (var dc = new Arma3BeClientContext())
            {
                var players = dc.Player.Where(x => ids.Contains(x.Id));
                foreach (var player in players)
                {
                    player.Comment = playersToUpdateComments[player.Id];
                }
                dc.SaveChanges();
            }
        }

        public void AddOrUpdate(IEnumerable<PlayerDto> players)
        {
            var playerList = players.Select(Map).ToArray();

            using (var dc = new Arma3BeClientContext())
            {
                dc.Player.AddOrUpdate(playerList);
                dc.SaveChanges();
            }
        }

        public void AddHistory(List<PlayerHistory> histories)
        {
            using (var dc = new Arma3BeClientContext())
            {
                dc.PlayerHistory.AddOrUpdate(histories.ToArray());
                dc.SaveChanges();
            }
        }

        public void AddNotes(Guid id, string s)
        {
            using (var dc = new Arma3BeClientContext())
            {
                dc.Comments.Add(new Note()
                {
                    PlayerId = id,
                    Text = s,
                });
                dc.SaveChanges();
            }
        }


        private Player Map(PlayerDto source)
        {
            return new Player()
            {
                Id = source.Id,
                Comment = source.Comment,
                GUID = source.GUID,
                LastIp = source.LastIp,
                LastSeen = source.LastSeen,
                Name = source.Name
            };
        }
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
}