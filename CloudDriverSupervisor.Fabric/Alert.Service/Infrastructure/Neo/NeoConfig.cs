namespace Alert.Service.Infrastructure.Neo
{
    using Domain.Entities;
    using Neo4jClient.Extension.Cypher;

    public static class NeoConfig
    {
        public static void ConfigureModel()
        {
            FluentConfig.Config()
                .With<Violation>()
                .Match(x => x.Type)
                .Merge(x => x.Type)
                .Merge(x => x.CreatedAt)
                .MergeOnMatchOrCreate(p => p.Type)
                .MergeOnMatchOrCreate(p => p.CreatedAt)
                .Set();

            FluentConfig.Config()
                .With<Alert>()
                .Match(x => x.AlertId)
                .Merge(x => x.Type)
                .Merge(x => x.CreatedAt)
                .Merge(x => x.SeverityLevel)
                .Merge(x => x.Status)
                .Merge(x => x.Description)
                .Merge(x => x.AlertId)
                .MergeOnMatchOrCreate(p => p.Type)
                .MergeOnMatchOrCreate(p => p.CreatedAt)
                .MergeOnMatchOrCreate(p => p.SeverityLevel)
                .MergeOnMatchOrCreate(p => p.Status)
                .MergeOnMatchOrCreate(p => p.Description)
                .MergeOnMatchOrCreate(p => p.Description)
                .MergeOnMatchOrCreate(p => p.AlertId)
                .Set();
        }
    }
}