using Arma3BE.Server.Models;
using Arma3BEClient.Libs.Core;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Threading.Tasks;

namespace Arma3BE.Client.Modules.ChatModule.Helpers
{
    public class ChatHelper
    {
        private readonly Guid _currentServerId;

        public ChatHelper(Guid currentServerId)
        {
            _currentServerId = currentServerId;
        }

        public async Task<bool> RegisterChatMessageAsync(ChatMessage message)
        {
            if (message.Type != ChatMessage.MessageType.Unknown)
                using (var repo = ServiceLocator.Current.TryResolve<IChatRepository>())
                {
                    await repo.AddOrUpdateAsync(message, _currentServerId);
                }

            return true;
        }
    }
}