using System;
using Arma3BE.Server.Models;

namespace Arma3BE.Client.Infrastructure.Events.BE
{
    public abstract class BELogMessage : BEMessageBase<LogMessage>
    {
        public LogMessage Message { get; }

        public BELogMessage(LogMessage message, Guid serverId) : base(serverId)
        {
            Message = message;
        }
    }
}