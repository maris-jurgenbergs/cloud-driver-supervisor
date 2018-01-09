namespace Mobile.App.Activities.Alert
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Android.App;
    using Android.Content;
    using Android.OS;
    using Android.Views;
    using Android.Widget;
    using Autofac;
    using Business.Bootstrapper;
    using Business.Modules.Alert;
    using Business.Modules.Alert.Entities;
    using Business.Modules.Alert.Interfaces;

    [Activity(Label = "Alert List")]
    public class AlertListActivity : Activity
    {
        private IList<AlertResultDto> _currentAlerts;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.AlertList);

            ActionBar.SetHomeButtonEnabled(true);
            ActionBar.SetDisplayHomeAsUpEnabled(true);
            // Create your application here
            Task.Run(async () =>
            {
                var container = Bootstrapper.GetContainer();
                var alertService = container.Resolve<IAlertService>();
                var alerts = await alertService.GetAlerts();
                _currentAlerts = alerts;
            RunOnUiThread(() =>
                {
                    var listView = FindViewById<ListView>(Resource.Id.alertListView);
                    listView.Adapter = new AlertListAdapter(this, alerts);

                    listView.ItemClick += delegate (object sender, AdapterView.ItemClickEventArgs args)
                    {
                        var currentAlert = _currentAlerts[args.Position];
                        var createdTime = DateTime.MinValue.AddSeconds(currentAlert.CreatedAt).AddYears(1969).ToLocalTime()
                            .ToString("dd/MM/yyyy HH:mm:ss");
                        var activity = new Intent(this, typeof(AlertItemActivity));
                        activity.PutExtra("Description", currentAlert.Description);
                        activity.PutExtra("CreatedAt", createdTime);
                        activity.PutExtra("Severitylevel", Utils.GetEnumDescription(currentAlert.SeverityLevel));
                        activity.PutExtra("Status", Utils.GetEnumDescription(currentAlert.Status));
                        activity.PutExtra("Type", Utils.GetEnumDescription(currentAlert.Type));
                        StartActivity(activity);
                    };
                });
            });
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    Finish();
                    return true;

                default:
                    return base.OnOptionsItemSelected(item);
            }
        }
    }
}