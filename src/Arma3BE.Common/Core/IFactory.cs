namespace Arma3BEClient.Common.Core
{
    public interface IFactory<T>
    {
        T Create();
    }
}