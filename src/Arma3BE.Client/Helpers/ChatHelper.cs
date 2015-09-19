using System;
using Arma3BE.Server.Models;
using Arma3BEClient.Common.Logging;
using Arma3BEClient.Libs.Context;
using Arma3BEClient.Libs.ModelCompact;

namespace Arma3BEClient.Helpers
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
            using (var repo = new Arma3BERepository())
            {
                repo.AddOrUpdate(message, _currentServerId);
            }

            return true;
        }
    }
}