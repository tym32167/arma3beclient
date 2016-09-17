using System.Collections.Generic;
using System.Linq;
using Arma3BE.Server.Abstract;
using Arma3BE.Server.Messaging;
using Arma3BE.Server.Recognizers;

namespace Arma3BE.Server.Models
{
    public class ServerMessage
    {
        private static IEnumerable<IServerMessageRecognizer> _recognizers = new IServerMessageRecognizer[]
        {
            new AdminListRecognizer(), 
            new BanListRecognizer(), 
            new BanLogRecognizer(), 
            new ChatMessageRecognizer(), 
            new MissionsListRecognizer(), 
            new PlayerListRecognizer(), 
            new PlayerLogRecognizer(), 
            new RconAdminLogRecognizer(), 
        };

        public ServerMessage(int messageId, string message)
        {
            MessageId = messageId;
            Message = message;
        }

        public int MessageId { get; }

        public string Message { get; }

        public ServerMessageType Type
        {
            get
            {
                var matches = _recognizers.Where(x => x.CanRecognize(this)).ToArray();
                if (matches.Length == 1) return matches[0].GetMessageType(this);
                return ServerMessageType.Unknown;
            }
        }
    }
}