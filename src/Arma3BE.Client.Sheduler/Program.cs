using System;
using System.Threading;
using Arma3BE.Server;
using Arma3BE.Server.ServerFactory;
using Arma3BEClient.Common.Logging;

namespace Arma3BEClient.Sheduler
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            string host;
            int port;
            string password;
            string adminname;
            string message;
            var interval = 0;


            if (args.Length == 6)
            {
                host = args[0];
                port = int.Parse(args[1]);
                password = args[2];
                adminname = args[3];
                message = args[4];
                interval = int.Parse(args[5]);
            }
            else
            {
                Console.WriteLine("Server IP");
                host = Console.ReadLine();

                Console.WriteLine("Server Port");
                port = int.Parse(Console.ReadLine());

                Console.WriteLine("Server Password");
                password = Console.ReadLine();

                Console.WriteLine("Server Admin name");
                adminname = Console.ReadLine();

                Console.WriteLine("Message");
                message = Console.ReadLine();

                Console.WriteLine("Interval");
                interval = int.Parse(Console.ReadLine());
            }


            Console.WriteLine(host);
            Console.WriteLine(port);
            Console.WriteLine(password);
            Console.WriteLine(adminname);
            Console.WriteLine(message);
            Console.WriteLine(interval);


            using (var uc = new BEServer(host, port, password, new FakeLog(), new BattlEyeServerFactory(new FakeLog())))
            {
                while (true)
                {
                    if (!uc.Connected)
                    {
                        Console.WriteLine("Attempt  to connect");
                        uc.Connect();
                        Thread.Sleep(1000);
                        continue;
                    }
                    var result = $" -1 {adminname}: {message}";
                    uc.SendCommand(CommandType.Say, result);
                    Console.WriteLine(message);

                    Thread.Sleep(interval*1000);
                }
            }
        }
    }

    public class FakeLog : ILog
    {
        public void DebugFormat(string format, object arg0)
        {
        }

        public void DebugFormat(string format, object arg0, object arg1)
        {
        }

        public void DebugFormat(string format, object arg0, object arg1, object arg2)
        {
        }

        public void DebugFormat(IFormatProvider provider, string format, params object[] args)
        {
        }

        public void Info(object message)
        {
        }

        public void Info(object message, Exception exception)
        {
        }

        public void InfoFormat(string format, params object[] args)
        {
        }

        public void InfoFormat(string format, object arg0)
        {
        }

        public void InfoFormat(string format, object arg0, object arg1)
        {
        }

        public void InfoFormat(string format, object arg0, object arg1, object arg2)
        {
        }

        public void InfoFormat(IFormatProvider provider, string format, params object[] args)
        {
        }

        public void Warn(object message)
        {
        }

        public void Warn(object message, Exception exception)
        {
        }

        public void WarnFormat(string format, params object[] args)
        {
        }

        public void WarnFormat(string format, object arg0)
        {
        }

        public void WarnFormat(string format, object arg0, object arg1)
        {
        }

        public void WarnFormat(string format, object arg0, object arg1, object arg2)
        {
        }

        public void WarnFormat(IFormatProvider provider, string format, params object[] args)
        {
        }

        public void Error(object message)
        {
        }

        public void Error(object message, Exception exception)
        {
        }

        public void ErrorFormat(string format, params object[] args)
        {
        }

        public void ErrorFormat(string format, object arg0)
        {
        }

        public void ErrorFormat(string format, object arg0, object arg1)
        {
        }

        public void ErrorFormat(string format, object arg0, object arg1, object arg2)
        {
        }

        public void ErrorFormat(IFormatProvider provider, string format, params object[] args)
        {
        }

        public void Fatal(object message)
        {
        }

        public void Fatal(object message, Exception exception)
        {
        }

        public void FatalFormat(string format, params object[] args)
        {
        }

        public void FatalFormat(string format, object arg0)
        {
        }

        public void FatalFormat(string format, object arg0, object arg1)
        {
        }

        public void FatalFormat(string format, object arg0, object arg1, object arg2)
        {
        }

        public void FatalFormat(IFormatProvider provider, string format, params object[] args)
        {
        }
    }
}