namespace Alert.Service.Modules.Violation.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Domain.Entities;

    public interface IViolationService
    {
        Task<IEnumerable<Violation>> GetUserViolations(Guid userId);
    }
}