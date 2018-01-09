namespace Mobile.Business.Modules.Alert.Entities
{
    using System.ComponentModel;

    public enum AlertStatus
    {
        [Description("Active")] Active = 0,
        [Description("Resolved")] Resolved = 1
    }
}