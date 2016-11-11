namespace Arma3BEClient.Common.Logging
{
    public interface ILog
    {
        void Debug(object message);
        void Info(object message);
        void Error(object message);
        void Fatal(object message);
    }
}