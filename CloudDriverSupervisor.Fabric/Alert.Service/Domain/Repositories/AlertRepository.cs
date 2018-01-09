namespace Alert.Service.Domain.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Common.Contracts.User;
    using Entities;
    using Infrastructure.Logging.Interfaces;
    using Infrastructure.Neo.Interfaces;
    using Infrastructure.Neo.Relationships;
    using Interfaces;
    using Neo4jClient;
    using Neo4jClient.Cypher;
    using Neo4jClient.Extension.Cypher;
    using Polly;
    using SeverityLevel = Common.Contracts.Alert.SeverityLevel;

    public class AlertRepository : IAlertRepository
    {
        private readonly IGraphClient _graphClientFunc;
        private readonly Policy _repositoryPolicy;

        public AlertRepository(IGraphClientBuilder graphClientBuilder, ILoggingService loggingService)
        {
            _graphClientFunc = graphClientBuilder.GetGraphClient();
            _repositoryPolicy = Policy.Handle<Exception>().WaitAndRetryAsync(5, retryAttempt =>
                    TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                (exception, span) => { loggingService.LogMessage(exception.ToString()); });
        }

        public async Task SaveAlert(Alert alert, Guid transportationId)
        {
            alert.CreatedAt = (float) DateTime.UtcNow.Subtract(DateTime.MinValue.AddYears(1969)).TotalSeconds;
            alert.AlertId = Guid.NewGuid();
            var relationship = new TransportationAlertRelationship("transportation", "alert");
            var query = new CypherFluentQuery(_graphClientFunc)
                .Match("(transportation:Transportation{transportationId: {transportationId}})")
                .WithParam("transportationId", transportationId)
                .CreateEntity(alert, "alert")
                .CreateRelationship(relationship);

            await _repositoryPolicy.ExecuteAsync(async () => await query.ExecuteWithoutResultsAsync());
        }

        public async Task<IEnumerable<Alert>> GetTransportationAlerts(Guid transportationId)
        {
            var query = new CypherFluentQuery(_graphClientFunc)
                .Match("(transportation:Transportation{transportationId: {transportationId}})")
                .WithParam("transportationId", transportationId)
                .Match("(transportation)-[:HAS_ALERT]->(alert:Alert)")
                .Return(alert => alert.As<Alert>());

            var result = await _repositoryPolicy.ExecuteAsync(async () => await query.ResultsAsync);
            return result;
        }

        public async Task<dynamic> GetAlerts()
        {
            var query = new CypherFluentQuery(_graphClientFunc)
                .Match("(alert:Alert)<-[:HAS_ALERT]-(transportation:Transportation)<-[:HAS_TRANSPORTATION]-(user:User)")
                .Where("alert.createdAt > {dayPeriod}")
                .WithParam("dayPeriod",
                    DateTime.UtcNow.AddDays(-1).Subtract(DateTime.MinValue.AddYears(1969)).TotalSeconds)
                .Return((alert, user) => new
                {
                    User = user.As<UserDto>(),
                    Alerts = alert.CollectAs<Alert>()
                });

            var result = await _repositoryPolicy.ExecuteAsync(async () => await query.ResultsAsync);
            return result;
        }

        public async Task<dynamic> GetAlert(Guid alertId)
        {
            var query = new CypherFluentQuery(_graphClientFunc)
                .Match("(alert:Alert)<-[:HAS_ALERT]-(transportation:Transportation)<-[:HAS_TRANSPORTATION]-(user:User)")
                .Where("alert.alertId = {alertId}")
                .WithParam("alertId", alertId)
                .Return((alert, user) => new
                {
                    User = user.As<UserDto>(),
                    Alert = alert.As<Alert>()
                });

            var result = await _repositoryPolicy.ExecuteAsync(async () => await query.ResultsAsync);
            return result;
        }

        public async Task UpdateAlertSeverityLevel(Guid alertId, SeverityLevel severityLevel)
        {
            var query = new CypherFluentQuery(_graphClientFunc)
                .Match("(alert:Alert{alertId: {alertId}})")
                .WithParam("alertId", alertId)
                .Set("alert.severityLevel = {severityLevel}")
                .WithParam("severityLevel", severityLevel);

            await _repositoryPolicy.ExecuteAsync(async () => await query.ExecuteWithoutResultsAsync());
        }
    }
}