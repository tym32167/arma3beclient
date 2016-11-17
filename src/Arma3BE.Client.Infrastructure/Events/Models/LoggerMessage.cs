using Arma3BEClient.Common.Attributes;
using System;

namespace Arma3BE.Client.Infrastructure.Events.Models
{
    public class LoggerMessage
    {
        public enum LogLevel { Debug, Info, Error, Fatal }

        [ShowInUi]
        public LogLevel Level { get; }

        [ShowInUi]
        public string Message { get; }
        public Exception Exception { get; }

        public LoggerMessage(LogLevel level, string message, Exception exception = null)
        {
            Level = level;
            Message = message;
            Exception = exception;
        }
    }
}