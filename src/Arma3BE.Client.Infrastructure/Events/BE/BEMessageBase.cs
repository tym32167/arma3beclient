using System;

namespace Arma3BE.Client.Infrastructure.Events.BE
{
    public abstract class BEMessageBase<T>
    {
        public BEMessageBase(Guid serverId)
        {
            ServerId = serverId;
        }

        public Guid ServerId { get; }
    }
}