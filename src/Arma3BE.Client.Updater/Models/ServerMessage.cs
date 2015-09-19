namespace Arma3BEClient.Updater.Models
{
    public class ServerMessage
    {
        public enum MessageType
        {
            PlayerList,
            BanList,
            AdminList,
            ChatMessage,
            RconAdminLog,
            PlayerLog,
            BanLog,
            MissionList,
            Unknown
        }

        public ServerMessage(int messageId, string message)
        {
            MessageId = messageId;
            Message = message;
        }

        public int MessageId { get; }

        public string Message { get; }

        public MessageType Type
        {
            get
            {
                if (Message.Contains("Players on server:")) return MessageType.PlayerList;
                if (Message.StartsWith("GUID Bans:") || Message.StartsWith("IP Bans:")) return MessageType.BanList;
                if (Message.StartsWith("Connected RCon admins:")) return MessageType.AdminList;

                if (Message.StartsWith("Missions on server:")) return MessageType.MissionList;


                if (Message.StartsWith("(Side)") || Message.StartsWith("(Vehicle)") ||
                    (Message.StartsWith("(Global)") || Message.StartsWith("(Group)")) ||
                    (Message.StartsWith("(Command)") || Message.StartsWith("(Direct)")))
                    return MessageType.ChatMessage;
                if (Message.StartsWith("RCon") && !Message.EndsWith("logged in"))
                    return MessageType.ChatMessage;

                if (Message.StartsWith("RCon admin") && Message.EndsWith("logged in"))
                    return MessageType.RconAdminLog;

                if (Message.StartsWith("Player") && (
                    //_message.Contains("connected")
                    //||
                    Message.Contains("disconnected")
                    ||
                    Message.Contains("is losing connection")
                    ))

                    return MessageType.PlayerLog;


                if (Message.StartsWith("Verified GUID"))
                    return MessageType.PlayerLog;


                if (Message.StartsWith("Player") && (
                    Message.Contains("kicked")
                    ))

                    return MessageType.BanLog;


                return MessageType.Unknown;
            }
        }
    }
}