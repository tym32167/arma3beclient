using System;
using Arma3BE.Server.Messaging.Recognizers.Core;
using Arma3BE.Server.Models;

namespace Arma3BE.Server.Messaging.Recognizers
{
    public class BanListRecognizer : IServerMessageRecognizer
    {
        public ServerMessageType GetMessageType(ServerMessage message)
        {
            return ServerMessageType.BanList;
        }

        public bool CanRecognize(ServerMessage serverMessage)
        {
            var firstLines = new[]
            {
                "GUID Bans:",
                "[#] [GUID] [Minutes left] [Reason]",
                "----------------------------------------"
            };

            var lines = serverMessage.Message.Split(Environment.NewLine.ToCharArray(),
                StringSplitOptions.RemoveEmptyEntries);

            if (lines.Length <= firstLines.Length) return false;

            var i = 0;
            for (; i < firstLines.Length; i++)
            {
                if (string.Compare(firstLines[i], lines[i], StringComparison.InvariantCultureIgnoreCase) != 0)
                    return false;
            }

            for (; i < (lines.Length); i++)
            {
                if (string.Compare("IP Bans:", lines[i], StringComparison.InvariantCultureIgnoreCase) == 0
                    || string.IsNullOrEmpty(lines[i])) break;

                if (Ban.Parse(lines[i]) == null || !CanRecognizeLine(lines[i]))
                {
                    return false;
                }
            }

            return true;
        }

        public bool CanRecognizeLine(string line)
        {
            var lines = line.Split(" \t".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            if (lines.Length < 4) return false;

            int test;
            if (!int.TryParse(lines[0], out test)) return false;

            if (!GUIDValidator.Validate(lines[1])) return false;

            return true;
        }
    }


//GUID Bans:
//[#] [GUID] [Minutes left] [Reason]
//----------------------------------------
//0  GUID perm TeamKill.Ban perm
//1  GUID perm TeamKill.Ban perm
//2  GUID perm TeamKill.Ban perm
//3  GUID perm TeamKill.Ban perm
//4  GUID perm Cheater.Ban perm!
//5  GUID perm Cheater(Ban PERM) 
//219 GUID perm [tim][15.09.14 12:25:59] Teamkill
//220 GUID perm [tim][15.09.14 12:27:46] Teamkill
//221 GUID perm [tim][15.09.14 12:33:07] Teamkill
//222 GUID perm [[SO]vosur][17.09.14 17:55:36] Teamkill
//364 GUID perm [[SO]vosur][04.11.14 18:21:03] Sabotage, VehicleDestroy
//508 GUID 8971 [Rembowest14][18.07.15 13:38:14] Sabotage

//IP Bans:
//[#] [IP Address] [Minutes left] [Reason]
//----------------------------------------------
//365 212.90.39.93    perm Admin Ban TK ���??���� �� �����
}