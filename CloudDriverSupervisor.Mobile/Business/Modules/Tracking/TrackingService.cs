namespace Mobile.Business.Modules.Tracking
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Timers;
    using Android.App;
    using Android.Content;
    using Android.Locations;
    using Android.OS;
    using ApiClient.Interfaces;
    using Autofac;
    using Binders;
    using Bootstrapper;
    using Configuration.Interfaces;
    using Entities;

    [Service]
    public class TrackingService : Service
    {
        private readonly IApiService _apiService;
        private readonly IConfigurationService _configurationService;
        private Timer _locationCommunicationTimer;
        private LocationListener _locationListener;

        private LocationManager _locationManager;
        private string _locationProvider;

        public TrackingService()
        {
            var container = Bootstrapper.GetContainer();
            var apiService = container.Resolve<IApiService>();
            _apiService = apiService;
            var configurationService = container.Resolve<IConfigurationService>();
            _configurationService = configurationService;
        }

        public IBinder Binder { get; private set; }

        public override void OnCreate()
        {
            _locationManager = (LocationManager) GetSystemService(LocationService);
            var criteriaForLocationService = new Criteria
            {
                Accuracy = Accuracy.Fine
            };
            var acceptableLocationProviders = _locationManager.GetProviders(criteriaForLocationService, true);

            if (acceptableLocationProviders.Any())
            {
                _locationProvider = acceptableLocationProviders.First();
            }
            else
            {
                _locationProvider = string.Empty;
            }

            base.OnCreate();
        }

        public override IBinder OnBind(Intent intent)
        {
            Binder = new TrackingBinder(this);

            return Binder;
        }

        public void StartShipmentTracking(Guid transportationId)
        {
            _configurationService.SetCurrentTransportationId(transportationId);
            _locationListener = new LocationListener();
            _locationManager.RequestLocationUpdates(_locationProvider, 1000, 0, _locationListener);
            _locationCommunicationTimer = new Timer { Interval = 2000 };
            _locationCommunicationTimer.Elapsed += async delegate
            {
                var capturedLocations = _locationListener.GetCapturedLocations();
                if (capturedLocations.Any())
                {
                    await _apiService.PostCapturedLocations(transportationId, capturedLocations);
                }
            };
            _locationCommunicationTimer.Start();
        }

        public void StopShipmentTracking()
        {
            _locationCommunicationTimer.Stop();
            _locationCommunicationTimer = null;
            _locationManager.RemoveUpdates(_locationListener);
            _locationListener.Dispose();
            _locationListener = null;
        }

        private List<CapturedLocation> GetCapturedLocations()
        {
            if (_locationListener == null)
            {
                throw new InvalidOperationException("Location listener has no been initialized");
            }

            return _locationListener.GetCapturedLocations();
        }
    }
}