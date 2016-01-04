using System;
using System.Collections.Generic;
using System.Linq;

namespace Arma3BE.Server.Models
{
    public class PlayerList : StateList<Player>
    {
        public PlayerList(ServerMessage message)
            : base(Parse(message.Message))
        {
        }

        private static IEnumerable<Player> Parse(string text)
        {
            var lines = text.Split(Environment.NewLine.ToCharArray()).ToArray();
            return lines.Select(Player.Parse).Where(p => p != null).ToList();
        }
    }
}