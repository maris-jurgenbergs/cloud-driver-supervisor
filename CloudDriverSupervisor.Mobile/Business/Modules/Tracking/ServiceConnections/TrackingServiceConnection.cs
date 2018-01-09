namespace Mobile.Business.Modules.Tracking.ServiceConnections
{
    using Android.App;
    using Android.Content;
    using Android.OS;
    using Binders;
    using Java.Lang;

    public class TrackingServiceConnection : Object, IServiceConnection
    {
        public Activity NavigationActivity { get; set; }

        public bool IsConnected { get; private set; }

        public TrackingBinder Binder { get; private set; }

        public void OnServiceConnected(ComponentName name, IBinder service)
        {
            Binder = service as TrackingBinder;
            IsConnected = Binder != null;
        }

        public void OnServiceDisconnected(ComponentName name)
        {
            IsConnected = false;
            Binder = null;
        }
    }
}