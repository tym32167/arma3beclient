using System;
using Arma3BE.Server.Abstract;
using Arma3BE.Server.Messaging;
using Arma3BE.Server.Models;
using Arma3BE.Server.Recognizers.Core;

namespace Arma3BE.Server.Recognizers
{
    public class AdminListRecognizer : IServerMessageRecognizer
    {
        public ServerMessageType GetMessageType(ServerMessage message)
        {
            return ServerMessageType.AdminList;
        }

        
        public bool CanRecognize(ServerMessage serverMessage)
        {
            var firstLines = new[]
            {
                "Connected RCon admins:",
                "[#] [IP Address]:[Port]",
                "-----------------------------"
            };


            var lines = serverMessage.Message.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            if (lines.Length <= firstLines.Length) return false;

            var i = 0;
            for (; i < firstLines.Length; i++)
            {
                if (string.Compare(firstLines[i], lines[i], StringComparison.InvariantCultureIgnoreCase) != 0)
                    return false;
            }

            for (; i < (lines.Length); i++)
            {
                if (Admin.Parse(lines[i]) == null || !CanRecognizeLine(lines[i]))
                {
                    return false;
                }
            }

            return true;
        }

        public bool CanRecognizeLine(string line)
        {
            var lines = line.Split(" \t".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            if (lines.Length < 2) return false;

            int test;
            if (!Int32.TryParse(lines[0], out test)) return false;

            if (!IPAndPortValidator.Validate(lines[1])) return false;

            return true;
        }
    }

//Sample
//
//Connected RCon admins:
//[#] [IP Address]:[Port]
//-----------------------------
//0 94.181.44.100:50960
//1 213.21.12.135:54678

}