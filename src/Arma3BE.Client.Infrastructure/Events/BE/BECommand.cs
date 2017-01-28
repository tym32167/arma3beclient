using Arma3BE.Server;
using System;

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

    public class BECustomCommand : BEMessageBase<object>
    {
        public string Command { get; }

        public BECustomCommand(Guid serverId, string command) : base(serverId)
        {
            Command = command;
        }
    }
}