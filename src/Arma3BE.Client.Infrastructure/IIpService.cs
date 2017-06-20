using System.Threading.Tasks;

namespace Arma3BE.Client.Infrastructure
{
    public interface IIpService
    {
        string GetIpAddress(string host);
        Task<string> Get(string ip);
        string GetCountryLocal(string ip);
        string GetCityLocal(string ip);
    }
}