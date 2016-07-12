using System;
using Arma3BE.Server.Models;

namespace Arma3BE.Client.Infrastructure.Events.BE
{
    public class BEBanLogMessage : BELogMessage
    {
        public BEBanLogMessage(LogMessage message, Guid serverId) : base(message, serverId)
        {
        }
    }
}