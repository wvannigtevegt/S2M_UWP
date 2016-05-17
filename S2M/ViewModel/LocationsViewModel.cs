using S2M.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S2M.ViewModel
{
	public class LocationsViewModel : NotificationBase
	{
		private bool _deviceIsOffline;
		private double _latitude;
		private ObservableCollection<Location> _locationList = new ObservableCollection<Location>();
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

		public ObservableCollection<Location> LocationList
		{
			get { return _locationList; }
			set { SetProperty(ref _locationList, value); }
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
