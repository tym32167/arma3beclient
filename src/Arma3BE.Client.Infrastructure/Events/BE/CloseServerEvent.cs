using Arma3BEClient.Libs.ModelCompact;
using Prism.Events;

namespace Arma3BE.Client.Infrastructure.Events.BE
{
    public class CloseServerEvent : PubSubEvent<ServerInfo>
    {

    }


    public class ConnectServerEvent : PubSubEvent<ServerInfo>
    {

    }

    public class DisConnectServerEvent : PubSubEvent<ServerInfo>
    {

    }

    public class ConnectingServerEvent : PubSubEvent<ServerInfo>
    {

    }
}