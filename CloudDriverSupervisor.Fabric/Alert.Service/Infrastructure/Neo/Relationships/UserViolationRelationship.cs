namespace Alert.Service.Infrastructure.Neo.Relationships
{
    using Neo4jClient.Extension.Cypher;
    using Neo4jClient.Extension.Cypher.Attributes;

    [CypherLabel(Name = "HAS_VIOLATION")]
    public class UserViolationRelationship : BaseRelationship
    {
        public UserViolationRelationship(string key) : base(key)
        {
        }

        public UserViolationRelationship(string fromKey, string toKey) : base(fromKey, toKey)
        {
        }

        public UserViolationRelationship(string key, string fromKey, string toKey) : base(key, fromKey, toKey)
        {
        }
    }
}