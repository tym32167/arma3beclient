using System;
using System.Collections.Generic;
using System.Linq;

namespace Arma3BE.Server.Models
{
    public class AdminList : StateList<Admin>
    {
        public AdminList(ServerMessage message)
            : base(Parse(message.Message))
        {
        }

        private static IEnumerable<Admin> Parse(string text)
        {
            var lines = text.Split(Environment.NewLine.ToCharArray()).ToArray();
            return lines.Select(Admin.Parse).Where(p => p != null).ToList();
        }
    }
}