namespace Alert.Service.Domain.Repositories.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Common.Contracts.Violation;
    using Entities;

    public interface IViolationRepository
    {
        Task SaveViolation(Guid userId, Violation violation);
        Task<bool> CheckIfViolationCreatedPastHour(Guid userId, ViolationType violationType);
        Task<IEnumerable<Violation>> GetUserViolations(Guid userId);
    }
}