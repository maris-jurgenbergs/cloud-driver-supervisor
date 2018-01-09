namespace User.Service.Domain.Repository
{
    using System;
    using System.Threading.Tasks;
    using Entities;
    using Infrastructure.Logging.Interfaces;
    using Infrastructure.Neo.Interfaces;
    using Interfaces;
    using Neo4jClient;
    using Neo4jClient.Cypher;
    using Neo4jClient.Extension.Cypher;
    using Polly;

    public class UserRepository : IUserRepository
    {
        private readonly IGraphClient _graphClientFunc;
        private readonly Policy _repositoryPolicy;

        public UserRepository(IGraphClientBuilder graphClientBuilder, ILoggingService loggingService)
        {
            _graphClientFunc = graphClientBuilder.GetGraphClient();
            _repositoryPolicy = Policy.Handle<Exception>().WaitAndRetryAsync(5, retryAttempt =>
                    TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                (exception, span) => { loggingService.LogMessage(exception.ToString()); });
        }

        public async Task<dynamic> GetUsers()
        {
            var query = new CypherFluentQuery(_graphClientFunc)
                .Match("(user:User)")
                .OptionalMatch("(user)-[:HAS_ROLE]->(role:Role)")
                .Return((user, role) => new
                {
                    user = user.As<User>(),
                    roles = role.CollectAs<Role>()
                });
            var result = await _repositoryPolicy.ExecuteAsync(async () => await query.ResultsAsync);
            return result;
        }

        public async Task DeleteUser(Guid userId)
        {
            var query = new CypherFluentQuery(_graphClientFunc)
                .Match("(user:User{azureId: {userId}})")
                .WithParam("userId", userId)
                .Remove("user:User")
                .Set("user:User_Deleted");
            await _repositoryPolicy.ExecuteAsync(async () => await query.ExecuteWithoutResultsAsync());
        }

        public async Task AddUser(User user)
        {
            var query = new CypherFluentQuery(_graphClientFunc)
                .MergeEntity(user);
            await _repositoryPolicy.ExecuteAsync(async () => await query.ExecuteWithoutResultsAsync());
        }
    }
}