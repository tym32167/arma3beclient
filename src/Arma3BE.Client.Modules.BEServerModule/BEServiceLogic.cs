using Arma3BE.Client.Infrastructure.Events.BE;
using Arma3BE.Server;
using Prism.Events;

namespace Arma3BE.Client.Modules.BEServerModule
{
    public class BEServiceLogic
    {
        private readonly IEventAggregator _aggregator;

        public BEServiceLogic(IEventAggregator aggregator)
        {
            _aggregator = aggregator;
            _aggregator.GetEvent<BEMessageEvent<BEPlayerLogMessage>>()
                .Subscribe(PlayerLogMessage);
        }

        private void PlayerLogMessage(BEPlayerLogMessage message)
        {
            _aggregator.GetEvent<BEMessageEvent<BECommand>>()
                   .Publish(new BECommand(message.ServerId, CommandType.Players));
        }
    }
}