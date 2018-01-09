namespace Alert.Service.Modules.Violation.Interfaces
{
    using System;
    using System.Threading.Tasks;
    using Entities;

    public interface IViolationCalculationService
    {
        Task<DrivingCalculationResult> CalculateDrivingLimit(string transportationListSasUri, Guid userId);
    }
}