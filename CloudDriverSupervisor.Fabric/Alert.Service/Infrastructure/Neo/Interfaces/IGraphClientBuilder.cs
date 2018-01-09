namespace Alert.Service.Infrastructure.Neo.Interfaces
{
    using Neo4jClient;

    public interface IGraphClientBuilder
    {
        IGraphClient GetGraphClient();
    }
}