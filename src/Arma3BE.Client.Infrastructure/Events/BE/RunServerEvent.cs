using Arma3BEClient.Libs.EF.Repositories;
using Prism.Events;

namespace Arma3BE.Client.Infrastructure.Events.BE
{
    public class RunServerEvent : PubSubEvent<ServerInfoDto>
    {

    }
}