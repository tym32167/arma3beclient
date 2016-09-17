using System;
using Arma3BE.Server;

namespace Arma3BE.Client.Infrastructure.Events.BE
{
    public class BECommand : BEMessageBase<object>
    {
        public BECommand(Guid serverId, CommandType commandType, string parameters = null) : base(serverId)
        {
            Parameters = parameters;
            CommandType = commandType;
        }

        public string Parameters { get; }
        public CommandType CommandType { get; }
    }
}