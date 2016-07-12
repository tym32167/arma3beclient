using BattleNET;

namespace Arma3BE.Server.Abstract
{
    public interface IBattlEyeServerFactory
    {
        IBattlEyeServer Create(BattlEyeLoginCredentials credentials);
    }
}