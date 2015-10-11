using Arma3BE.Server.Models;

namespace Arma3BE.Server.Messaging.Recognizers
{
    public class BanListRecognizer : IServerMessageRecognizer
    {
        public ServerMessageType GetMessageType(ServerMessage message)
        {
            return ServerMessageType.BanList;
        }

        public bool CanRecognize(ServerMessage serverMessage)
        {
            return serverMessage.Message.StartsWith("GUID Bans:") || serverMessage.Message.StartsWith("IP Bans:");
        }
    }
}