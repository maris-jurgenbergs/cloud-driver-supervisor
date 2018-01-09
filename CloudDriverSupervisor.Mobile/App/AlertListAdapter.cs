namespace Mobile.App
{
    using System;
    using System.Collections.Generic;
    using Android.App;
    using Android.Views;
    using Android.Widget;
    using Business.Modules.Alert.Entities;

    public class AlertListAdapter : BaseAdapter<AlertResultDto>
    {
        private readonly Activity _context;
        private readonly IList<AlertResultDto> _items;

        public AlertListAdapter(Activity context, IList<AlertResultDto> items)
        {
            this._context = context;
            this._items = items;
        }

        public override AlertResultDto this[int position] => _items[position];

        public override int Count => _items.Count;

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var item = _items[position];
            var view = convertView;
            if (view == null) // no view to re-use, create new
            {
                view = _context.LayoutInflater.Inflate(Resource.Layout.AlertListItem, null);
            }

            var alertType = Utils.GetEnumDescription(item.Type);
            view.FindViewById<TextView>(Resource.Id.alertTypeListItem).Text = alertType;
            view.FindViewById<TextView>(Resource.Id.alertCreatedDateListItem).Text = DateTime.MinValue
                .AddSeconds(item.CreatedAt).AddYears(1969).ToLocalTime().ToString("dd/MM/yyyy HH:mm:ss");
            return view;
        }
    }
}