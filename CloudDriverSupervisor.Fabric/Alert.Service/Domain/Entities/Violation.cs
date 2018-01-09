namespace Alert.Service.Domain.Entities
{
    using Common.Contracts.Violation;

    public class Violation
    {
        public ViolationType Type { get; set; }

        public float CreatedAt { get; set; }
    }
}