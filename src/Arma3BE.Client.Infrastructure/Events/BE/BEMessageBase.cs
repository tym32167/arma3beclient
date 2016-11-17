using System;

namespace Arma3BE.Client.Infrastructure.Events.BE
{
    public abstract class BEMessageBase<T> : BEMessageBase
    {
        protected BEMessageBase(Guid serverId) : base(serverId)
        {
        }
    }

    public abstract class BEMessageBase
    {
        protected BEMessageBase(Guid serverId)
        {
            ServerId = serverId;
        }

        public Guid ServerId { get; }
    }

    public class BEMessage : BEMessageBase
    {
        public BEMessage(Guid serverId) : base(serverId)
        {
        }
    }
}