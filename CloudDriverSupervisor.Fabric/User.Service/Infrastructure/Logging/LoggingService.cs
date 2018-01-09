namespace User.Service.Infrastructure.Logging
{
    using System;
    using Bootstrapper.Interfaces;
    using Interfaces;

    public class LoggingService : ILoggingService, ISingletonService
    {
        private Action<string> _logAction;

        public void SetLogAction(Action<string> logAction)
        {
            _logAction = logAction;
        }

        public Action<string> GetLogAction()
        {
            return _logAction ?? throw new InvalidOperationException("Log action has not been set");
        }

        public void LogMessage(string message)
        {
            _logAction(message);
        }
    }
}