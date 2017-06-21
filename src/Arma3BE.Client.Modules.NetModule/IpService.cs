using Arma3BE.Client.Infrastructure;
using Arma3BEClient.Common.Core;
using Arma3BEClient.Common.Logging;
using MaxMind.GeoIP2;
using MaxMind.GeoIP2.Responses;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Arma3BE.Client.Modules.NetModule
{
    public class IpService : DisposeObject, IIpService
    {
        private DatabaseReader reader;
        private static readonly ILog Log = LogFactory.Create(new StackTrace().GetFrame(0).GetMethod().DeclaringType);

        public IpService()
        {
            reader = new DatabaseReader(@"IPDatabase\GeoLite2-City.mmdb");
        }

        protected override void DisposeManagedResources()
        {
            base.DisposeManagedResources();
            reader.Dispose();
        }

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
                CityResponse city;
                if (reader.TryCity(ip, out city))
                {
                    var result = new GeoInfo(city.Country.Name, city.City.Name);
                    return result;
                }
                return default(GeoInfo);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return default(GeoInfo);
            }
        }
    }
}