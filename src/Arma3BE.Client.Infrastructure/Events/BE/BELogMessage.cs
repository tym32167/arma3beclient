using Arma3BE.Server.Models;
using System;

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