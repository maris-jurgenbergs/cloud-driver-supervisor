namespace Mobile.App.Activities
{
    using System;
    using System.Net;
    using Android.App;
    using Android.Content;
    using Android.Net;
    using Android.OS;
    using Android.Widget;
    using Autofac;
    using Business.Bootstrapper;
    using Business.Modules;
    using Business.Modules.Authentication;
    using Business.Modules.Authentication.Interfaces;
    using Business.Modules.Tracking;
    using Business.Modules.Tracking.ServiceConnections;
    using Microsoft.IdentityModel.Clients.ActiveDirectory;
    using Resource = App.Resource;

    [Activity(Label = "CDSm", MainLauncher = true, Icon = "@drawable/cdsm",
        Theme = "@android:style/Theme.Material.NoActionBar")]
    public class MainActivity : Activity
    {
        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            ServicePointManager.ServerCertificateValidationCallback +=
                (sender, certificate, chain, sslPolicyErrors) => true;
            SetContentView(Resource.Layout.Main);

            var button = FindViewById<Button>(Resource.Id.buttonLoginUser);
            var connectivityManager = (ConnectivityManager) GetSystemService(ConnectivityService);
            var networkInfo = connectivityManager.ActiveNetworkInfo;
            var textViewLoginResult = FindViewById<TextView>(Resource.Id.textViewLoginResult);

            var container = Bootstrapper.GetContainer();
            var authenticationService = container.Resolve<IAuthenticationService>();
            var authorizationService = container.Resolve<IAuthorizationService>();

            if (networkInfo != null && networkInfo.IsConnected)
            {
                try
                {
                    var progress = new ProgressDialog(this) { Indeterminate = true };
                    progress.SetProgressStyle(ProgressDialogStyle.Spinner);
                    progress.SetMessage("Authenticating... Please wait...");
                    progress.SetCancelable(false);
                    progress.Show();
                    if (await authenticationService.AuthenticateUserAsync(
                            new PlatformParameters(this, false, PromptBehavior.RefreshSession))
                        && await authorizationService.IsUserInDriverRole())
                    {
                        StartActivity(typeof(DriverTrackingActivity));
                        Finish();
                    }
                    else
                    {
                        progress.Hide();
                    }
                }
                catch (AdalServiceException e) when (e.ErrorCode == "authentication_canceled")
                {
                    textViewLoginResult.Text = e.Message;
                }
            }

            button.Click += async delegate
            {
                networkInfo = connectivityManager.ActiveNetworkInfo;
                if (networkInfo != null && networkInfo.IsConnected)
                {
                    try
                    {
                        if (await authenticationService.AuthenticateUserAsync(
                                new PlatformParameters(this, false, PromptBehavior.RefreshSession))
                            && await authorizationService.IsUserInDriverRole())
                        {
                            StartActivity(typeof(DriverTrackingActivity));
                            Finish();
                        }
                    }
                    catch (AdalServiceException e) when (e.ErrorCode == "authentication_canceled")
                    {
                        textViewLoginResult.Text = e.Message;
                    }
                }
                else
                {
                    textViewLoginResult.Text = "Internet connection is not available";
                }
            };
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            AuthenticationAgentContinuationHelper.SetAuthenticationAgentContinuationEventArgs(requestCode, resultCode,
                data);
        }

        protected override void OnStart()
        {
            var container = Bootstrapper.GetContainer();
            var trackingServiceConnection = container.Resolve<TrackingServiceConnection>();
            trackingServiceConnection.NavigationActivity = this;

            var trackingServiceIntent = new Intent(this, typeof(TrackingService));
            try
            {
                BindService(trackingServiceIntent, trackingServiceConnection, Bind.AutoCreate);
            }
            catch (Exception e)
            {
                throw;
            }

            base.OnStart();
        }
    }
}