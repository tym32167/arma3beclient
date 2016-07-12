using System;
using System.Collections.Generic;

namespace Arma3BE.Client.Infrastructure.Events.BE
{
    public class BEItemsMessage<T> : BEMessageBase<IEnumerable<T>>
    {
        public BEItemsMessage(IEnumerable<T> items, Guid serverId) : base(serverId)
        {
            Items = items;
        }

        public IEnumerable<T> Items { get; }
    }
}