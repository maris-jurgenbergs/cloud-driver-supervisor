namespace Transportation.Service.Domain.Repository.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Common.Contracts.Transportation;
    using Entities;

    public interface ITransportationRepository
    {
        Task SaveTransportation(Transportation transportation, Guid userId);
        Task SaveCapturedLocations(Dictionary<Guid, List<CapturedLocation>> locationDictionary);
        Task<dynamic> GetTransportations(DateTime periodStart, DateTime periodEnd);
        Task PatchTransportationStatus(Transportation transportation);
        Task<dynamic> GetUserTransportations(Guid userId, DateTime periodStart, DateTime periodEnd);
        Task<GetTransportationDetailsResultMessage> GetTransportationDetails(Guid transportationId);
    }
}