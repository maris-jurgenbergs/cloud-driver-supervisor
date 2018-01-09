namespace User.Service.Infrastructure.Neo
{
    using Domain.Entities;
    using Neo4jClient.Extension.Cypher;
    using Relationships;

    public static class NeoConfig
    {
        public static void ConfigureModel()
        {
            FluentConfig.Config()
                .With<User>()
                .Match(x => x.AzureId)
                .Merge(x => x.AzureId)
                .MergeOnCreate(p => p.AzureId)
                .MergeOnCreate(p => p.CreatedAt)
                .MergeOnCreate(p => p.Phone)
                .MergeOnMatchOrCreate(p => p.Name)
                .MergeOnMatchOrCreate(p => p.Surname)
                .MergeOnMatchOrCreate(p => p.Email)
                .Set();

            FluentConfig.Config()
                .With<Role>()
                .Match(role => role.RoleType)
                .Merge(role => role.RoleType)
                .MergeOnMatchOrCreate(role => role.RoleType)
                .Set();

            FluentConfig.Config()
                .With<UserRoleRelationship>()
                .Set();
        }
    }
}