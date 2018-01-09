namespace User.Service.Domain.Repository
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Entities;
    using Infrastructure.Logging.Interfaces;
    using Infrastructure.Neo.Interfaces;
    using Infrastructure.Neo.Relationships;
    using Interfaces;
    using Neo4jClient;
    using Neo4jClient.Cypher;
    using Neo4jClient.Extension.Cypher;
    using Polly;

    public class RoleRepository : IRoleRepository
    {
        private readonly IGraphClient _graphClientFunc;
        private readonly Policy _repositoryPolicy;

        public RoleRepository(IGraphClientBuilder graphClientBuilder, ILoggingService loggingService)
        {
            _graphClientFunc = graphClientBuilder.GetGraphClient();
            _repositoryPolicy = Policy.Handle<Exception>().WaitAndRetryAsync(5, retryAttempt =>
                    TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                (exception, span) => { loggingService.LogMessage(exception.ToString()); });
        }

        public async Task<IEnumerable<Role>> GetUserRoles(Guid userId)
        {
            var user = new User
            {
                AzureId = userId
            };

            var userRoleRelationship = new UserRoleRelationship("user", "role");
            var cypherFluentQuery = new CypherFluentQuery(_graphClientFunc)
                .MatchEntity(user)
                .MatchRelationship(userRoleRelationship)
                .Return(role => role.As<Role>());

            var result = await cypherFluentQuery.ResultsAsync;
            return result;
        }

        public async Task AddUserRole(Guid userId, RoleType roleType)
        {
            var user = new User
            {
                AzureId = userId
            };

            var role = new Role
            {
                RoleType = roleType
            };

            var userRoleRelationship = new UserRoleRelationship("user", "role");
            var query = new CypherFluentQuery(_graphClientFunc)
                .MatchEntity(user)
                .MergeEntity(role)
                .CreateRelationship(userRoleRelationship);

            await _repositoryPolicy.ExecuteAsync(async () => await query.ExecuteWithoutResultsAsync());
        }

        public async Task DeleteUserRole(Guid userId, RoleType roleType)
        {
            var user = new User
            {
                AzureId = userId
            };

            var query = new CypherFluentQuery(_graphClientFunc)
                .MatchEntity(user)
                .Match("(user)-[relationship:HAS_ROLE]->(:Role{roleType: {roleType}})")
                .WithParam("roleType", roleType)
                .Delete("relationship");

            await _repositoryPolicy.ExecuteAsync(async () => await query.ExecuteWithoutResultsAsync());
        }
    }
}