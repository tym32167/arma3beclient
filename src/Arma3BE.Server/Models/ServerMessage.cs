using Arma3BE.Server.Abstract;
using Arma3BE.Server.Messaging;
using Arma3BE.Server.Recognizers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Arma3BE.Server.Models
{
    public class ServerMessage
    {
        private static readonly IEnumerable<IServerMessageRecognizer> Recognizers = new IServerMessageRecognizer[]
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

            _type = new Lazy<ServerMessageType>(GetMessageType);
        }

        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        // ReSharper disable once MemberCanBePrivate.Global
        public int MessageId { get; }

        public string Message { get; }

        private readonly Lazy<ServerMessageType> _type;

        public ServerMessageType Type => _type.Value;

        private ServerMessageType GetMessageType()
        {
            var matches = Recognizers.Where(x => x.CanRecognize(this)).ToArray();
            if (matches.Length == 1) return matches[0].GetMessageType(this);
            return ServerMessageType.Unknown;
        }

    }
}