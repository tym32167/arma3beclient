using Prism.Events;
using System;
using System.Collections.Generic;

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

    public class BEItemsMessage<T> : BEMessageBase<IEnumerable<T>>
    {
        public BEItemsMessage(IEnumerable<T> items, Guid serverId) : base(serverId)
        {
            Items = items;
        }

        public IEnumerable<T> Items { get; }
    }

    public class BEMessageEvent<T> : PubSubEvent<T>
    {

    }
}