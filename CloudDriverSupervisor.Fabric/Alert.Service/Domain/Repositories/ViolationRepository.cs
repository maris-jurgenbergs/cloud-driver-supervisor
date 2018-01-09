namespace Alert.Service.Domain.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Common.Contracts.Violation;
    using Entities;
    using Infrastructure.Logging.Interfaces;
    using Infrastructure.Neo.Interfaces;
    using Infrastructure.Neo.Relationships;
    using Interfaces;
    using Neo4jClient;
    using Neo4jClient.Cypher;
    using Neo4jClient.Extension.Cypher;
    using Polly;

    public class ViolationRepository : IViolationRepository
    {
        private readonly IGraphClient _graphClientFunc;
        private readonly Policy _repositoryPolicy;

        public ViolationRepository(IGraphClientBuilder graphClientBuilder, ILoggingService loggingService)
        {
            _graphClientFunc = graphClientBuilder.GetGraphClient();
            _repositoryPolicy = Policy.Handle<Exception>().WaitAndRetryAsync(5, retryAttempt =>
                    TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                (exception, span) => { loggingService.LogMessage(exception.ToString()); });
        }

        public async Task SaveViolation(Guid userId, Violation violation)
        {
            var relationship = new UserViolationRelationship("user", "violation");
            var query = new CypherFluentQuery(_graphClientFunc)
                .Match("(user:User{azureId: {userId}})")
                .WithParam("userId", userId)
                .CreateEntity(violation, "violation")
                .CreateRelationship(relationship);

            await _repositoryPolicy.ExecuteAsync(async () => await query.ExecuteWithoutResultsAsync());
        }

        public async Task<bool> CheckIfViolationCreatedPastHour(Guid userId, ViolationType violationType)
        {
            var query = new CypherFluentQuery(_graphClientFunc)
                .Match("(user:User{azureId: {userId}})")
                .WithParam("userId", userId)
                .Match("(violation:Violation{type: {violationType}})")
                .WithParam("violationType", violationType)
                .Where("violation.createdAt > {pastHour}")
                .WithParam("pastHour",
                    DateTime.UtcNow.AddHours(-1).Subtract(DateTime.MinValue.AddYears(1969))
                        .TotalSeconds)
                .Return(violation => violation.As<Violation>());

            var result = await _repositoryPolicy.ExecuteAsync(async () => await query.ResultsAsync);
            return result.Any();
        }

        public async Task<IEnumerable<Violation>> GetUserViolations(Guid userId)
        {
            var relationship = new UserViolationRelationship("user", "violation");
            var query = new CypherFluentQuery(_graphClientFunc)
                .Match("(user:User{azureId: {userId}})")
                .WithParam("userId", userId)
                .MatchRelationship(relationship)
                .Return(violation => violation.As<Violation>());

            var results = await _repositoryPolicy.ExecuteAsync(async () => await query.ResultsAsync);
            return results;
        }
    }
}