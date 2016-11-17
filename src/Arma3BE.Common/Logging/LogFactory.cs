using System;

namespace Arma3BEClient.Common.Logging
{
    public class LogFactory
    {
        public static ILog Create(Type owner)
        {
            return new Log(owner);
        }
    }
}