namespace Transportation.Service.Modules.CapturedLocation.Interfaces
{
    using System;
    using System.Collections.Generic;
    using Domain.Entities;

    public interface ICapturedLocationService
    {
        void ProcessCapturedLocations(Guid transportationId, List<CapturedLocation> locations);
        void StartTransactionCountdown();
    }
}