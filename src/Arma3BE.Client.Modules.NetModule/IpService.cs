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

        public string GetCountryLocal(string ip)
        {
            if (string.IsNullOrEmpty(ip)) return string.Empty;

            try
            {
                using (var reader = new DatabaseReader(@"IPDatabase\GeoLite2-Country.mmdb"))
                {
                    var country = reader.Country(ip);
                    return country.Country.Name;
                }
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }


        public string GetCityLocal(string ip)
        {
            if (string.IsNullOrEmpty(ip)) return string.Empty;

            try
            {
                using (var reader = new DatabaseReader(@"IPDatabase\GeoLite2-City.mmdb"))
                {
                    var city = reader.City(ip);
                    return city.City.Name;
                }
            }
            catch (Exception e)
            {
                return string.Empty;
            }
        }
    }
}