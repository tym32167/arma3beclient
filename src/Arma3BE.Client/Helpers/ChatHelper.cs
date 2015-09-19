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
            if (message.Type != ChatMessage.MessageType.Unknown)
            {
                using (var context = new Arma3BeClientContext())
                {
                    context.ChatLog.Add(new ChatLog
                    {
                        Date = message.Date,
                        ServerId = _currentServerId,
                        Text = message.Message
                    });

                    context.SaveChangesAsync();
                }
            }

            return true;
        }
    }
}