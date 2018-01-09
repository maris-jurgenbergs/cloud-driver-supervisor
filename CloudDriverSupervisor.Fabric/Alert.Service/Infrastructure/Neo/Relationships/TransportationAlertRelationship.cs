namespace Alert.Service.Infrastructure.Neo.Relationships
{
    using Neo4jClient.Extension.Cypher;
    using Neo4jClient.Extension.Cypher.Attributes;

    [CypherLabel(Name = "HAS_ALERT")]
    public class TransportationAlertRelationship : BaseRelationship
    {
        public TransportationAlertRelationship(string key) : base(key)
        {
        }

        public TransportationAlertRelationship(string fromKey, string toKey) : base(fromKey, toKey)
        {
        }

        public TransportationAlertRelationship(string key, string fromKey, string toKey) : base(key, fromKey, toKey)
        {
        }
    }
}