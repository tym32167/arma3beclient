using System;
using Arma3BE.Server.Messaging.Recognizers.Core;
using Arma3BE.Server.Models;

namespace Arma3BE.Server.Messaging.Recognizers
{
    public class PlayerListRecognizer : IServerMessageRecognizer
    {
        public ServerMessageType GetMessageType(ServerMessage message)
        {
            return ServerMessageType.PlayerList;
        }

        public bool CanRecognize(ServerMessage serverMessage)
        {
            var firstLines = new[]
            {
                "Players on server:",
                "[#] [IP Address]:[Port] [Ping] [GUID] [Name]",
                "--------------------------------------------------"
            };


            var lines = serverMessage.Message.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            if (lines.Length <= firstLines.Length) return false;

            var i = 0;
            for (; i < firstLines.Length; i++)
            {
                if (string.Compare(firstLines[i], lines[i], StringComparison.InvariantCultureIgnoreCase) != 0)
                    return false;
            }

            for (; i < (lines.Length - 1); i++)
            {
                if (Player.Parse(lines[i]) == null || !CanRecognizeLine(lines[i]))
                {
                    return false;
                }
            }


            var lastLine = $"({lines.Length - 4} players in total)";
            if (string.Compare(lastLine, lines[lines.Length - 1], StringComparison.InvariantCultureIgnoreCase) != 0)
                return false;


            return true;
        }

        public bool CanRecognizeLine(string line)
        {
            var lines = line.Split(" \t".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            if (lines.Length < 5) return false;

            int test;
            if (!Int32.TryParse(lines[0], out test)) return false;

            if (!IPAndPortValidator.Validate(lines[1])) return false;

            if (!Int32.TryParse(lines[2], out test)) return false;

            if (!GUIDValidator.Validate(lines[3].Replace("(OK)", string.Empty))) return false;

            return true;
        }
    }
}