using Arma3BE.Client.Infrastructure;
using MaxMind.GeoIP2;
using System;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Arma3BE.Client.Modules.NetModule
{
    public class IpService : IIpService
    {
        public string GetIpAddress(string host)
        {
            IPAddress ip;

            if (IPAddress.TryParse(host, out ip))
            {
                return ip.ToString();
            }
            try
            {
                var entry = Dns.GetHostEntry(host);
                return entry.AddressList[0].ToString();
            }
            catch
            {
                return string.Empty;
            }
        }

        public async Task<string> Get(string ip)
        {
            if (string.IsNullOrEmpty(ip)) return string.Empty;
            using (var c = new HttpClient())
            {
                var pattern = ConfigurationManager.AppSettings["IPServicePattern"];
                var data = await c.GetStringAsync(string.Format(pattern, ip));
                return data;
            }
        }

        public GeoInfo GetGeoInfoLocal(string ip)
        {
            if (string.IsNullOrEmpty(ip)) return default(GeoInfo);

            try
            {
                using (var reader = new DatabaseReader(@"IPDatabase\GeoLite2-City.mmdb"))
                {
                    var city = reader.City(ip);

                    var result = new GeoInfo(city.Country.Name, city.City.Name);
                    return result;
                }
            }
            catch (Exception)
            {
                return default(GeoInfo);
            }
        }
    }
}