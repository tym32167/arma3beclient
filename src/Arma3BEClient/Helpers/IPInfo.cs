using System;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Arma3BEClient.Helpers
{
    public static class IPInfo
    {
        public async static Task<string> Get(string ip)
        {
            if (string.IsNullOrEmpty(ip)) return string.Empty;

            var c = new HttpClient();
            var pattern = ConfigurationManager.AppSettings["IPServicePattern"];
            var data = await c.GetStringAsync(string.Format(pattern, ip));
            return data;
        }

        public static string GetIPAddress(string host)
        {
            IPAddress ip;

            if (IPAddress.TryParse(host, out ip))
            {
                return ip.ToString();
            }
            else
            {
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

            return string.Empty;
        }
    }
}