namespace Common.Contracts.Alert
{
    using System;

    public class PatchAlertSeverityMessage
    {
        public Guid AlertId { get; set; }

        public SeverityLevel SeverityLevel { get; set; }
    }
}