using System.Net;

namespace Arma3BEClient.Common.Dns
{
    public static class DnsService
    {
        public static string GetIpAddress(string host)
        {
            IPAddress ip;

            if (IPAddress.TryParse(host, out ip))
            {
                return ip.ToString();
            }
            try
            {
                var entry = System.Net.Dns.GetHostEntry(host);
                return entry.AddressList[0].ToString();
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}