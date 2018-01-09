namespace Alert.Service.Modules.Violation
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Domain.Entities;
    using Domain.Repositories.Interfaces;
    using Infrastructure.Bootstrapper.Interfaces;
    using Interfaces;

    public class ViolationService : IViolationService, ISingletonService
    {
        private readonly IViolationRepository _violationRepository;

        public ViolationService(IViolationRepository violationRepository)
        {
            _violationRepository = violationRepository;
        }

        public async Task<IEnumerable<Violation>> GetUserViolations(Guid userId)
        {
            var userViolations = await _violationRepository.GetUserViolations(userId);
            return userViolations;
        }
    }
}