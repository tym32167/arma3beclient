using Arma3BEClient.Libs.Core.Model;
using Arma3BEClient.Libs.EF.Repositories;
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