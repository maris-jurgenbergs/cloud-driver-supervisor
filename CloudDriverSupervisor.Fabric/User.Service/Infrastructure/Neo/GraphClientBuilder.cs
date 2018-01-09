namespace User.Service.Infrastructure.Neo
{
    using System;
    using System.Net.Http;
    using Bootstrapper.Options;
    using Interfaces;
    using Microsoft.Extensions.Options;
    using Neo4jClient;
    using Newtonsoft.Json.Serialization;

    public class GraphClientBuilder : IGraphClientBuilder
    {
        private readonly IOptions<Neo4JOptions> _neo4JOptions;
        private IGraphClient _graphClient;

        public GraphClientBuilder(IOptions<Neo4JOptions> neo4JOptions)
        {
            _neo4JOptions = neo4JOptions;
        }

        public IGraphClient GetGraphClient()
        {
            return _graphClient ?? Build();
        }

        private IGraphClient Build()
        {
            var client = new HttpClient { Timeout = TimeSpan.FromMinutes(2) };
            var graphClient = new GraphClient(new Uri(_neo4JOptions.Value.Endpoint),
                new HttpClientWrapper(_neo4JOptions.Value.User, _neo4JOptions.Value.Password, client))
            {
                JsonContractResolver = new CamelCasePropertyNamesContractResolver(),
                UseJsonStreamingIfAvailable = true
            };
            graphClient.Connect();
            _graphClient = graphClient;
            return graphClient;
        }
    }
}