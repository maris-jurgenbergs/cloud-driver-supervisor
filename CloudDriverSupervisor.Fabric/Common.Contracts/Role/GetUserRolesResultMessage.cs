namespace Common.Contracts.Role
{
    public class GetUserRolesResultMessage : IGatewayResultMessage
    {
        public string[] Roles;
    }
}
