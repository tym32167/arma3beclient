using System;
using Arma3BE.Server.Models;

namespace Arma3BE.Client.Infrastructure.Events.BE
{
    public class BEAdminLogMessage : BELogMessage
    {
        public BEAdminLogMessage(LogMessage message, Guid serverId) : base(message, serverId)
        {
        }
    }
}