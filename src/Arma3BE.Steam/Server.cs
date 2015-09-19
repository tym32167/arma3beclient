using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Arma3BEClient.Steam
{
    public class Server
    {
        public Server(IPEndPoint endPoint)
        {
            EndPoint = endPoint;
        }

        public IPEndPoint EndPoint { get; }


        public ServerRulesResult GetServerRulesSync(GetServerInfoSettings settings)
        {
            var localEndpoint = new IPEndPoint(IPAddress.Any, 0);
            using (var client = new UdpClient(localEndpoint))
            {
                client.Client.ReceiveTimeout = settings.ReceiveTimeout;
                client.Client.SendTimeout = settings.SendTimeout;

                client.Connect(EndPoint);
                var requestPacket = new List<byte>();
                requestPacket.AddRange(new byte[] {0xFF, 0xFF, 0xFF, 0xFF, 0x56});
                requestPacket.AddRange(BitConverter.GetBytes(-1));
                client.Send(requestPacket.ToArray(), requestPacket.ToArray().Length);
                var responseData = client.Receive(ref localEndpoint);
                requestPacket.Clear();
                requestPacket.AddRange(new byte[] {0xFF, 0xFF, 0xFF, 0xFF, 0x56});
                requestPacket.AddRange(responseData.Skip(5).Take(4));
                client.Send(requestPacket.ToArray(), requestPacket.ToArray().Length);
                responseData = client.Receive(ref localEndpoint);
                return ServerRulesResult.Parse(responseData);
            }
        }


        public ServerPlayers GetServerChallengeSync(GetServerInfoSettings settings)
        {
            var localEndpoint = new IPEndPoint(IPAddress.Any, 0);
            using (var client = new UdpClient(localEndpoint))
            {
                client.Client.ReceiveTimeout = settings.ReceiveTimeout;
                client.Client.SendTimeout = settings.SendTimeout;

                client.Connect(EndPoint);

                var requestPacket = new List<byte>();
                requestPacket.AddRange(new byte[] {0xFF, 0xFF, 0xFF, 0xFF, 0x55});
                requestPacket.AddRange(BitConverter.GetBytes(-1));

                client.Send(requestPacket.ToArray(), requestPacket.ToArray().Length);
                var responseData = client.Receive(ref localEndpoint);
                requestPacket.Clear();
                requestPacket.AddRange(new byte[] {0xFF, 0xFF, 0xFF, 0xFF, 0x55});
                requestPacket.AddRange(responseData.Skip(5).Take(4));
                client.Send(requestPacket.ToArray(), requestPacket.ToArray().Length);
                responseData = client.Receive(ref localEndpoint);

                return ServerPlayers.Parse(responseData);
            }
        }


        public ServerInfoResult GetServerInfoSync(GetServerInfoSettings settings)
        {
            var localEndPoint = new IPEndPoint(IPAddress.Any, 0);
            using (var client = new UdpClient(localEndPoint))
            {
                client.Client.ReceiveTimeout = settings.ReceiveTimeout;
                client.Client.SendTimeout = settings.SendTimeout;

                client.Connect(EndPoint);
                var requestPacket = new List<byte>();
                requestPacket.AddRange(new byte[] {0xFF, 0xFF, 0xFF, 0xFF});
                requestPacket.Add(0x54);
                requestPacket.AddRange(Encoding.ASCII.GetBytes("Source Engine Query"));
                requestPacket.Add(0x00);
                var requestData = requestPacket.ToArray();
                client.Send(requestData, requestData.Length);
                var data = client.Receive(ref localEndPoint);
                return ServerInfoResult.Parse(data);
            }
        }
    }
}