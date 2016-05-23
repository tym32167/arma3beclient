using System;
using Arma3BE.Server.Models;
using Arma3BEClient.Common.Logging;
using Arma3BEClient.Libs.Repositories;

namespace Arma3BE.Client.Modules.MainModule.Helpers
{
    public class ChatHelper
    {
        private readonly Guid _currentServerId;
        private readonly ILog _log;

        public ChatHelper(ILog log, Guid currentServerId)
        {
            _log = log;
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