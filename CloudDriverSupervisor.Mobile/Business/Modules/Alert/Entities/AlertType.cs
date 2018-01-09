namespace Mobile.Business.Modules.Alert.Entities
{
    using System.ComponentModel;

    public enum AlertType
    {
        Unknown = 0,
        [Description("Road accident")] RoadAccident = 1,
        [Description("Health issue")] HealthIssue = 2,
        [Description("Assault")] Assault = 3,
        [Description("Heavy Traffic")] HeavyTraffic = 4,
        [Description("Other")] Other = 5
    }
}