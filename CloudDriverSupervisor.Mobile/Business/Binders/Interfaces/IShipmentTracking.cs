namespace Mobile.Business.Binders.Interfaces
{
    using System;

    public interface IShipmentTracking
    {
        void StartTransportationTracking(Guid transportationId);
    }
}