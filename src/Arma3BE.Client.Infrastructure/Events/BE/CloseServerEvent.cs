using Arma3BEClient.Libs.Repositories;
using Prism.Events;

namespace Arma3BE.Client.Infrastructure.Events.BE
{
    public class CloseServerEvent : PubSubEvent<ServerInfoDto>
    {

    }


    public class ConnectServerEvent : PubSubEvent<ServerInfoDto>
    {

    }

    public class DisConnectServerEvent : PubSubEvent<ServerInfoDto>
    {

    }

    public class ConnectingServerEvent : PubSubEvent<ServerInfoDto>
    {

    }
}