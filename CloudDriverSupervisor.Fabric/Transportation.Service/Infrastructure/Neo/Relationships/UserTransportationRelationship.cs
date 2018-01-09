namespace Transportation.Service.Infrastructure.Neo.Relationships
{
    using Neo4jClient.Extension.Cypher;
    using Neo4jClient.Extension.Cypher.Attributes;

    [CypherLabel(Name = "HAS_TRANSPORTATION")]
    public class UserTransportationRelationship : BaseRelationship
    {
        public UserTransportationRelationship(string key) : base(key)
        {
        }

        public UserTransportationRelationship(string fromKey, string toKey) : base(fromKey, toKey)
        {
        }

        public UserTransportationRelationship(string key, string fromKey, string toKey) : base(key, fromKey, toKey)
        {
        }
    }
}