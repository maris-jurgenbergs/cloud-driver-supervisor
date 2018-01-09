namespace Transportation.Service.Domain.Repository
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Common.Contracts.Alert;
    using Common.Contracts.Transportation;
    using Common.Contracts.User;
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

    public class TransportationRepository : ITransportationRepository
    {
        private readonly ICypherFluentQuery _cypherFluentQuery;
        private readonly IGraphClient _graphClientFunc;
        private readonly Policy _repositoryPolicy;

        public TransportationRepository(IGraphClientBuilder graphClientBuilder, ILoggingService loggingService)
        {
            _graphClientFunc = graphClientBuilder.GetGraphClient();
            _cypherFluentQuery = new CypherFluentQuery(_graphClientFunc);
            _repositoryPolicy = Policy.Handle<Exception>().WaitAndRetryAsync(5, retryAttempt =>
                    TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                (exception, span) => { loggingService.LogMessage(exception.ToString()); });
        }

        public async Task SaveTransportation(Transportation transportation, Guid userId)
        {
            var relationship = new UserTransportationRelationship("user", "transportation");

            var query = new CypherFluentQuery(_graphClientFunc)
                .CreateEntity(transportation, "transportation")
                .With("transportation")
                .Match("(user:User{azureId: {userId}})")
                .WithParam("userId", userId)
                .CreateRelationship(relationship);
            await query.ExecuteWithoutResultsAsync();
        }

        public async Task PatchTransportationStatus(Transportation transportation)
        {
            var query = new CypherFluentQuery(_graphClientFunc)
                .MatchEntity(transportation, "transportation")
                .Set("transportation.isActive = {isActive}")
                .WithParam("isActive", transportation.IsActive);
            await query.ExecuteWithoutResultsAsync();
        }

        public async Task SaveCapturedLocations(Dictionary<Guid, List<CapturedLocation>> locationDictionary)
        {
            // 2 type of query handling was developed. The Second fluent query methods is left here, but it had really bad performance in handling a lot of objects
            // Easier to use StringBuilder, that can generate the appropriate Cypher Query fast
            await StringQueryMethod(locationDictionary);
            //await FluentQueryMethod(locationDictionary);
        }

        public async Task<dynamic> GetTransportations(DateTime periodStart, DateTime periodEnd)
        {
            var relationship = new TransportationCapturedLocationRelationship("transportation", "capturedLocation");

            var query = new CypherFluentQuery(_graphClientFunc)
                .Match("(transportation:Transportation)")
                .MatchRelationship(relationship)
                .Where("capturedLocation.capturedDateTimeUtc >= {periodStart} " +
                       "AND capturedLocation.capturedDateTimeUtc <= {periodEnd}")
                .WithParam("periodStart",
                    periodStart.ToUniversalTime().Subtract(DateTime.MinValue.AddYears(1969)).TotalSeconds)
                .WithParam("periodEnd",
                    periodEnd.ToUniversalTime().Subtract(DateTime.MinValue.AddYears(1969)).TotalSeconds)
                .Return((transportation, capturedLocation) => new
                {
                    transportation = transportation.As<Transportation>(),
                    capturedLocations = capturedLocation.CollectAs<CapturedLocation>()
                });
            var result = await _repositoryPolicy.ExecuteAsync(async () => await query.ResultsAsync);
            return result;
        }

        public async Task<dynamic> GetUserTransportations(Guid userId, DateTime periodStart, DateTime periodEnd)
        {
            var userTransportationRelationship = new UserTransportationRelationship("user", "transportation");
            var transportationCapturedLocationRelationship =
                new TransportationCapturedLocationRelationship("transportation", "capturedLocation");

            var query = new CypherFluentQuery(_graphClientFunc)
                .Match("(user:User{azureId: {userId}})")
                .WithParam("userId", userId)
                .MatchRelationship(userTransportationRelationship)
                .Match("(transportation:Transportation)")
                .MatchRelationship(transportationCapturedLocationRelationship)
                .Where("capturedLocation.capturedDateTimeUtc >= {periodStart} " +
                       "AND capturedLocation.capturedDateTimeUtc <= {periodEnd}")
                .WithParam("periodStart",
                    periodStart.ToUniversalTime().Subtract(DateTime.MinValue.AddYears(1969)).TotalSeconds)
                .WithParam("periodEnd",
                    periodEnd.ToUniversalTime().Subtract(DateTime.MinValue.AddYears(1969)).TotalSeconds)
                .Return((transportation, capturedLocation) => new
                {
                    transportation = transportation.As<Transportation>(),
                    capturedLocations = capturedLocation.CollectAs<CapturedLocation>()
                });
            var result = await _repositoryPolicy.ExecuteAsync(async () => await query.ResultsAsync);
            return result;
        }

        public async Task<GetTransportationDetailsResultMessage> GetTransportationDetails(Guid transportationId)
        {
            var userTransportationRelationship = new UserTransportationRelationship("user", "transportation");

            var query = new CypherFluentQuery(_graphClientFunc)
                .Match("(transportation:Transportation{transportationId: {transportationId}})")
                .WithParam("transportationId", transportationId)
                .MatchRelationship(userTransportationRelationship)
                .Match("(transportation:Transportation)")
                .OptionalMatch("(transportation:Transportation)-[:HAS_ALERT]->(alert:Alert)")
                .OptionalMatch("(user:User)-[:HAS_VIOLATION]->(violation:Violation)")
                .Return((user, violation, alert) => new GetTransportationDetailsResultMessage
                {
                    User = user.As<UserDto>(),
                    Violations = violation.CollectAs<ViolationDto>(),
                    Alerts = alert.CollectAs<AlertResultDto>()
                });
            var result = await _repositoryPolicy.ExecuteAsync(async () => await query.ResultsAsync);
            return result.FirstOrDefault();
        }

        private async Task StringQueryMethod(Dictionary<Guid, List<CapturedLocation>> locationDictionary)
        {
            var query = new StringBuilder();
            var transportationCount = 0;
            var locationCount = 0;
            foreach (var transportationLocations in locationDictionary)
            {
                if (transportationCount != 0)
                {
                    query.Append("\r\nWITH 1 as dummy\r\n");
                }

                var transportation = new Transportation { TransportationId = transportationLocations.Key };
                var transportationKey = $"transportation_{transportationCount}";
                query.Append(
                    $"MATCH ({transportationKey}:Transportation {{transportationId: '{transportation.TransportationId}'}})");
                foreach (var location in transportationLocations.Value)
                {
                    var locationKey = $"capturedLocation_{locationCount}";
                    query.Append(
                        $"\r\nCREATE ({locationKey}:CapturedLocation {{\r\ncapturedDateTimeUtc: {location.CapturedDateTimeUtc}, \r\n longitude: {location.Longitude}, \r\n altitude: {location.Altitude}}})" +
                        $"\r\nCREATE ({transportationKey})-[{transportationKey}{locationKey}:HAS_CAPTURED_LOCATION]->({locationKey})");
                    locationCount++;
                }

                transportationCount++;
            }

            var cypherQuery = new CypherQuery(query.ToString(), new Dictionary<string, object>(),
                CypherResultMode.Set, CypherResultFormat.Rest);
            await ((IRawGraphClient) _graphClientFunc).ExecuteCypherAsync(cypherQuery);
        }

        private async Task FluentQueryMethod(Dictionary<Guid, List<CapturedLocation>> locationDictionary)
        {
            var cypherFluentQuery = _cypherFluentQuery;
            var transportationCount = 0;
            var locationCount = 0;
            foreach (var transportationLocations in locationDictionary)
            {
                if (transportationCount != 0)
                {
                    cypherFluentQuery = cypherFluentQuery.With("1 as dummy");
                }

                var transportation = new Transportation { TransportationId = transportationLocations.Key };
                var transportationKey = $"transportation_{transportationCount}";

                cypherFluentQuery = cypherFluentQuery.MatchEntity(transportation, transportationKey);

                foreach (var location in transportationLocations.Value)
                {
                    var locationKey = $"capturedLocation_{locationCount}";
                    var relationship = new TransportationCapturedLocationRelationship(transportationKey, locationKey);
                    cypherFluentQuery = cypherFluentQuery
                        .CreateEntity(location, locationKey)
                        .CreateRelationship(relationship);

                    locationCount++;
                }

                transportationCount++;
            }

            await cypherFluentQuery.ExecuteWithoutResultsAsync();
        }
    }
}