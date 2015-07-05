using System;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using MaxMind.GeoIP2;

namespace Arma3BEClient.Helpers
{
    public static class IPInfo
    {
        public static async Task<string> Get(string ip)
        {
            if (string.IsNullOrEmpty(ip)) return string.Empty;
            var c = new HttpClient();
            var pattern = ConfigurationManager.AppSettings["IPServicePattern"];
            var data = await c.GetStringAsync(string.Format(pattern, ip));
            return data;
        }

        public static string GetCountryLocal(string ip)
        {
            if (string.IsNullOrEmpty(ip)) return string.Empty;

            try
            {
                using (var reader = new DatabaseReader(@"IPDatabase\GeoLite2-City.mmdb"))
                {
                    var city = reader.City(ip);
                    return city.Country.Name;
                }
            }
            catch (Exception e)
            {
                return string.Empty;
            }
        }

        public static string GetIPAddress(string host)
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
    }
}