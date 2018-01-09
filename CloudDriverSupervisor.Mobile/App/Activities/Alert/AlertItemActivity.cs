namespace Mobile.App.Activities.Alert
{
    using Android.App;
    using Android.OS;
    using Android.Widget;

    [Activity(Label = "Alert")]
    public class AlertItemActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.AlertItem);
            ActionBar.SetHomeButtonEnabled(true);
            ActionBar.SetDisplayHomeAsUpEnabled(true);
            // Create your application here
            var description = Intent.GetStringExtra("Description") ?? "Data not available";
            var descriptionTextView = FindViewById<TextView>(Resource.Id.textAlertItemDescriptionValue);
            descriptionTextView.Text = description;

            var createdAt = Intent.GetStringExtra("CreatedAt") ?? "Data not available";
            var createdDateTextView = FindViewById<TextView>(Resource.Id.textAlertItemCreatedDateValue);
            createdDateTextView.Text = createdAt;

            var severityLevel = Intent.GetStringExtra("Severitylevel") ?? "Data not available";
            var severityLevelTextView = FindViewById<TextView>(Resource.Id.textAlertItemSeverityLevelValue);
            severityLevelTextView.Text = severityLevel;

            var status = Intent.GetStringExtra("Status") ?? "Data not available";
            var statusTextView = FindViewById<TextView>(Resource.Id.textAlertItemStatusValue);
            statusTextView.Text = status;

            var type = Intent.GetStringExtra("Type") ?? "Data not available";
            var typeTextView = FindViewById<TextView>(Resource.Id.textAlertItemTypeValue);
            typeTextView.Text = type;
        }
    }
}