namespace Transportation.Service.Infrastructure.Neo
{
    using Domain.Entities;
    using Neo4jClient.Extension.Cypher;

    public static class NeoConfig
    {
        public static void ConfigureModel()
        {
            FluentConfig.Config()
                .With<Transportation>()
                .Match(x => x.TransportationId)
                .Merge(x => x.TransportationId)
                .Merge(x => x.IsActive)
                .MergeOnCreate(p => p.TransportationId)
                .MergeOnCreate(p => p.IsActive)
                .MergeOnCreate(p => p.CreatedAt)
                .Set();

            FluentConfig.Config()
                .With<CapturedLocation>()
                .MergeOnMatchOrCreate(x => x.Altitude)
                .MergeOnMatchOrCreate(x => x.Longitude)
                .MergeOnMatchOrCreate(x => x.CapturedDateTimeUtc)
                .Set();
        }
    }
}