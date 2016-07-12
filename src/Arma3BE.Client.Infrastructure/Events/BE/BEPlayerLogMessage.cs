using System;
using Arma3BE.Server.Models;

namespace Arma3BE.Client.Infrastructure.Events.BE
{
    public class BEPlayerLogMessage : BELogMessage
    {
        public BEPlayerLogMessage(LogMessage message, Guid serverId) : base(message, serverId)
        {
        }
    }
}