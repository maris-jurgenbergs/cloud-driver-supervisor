namespace Transportation.Service.Infrastructure.Neo.Relationships
{
    using Neo4jClient.Extension.Cypher;
    using Neo4jClient.Extension.Cypher.Attributes;

    [CypherLabel(Name = "HAS_CAPTURED_LOCATION")]
    public class TransportationCapturedLocationRelationship : BaseRelationship
    {
        public TransportationCapturedLocationRelationship(string key) : base(key)
        {
        }

        public TransportationCapturedLocationRelationship(string fromKey, string toKey) : base(fromKey, toKey)
        {
        }

        public TransportationCapturedLocationRelationship(string key, string fromKey, string toKey) : base(key, fromKey,
            toKey)
        {
        }
    }
}