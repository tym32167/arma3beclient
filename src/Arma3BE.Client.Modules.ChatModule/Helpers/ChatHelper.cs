using Arma3BE.Server.Models;
using Arma3BEClient.Libs.Repositories;
using System;

namespace Arma3BE.Client.Modules.ChatModule.Helpers
{
    public class ChatHelper
    {
        private readonly Guid _currentServerId;

        public ChatHelper(Guid currentServerId)
        {
            _currentServerId = currentServerId;
        }

        public bool RegisterChatMessage(ChatMessage message)
        {
            if (message.Type != ChatMessage.MessageType.Unknown)
                using (var repo = new ChatRepository())
                {
                    repo.AddOrUpdate(message, _currentServerId);
                }

            return true;
        }
    }
}