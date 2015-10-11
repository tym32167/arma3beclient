using Arma3BE.Server.Models;

namespace Arma3BE.Server.Messaging.Recognizers
{
    public class PlayerListRecognizer : IServerMessageRecognizer
    {
        public ServerMessageType GetMessageType(ServerMessage message)
        {
            return ServerMessageType.PlayerList;
        }

        public bool CanRecognize(ServerMessage serverMessage)
        {
            return serverMessage.Message.Contains("Players on server:");
        }
    }
}