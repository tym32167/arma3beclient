namespace Arma3BE.Client.Modules.SteamModule.Core
{
    public interface IMd5Provider
    {
        uint ComputeUIntHash(uint input);
        byte[] ComputeByteHash(uint input);
        byte[] ComputeByteHash(long input);
    }


    public interface IMd5ProviderFactory
    {
        IMd5Provider Create();
    }
}