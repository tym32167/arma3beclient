using System.Threading.Tasks;

namespace Arma3BE.Client.Infrastructure
{
    public interface IIpService
    {
        string GetIpAddress(string host);

        Task<string> Get(string ip);
        GeoInfo GetGeoInfoLocal(string ip);
    }

    public class GeoInfo
    {
        public GeoInfo(string country, string city)
        {
            City = city;
            Country = country;
        }

        public string City { get; }
        public string Country { get; }
    }
}