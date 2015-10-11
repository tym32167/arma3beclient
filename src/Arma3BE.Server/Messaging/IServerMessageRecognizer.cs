using Arma3BE.Server.Models;

namespace Arma3BE.Server.Messaging
{
    public interface IServerMessageRecognizer
    {
        ServerMessageType GetMessageType(ServerMessage message);
        bool CanRecognize(ServerMessage serverMessage);
    }
}

