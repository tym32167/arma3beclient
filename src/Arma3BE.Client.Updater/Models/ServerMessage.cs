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

        private readonly string _message;
        private readonly int _messageId;

        public ServerMessage(int messageId, string message)
        {
            _messageId = messageId;
            _message = message;
        }

        public int MessageId
        {
            get { return _messageId; }
        }

        public string Message
        {
            get { return _message; }
        }

        public MessageType Type
        {
            get
            {
                if (_message.Contains("Players on server:")) return MessageType.PlayerList;
                if (_message.StartsWith("GUID Bans:") || _message.StartsWith("IP Bans:")) return MessageType.BanList;
                if (_message.StartsWith("Connected RCon admins:")) return MessageType.AdminList;

                if (_message.StartsWith("Missions on server:")) return MessageType.MissionList;


                if (_message.StartsWith("(Side)") || _message.StartsWith("(Vehicle)") ||
                    (_message.StartsWith("(Global)") || _message.StartsWith("(Group)")) ||
                    (_message.StartsWith("(Command)") || _message.StartsWith("(Direct)")))
                    return MessageType.ChatMessage;
                if (_message.StartsWith("RCon") && !_message.EndsWith("logged in"))
                    return MessageType.ChatMessage;

                if (_message.StartsWith("RCon admin") && _message.EndsWith("logged in"))
                    return MessageType.RconAdminLog;

                if (_message.StartsWith("Player") && (
                    //_message.Contains("connected")
                    //||
                    _message.Contains("disconnected")
                    ||
                    _message.Contains("is losing connection")
                    ))

                    return MessageType.PlayerLog;


                if (_message.StartsWith("Verified GUID"))
                    return MessageType.PlayerLog;


                if (_message.StartsWith("Player") && (
                    _message.Contains("kicked")
                    ))

                    return MessageType.BanLog;


                return MessageType.Unknown;
            }
        }
    }
}