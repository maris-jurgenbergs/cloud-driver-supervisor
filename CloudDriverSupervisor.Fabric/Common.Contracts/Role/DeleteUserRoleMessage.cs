namespace Common.Contracts.Role
{
    using System;
    using System.Collections.Generic;

    public class DeleteUserRoleMessage
    {
        public Guid UserId { get; set; }

        public IEnumerable<string> Roles { get; set; }
    }
}