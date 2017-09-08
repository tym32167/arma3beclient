using Arma3BEClient.Libs.ModelCompact;
using Arma3BEClient.Libs.Repositories;
using Prism.Events;

namespace Arma3BE.Client.Infrastructure.Events.BE
{
    public class RunServerEvent : PubSubEvent<ServerInfoDto>
    {

    }
}