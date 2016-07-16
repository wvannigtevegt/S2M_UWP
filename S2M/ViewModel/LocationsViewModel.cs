using S2M.Models;
using System.Collections.ObjectModel;

namespace S2M.ViewModel
{
	public class LocationsViewModel : NotificationBase
	{
		private bool _deviceIsOffline;
		private double _latitude;
		private ObservableCollection<Location> _locations = new ObservableCollection<Location>();
		private double _longitude;
		private bool _showLocationDistance;

		public bool DeviceIsOffline
		{
			get { return _deviceIsOffline; }
			set { SetProperty(_deviceIsOffline, value, () => _deviceIsOffline = value); }
		}

		public double Latitude
		{
			get { return _latitude; }
			set { SetProperty(_latitude, value, () => _latitude = value); }
		}

		public ObservableCollection<Location> Locations
		{
			get { return _locations; }
			set { SetProperty(ref _locations, value); }
		}

		public double Longitude
		{
			get { return _longitude; }
			set { SetProperty(_longitude, value, () => _longitude = value); }
		}

		public bool ShowLocationDistance
		{
			get { return _showLocationDistance; }
			set { SetProperty(_showLocationDistance, value, () => _showLocationDistance = value); }
		}
	}
}
