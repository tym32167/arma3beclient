using System;
using System.Collections.Generic;
using System.Linq;

namespace Arma3BEClient.Updater.Models
{
    public class MissionList : StateList<Mission>
    {
        public MissionList(ServerMessage message)
            : base(Parse(message.Message))
        {
        }

        private static IEnumerable<Mission> Parse(string text)
        {
            var lines = text.Split(Environment.NewLine.ToCharArray()).ToArray();
            return lines.Select(Mission.Parse).Where(p => p != null).ToList();
        }
    }
}