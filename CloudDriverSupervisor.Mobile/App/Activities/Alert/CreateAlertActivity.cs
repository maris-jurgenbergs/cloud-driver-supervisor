namespace Mobile.App.Activities.Alert
{
    using System.Collections.Generic;
    using System.Linq;
    using Android.App;
    using Android.OS;
    using Android.Views;
    using Android.Widget;
    using Autofac;
    using Business.Bootstrapper;
    using Business.Modules.Alert;
    using Business.Modules.Alert.Interfaces;

    [Activity(Label = "Create Alert")]
    public class CreateAlertActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            var container = Bootstrapper.GetContainer();
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.CreateAlert);

            ActionBar.SetHomeButtonEnabled(true);
            ActionBar.SetDisplayHomeAsUpEnabled(true);
            // Create your application here

            var alertTypes = new Dictionary<string, int>
            {
                { "Road accident", 1 },
                { "Health issue", 2 },
                { "Assault", 3 },
                { "Heavy traffic", 4 },
                { "Other", 5 }
            };

            var adapter = new ArrayAdapter<string>(this,
                Android.Resource.Layout.SimpleSpinnerItem, alertTypes.Select(pair => pair.Key).ToList());

            var spinner = FindViewById<Spinner>(Resource.Id.spinnerAlertType);
            spinner.Adapter = adapter;
            var descriptionText = FindViewById<EditText>(Resource.Id.descriptionTextValue);
            var createAlertButton = FindViewById<Button>(Resource.Id.buttonConfirmCreateAlert);
            createAlertButton.Click += async delegate
            {
                var alertService = container.Resolve<IAlertService>();
                var selectedType = spinner.SelectedItem.ToString();
                await alertService.CreateAlert(alertTypes[selectedType], descriptionText.Text);
                Finish();
            };
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