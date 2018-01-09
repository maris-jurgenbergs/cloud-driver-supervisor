namespace Mobile.Business.Binders
{
    using System;
    using Android.OS;
    using Interfaces;
    using Modules.Tracking;

    public class TrackingBinder : Binder, IShipmentTracking
    {
        public TrackingBinder(TrackingService trackingService)
        {
            TrackingService = trackingService;
        }

        public TrackingService TrackingService { get; }

        public void StartTransportationTracking(Guid transportationID)
        {
            TrackingService.StartShipmentTracking(transportationID);
        }

        public void StopShipmentTracking()
        {
            TrackingService.StopShipmentTracking();
        }
    }
}