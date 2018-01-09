namespace Alert.Service.Infrastructure.Logging.Interfaces
{
    using System;

    public interface ILoggingService
    {
        void SetLogAction(Action<string> logAction);
        Action<string> GetLogAction();
        void LogMessage(string message);
    }
}