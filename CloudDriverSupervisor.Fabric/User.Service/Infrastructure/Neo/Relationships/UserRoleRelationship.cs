namespace User.Service.Infrastructure.Neo.Relationships
{
    using Neo4jClient.Extension.Cypher;
    using Neo4jClient.Extension.Cypher.Attributes;

    [CypherLabel(Name = "HAS_ROLE")]
    public class UserRoleRelationship : BaseRelationship
    {
        public UserRoleRelationship(string key) : base(key)
        {
        }

        public UserRoleRelationship(string fromKey, string toKey) : base(fromKey, toKey)
        {
        }

        public UserRoleRelationship(string key, string fromKey, string toKey) : base(key, fromKey, toKey)
        {
        }
    }
}