using System;
using System.Collections.Generic;
using System.Linq;

namespace Arma3BE.Server.Models
{
    public class BanList : StateList<Ban>
    {
        public BanList(ServerMessage message)
            : base(Parse(message.Message))
        {
        }

        private static IEnumerable<Ban> Parse(string text)
        {
            var lines = text.Split(Environment.NewLine.ToCharArray()).ToArray();
            return lines.Select(Ban.Parse).Where(p => p != null).ToList();
        }
    }
}