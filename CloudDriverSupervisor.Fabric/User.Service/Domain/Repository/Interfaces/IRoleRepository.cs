namespace User.Service.Domain.Repository.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Entities;

    public interface IRoleRepository
    {
        Task<IEnumerable<Role>> GetUserRoles(Guid userId);
        Task AddUserRole(Guid userId, RoleType roleType);
        Task DeleteUserRole(Guid userId, RoleType roleType);
    }
}