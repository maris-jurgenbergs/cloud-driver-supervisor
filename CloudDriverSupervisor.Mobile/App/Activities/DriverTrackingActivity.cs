namespace Mobile.App.Activities
{
    using System;
    using System.Threading.Tasks;
    using System.Timers;
    using Alert;
    using Android;
    using Android.App;
    using Android.Content.PM;
    using Android.OS;
    using Android.Support.V4.App;
    using Android.Widget;
    using Autofac;
    using Business.ApiClient;
    using Business.ApiClient.Interfaces;
    using Business.Bootstrapper;
    using Business.Modules.DrivingTimeMonitoring;
    using Business.Modules.DrivingTimeMonitoring.Interfaces;
    using Business.Modules.Tracking.ServiceConnections;
    using Resource = App.Resource;

    [Activity(Theme = "@android:style/Theme.Material.Light.NoActionBar")]
    public class DriverTrackingActivity : Activity
    {
        private bool _isTrackingInProgress;
        private bool _fineLocationPermissionGranted;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            var container = Bootstrapper.GetContainer();
            SetContentView(Resource.Layout.DriverTrackingView);

            Task.Run(delegate
            {
                var drivingTimeMonitoringService = container.Resolve<IDrivingTimeMonitoringService>();
                var timeLeftBeforeRestTextView = FindViewById<TextView>(Resource.Id.textTimeLeftBeforeRestValue);
                var drivingTimeOneDayTextView = FindViewById<TextView>(Resource.Id.textDrivingTimeOneDayValue);
                var drivingTimeSevenDaysTextView = FindViewById<TextView>(Resource.Id.textDrivingTimeSevenDaysValue);
                var restingTimeLeftBeforeNewTransportationTextView = FindViewById<TextView>(Resource.Id.textRestingTimeLeftBeforeNewTransportationValue);
                var timer = new Timer(5000);
                timer.Elapsed += async (sender, args) =>
                {
                    var results = await drivingTimeMonitoringService.GetDrivingTime();

                    RunOnUiThread(() =>
                    {
                        timeLeftBeforeRestTextView.SetText(
                            $"{results.DrivingTimeLeftTillNextRestRequired.TotalHours:0}:{results.DrivingTimeLeftTillNextRestRequired.Minutes:0}.{results.DrivingTimeLeftTillNextRestRequired.Seconds:0}",
                            TextView.BufferType.Normal);
                        drivingTimeOneDayTextView.SetText(
                            $"{results.DrivingTimePastDay.TotalHours:0}:{results.DrivingTimePastDay.Minutes:0}.{results.DrivingTimePastDay.Seconds:0}",
                            TextView.BufferType.Normal);
                        drivingTimeSevenDaysTextView.SetText(
                            $"{results.DrivingTimePastWeek.TotalHours:0}:{results.DrivingTimePastWeek.Minutes:0}.{results.DrivingTimePastWeek.Seconds:0}",
                            TextView.BufferType.Normal);
                        restingTimeLeftBeforeNewTransportationTextView.SetText(
                            $"{results.RestTimeLeftTillNextTransportationCanBeStarted.TotalHours:0}:{results.RestTimeLeftTillNextTransportationCanBeStarted.Minutes:0}.{results.RestTimeLeftTillNextTransportationCanBeStarted.Seconds:0}",
                            TextView.BufferType.Normal);
                    });
                };
                timer.Start();
            });

            var startShipmentButton = FindViewById<Button>(Resource.Id.buttonStartShipment);
            var stopShipmentButton = FindViewById<Button>(Resource.Id.buttonStopShipment);
            var createAlertButton = FindViewById<Button>(Resource.Id.buttonCreateAlert);
            var alertListButton = FindViewById<Button>(Resource.Id.buttonAlertList);
            startShipmentButton.Click += async delegate
            {
                if (CheckSelfPermission(Manifest.Permission.AccessFineLocation) == (int) Permission.Granted)
                {
                    var apiService = container.Resolve<IApiService>();
                    try
                    {
                        var transportationId = await apiService.PostTransporation();

                        _isTrackingInProgress = true;

                        var trackingServiceConnection = container.Resolve<TrackingServiceConnection>();
                        trackingServiceConnection.Binder.StartTransportationTracking(transportationId);
                        startShipmentButton.Enabled = false;
                        stopShipmentButton.Enabled = true;
                        createAlertButton.Enabled = true;
                        alertListButton.Enabled = true;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
                }
                else
                {
                    ActivityCompat.RequestPermissions(this, new[] { Manifest.Permission.AccessFineLocation }, 1);
                }
            };

            stopShipmentButton.Click += delegate
            {
                _isTrackingInProgress = false;
                var trackingServiceConnection = container.Resolve<TrackingServiceConnection>();
                trackingServiceConnection.Binder.StopShipmentTracking();
                startShipmentButton.Enabled = true;
                stopShipmentButton.Enabled = false;
                createAlertButton.Enabled = false;
                alertListButton.Enabled = false;
            };

            createAlertButton.Click += delegate
            {
                StartActivity(typeof(CreateAlertActivity));
            };

            alertListButton.Click += delegate
            {
                StartActivity(typeof(AlertListActivity));
            };
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            switch (requestCode)
            {
                case 1:
                {
                    // If request is cancelled, the result arrays are empty.
                    if (grantResults[0] == Permission.Granted)
                    {
                        _fineLocationPermissionGranted = true;
                    }
                    else
                    {
                        _fineLocationPermissionGranted = false;
                        // permission denied, boo! Disable the
                        // functionality that depends on this permission.
                    }
                    return;
                }
                // other 'case' lines to check for other
                // permissions this app might request
            }
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public override void OnBackPressed()
        {
            if (_isTrackingInProgress)
            {
                //var alertBuilder = new AlertDialog.Builder(this);
                //alertBuilder.SetMessage("Activity tracking is in process. Stop tracking before going back.");
                //alertBuilder.Show();
            }
            else
            {
                base.OnBackPressed();
            }
        }
    }
}