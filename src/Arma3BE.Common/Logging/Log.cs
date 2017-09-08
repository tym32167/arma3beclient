using log4net;
using System;
using System.Diagnostics;

namespace Arma3BEClient.Common.Logging
{
    public class Log : ILog
    {
        private readonly log4net.ILog _log;

        public Log()
        {
            _log = LogManager.GetLogger(typeof(Log));
        }

        public Log(Type targetType)
        {
            _log = LogManager.GetLogger(targetType);
        }

        #region Implementation of ILog

        public void Debug(object message)
        {
            DebugConditional(message);
        }

        [Conditional("DEBUG")]
        private void DebugConditional(object message)
        {
            _log.Debug(message);
        }


        public void Info(object message)
        {
            _log.Info(message);
        }

        public void Error(object message)
        {
            _log.Error(message);
        }

        public void Fatal(object message)
        {
            _log.Fatal(message);
        }

        #endregion
    }
}