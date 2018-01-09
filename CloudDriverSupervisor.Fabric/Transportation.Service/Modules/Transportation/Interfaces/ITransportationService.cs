namespace Transportation.Service.Modules.Transportation.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Common.Contracts.Transportation;
    using Domain.Entities;

    public interface ITransportationService
    {
        Task<string> GetTransportationListSasUri(DateTime periodStart, DateTime periodEnd);

        Task<IEnumerable<string>> GetTransportationListSasUri(
            IEnumerable<Dictionary<Guid, List<CapturedLocation>>> listOfParsedDictionaries);

        Task<string> GetUserTransportationListSasUri(Guid userId, DateTime periodStart, DateTime periodEnd);
        Task<GetTransportationDetailsResultMessage> GetTransportationDetails(Guid transportationId);
    }
}