namespace Mobile.Business.Modules.Tracking
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Android.Locations;
    using Android.OS;
    using Entities;
    using Object = Java.Lang.Object;

    public class LocationListener : Object, ILocationListener
    {
        public LocationListener()
        {
            savedLocations = new List<CapturedLocation>();
        }

        private List<CapturedLocation> savedLocations { get; }

        public void OnLocationChanged(Location location)
        {
            var dateTime = DateTime.MinValue.AddYears(1969).AddMilliseconds(location.Time);
            savedLocations.Add(new CapturedLocation
            {
                Altitude = location.Latitude,
                Longitude = location.Longitude,
                CapturedDateTimeUtc = dateTime
            });
        }

        public void OnProviderDisabled(string provider)
        {
            throw new NotImplementedException();
        }

        public void OnProviderEnabled(string provider)
        {
            throw new NotImplementedException();
        }

        public void OnStatusChanged(string provider, Availability status, Bundle extras)
        {
            //throw new System.NotImplementedException();
        }

        public List<CapturedLocation> GetCapturedLocations()
        {
            var tempList = savedLocations.ToList();
            savedLocations.Clear();
            return tempList;
        }
    }
}