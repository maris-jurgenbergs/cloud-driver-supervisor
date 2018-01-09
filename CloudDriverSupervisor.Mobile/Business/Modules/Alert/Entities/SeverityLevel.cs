namespace Mobile.Business.Modules.Alert.Entities
{
    using System.ComponentModel;

    public enum SeverityLevel
    {
        [Description("Unevaluated")] Unevaluated = 0,
        [Description("Trivial")] Trivial = 1,
        [Description("Low")] Low = 2,
        [Description("Average")] Average = 3,
        [Description("High")] High = 4,
        [Description("Critical")] Critical = 5
    }
}