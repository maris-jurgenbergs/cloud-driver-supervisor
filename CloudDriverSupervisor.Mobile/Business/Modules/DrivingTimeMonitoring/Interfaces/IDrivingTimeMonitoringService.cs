namespace Mobile.Business.Modules.DrivingTimeMonitoring.Interfaces
{
    using System.Threading.Tasks;
    using Entities;

    public interface IDrivingTimeMonitoringService
    {
        Task<DrivingTimeCalculations> GetDrivingTime();
    }
}