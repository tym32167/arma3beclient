using System.Net;
using Arma3BE.Client.Infrastructure;

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
    }
}