using Arma3BE.Server.Models;
using System;

namespace Arma3BE.Client.Infrastructure.Events.BE
{
    public class BEChatMessage : BEMessageBase<ChatMessage>
    {
        public ChatMessage Message { get; }

        public BEChatMessage(ChatMessage message, Guid serverId) : base(serverId)
        {
            Message = message;
        }
    }
}