using S2M.Common;
using S2M.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace S2M.ViewModel
{
	public class HomeViewModel : NotificationBase
	{
		private CheckIn _currentCheckin = null;
		private ObservableCollection<EventCalendar> _events = new ObservableCollection<EventCalendar>();
		private ObservableCollection<Location> _favoriteLocations = new ObservableCollection<Location>();
		private double _latitude;
		private double _longitude;
		private ObservableCollection<Location> _nearbyLocations = new ObservableCollection<Location>();
		private int _nrOfEvents;
		private int _nrOfFavoriteLocations;
		private int _nrOfNearbyLocations;
		private bool _showLocationFavoritesSpinner;
		private bool _showLocationNearbySpinner;

		public CheckIn CurrentCheckin
		{
			get { return _currentCheckin; }
			set { SetProperty(_currentCheckin, value, () => _currentCheckin = value); }
		}

		public ObservableCollection<EventCalendar> Events
		{
			get { return _events; }
			set { SetProperty(ref _events, value); }
		}

		public ObservableCollection<Location> FavoriteLocations
		{
			get { return _favoriteLocations; }
			set { SetProperty(ref _favoriteLocations, value); }
		}

		public double Latitude
		{
			get { return _latitude; }
			set { SetProperty(_latitude, value, () => _latitude = value); }
		}

		public double Longitude
		{
			get { return _longitude; }
			set { SetProperty(_longitude, value, () => _longitude = value); }
		}

		public ObservableCollection<Location> NearbyLocations
		{
			get { return _nearbyLocations; }
			set { SetProperty(ref _nearbyLocations, value); }
		}

		public int NrOfEvents
		{
			get { return _nrOfEvents; }
			set { SetProperty(_nrOfEvents, value, () => _nrOfEvents = value); }
		}

		public int NrOfFavoriteLocations
		{
			get { return _nrOfFavoriteLocations; }
			set { SetProperty(_nrOfFavoriteLocations, value, () => _nrOfFavoriteLocations = value); }
		}

		public int NrOfNearbyLocations
		{
			get { return _nrOfNearbyLocations; }
			set { SetProperty(_nrOfNearbyLocations, value, () => _nrOfNearbyLocations = value); }
		}

		public bool ShowLocationFavoritesSpinner
		{
			get
			{
				return _showLocationFavoritesSpinner;
			}
			set { SetProperty(_showLocationFavoritesSpinner, value, () => _showLocationFavoritesSpinner = value); }
		}

		public bool ShowLocationNearbySpinner
		{
			get
			{
				return _showLocationNearbySpinner;
			}
			set { SetProperty(_showLocationNearbySpinner, value, () => _showLocationNearbySpinner = value); }
		}

		public async Task<CheckIn> GetCurrentCheckin()
		{
			var _cts = new CancellationTokenSource();
			CancellationToken token = _cts.Token;

			try
			{
				return await CheckIn.GetCurrentCheckIn(token);
			}
			catch (Exception) { }
			finally
			{
				_cts = null;
			}

			return null;
		}

		public async Task GetNearbyLocations()
		{
			var _cts = new CancellationTokenSource();
			CancellationToken token = _cts.Token;

			ShowLocationNearbySpinner = true;

			try
			{
				const string searchTerm = "";
				const string workingOn = "";

				var accessStatus = await Geolocator.RequestAccessAsync();
				switch (accessStatus)
				{
					case GeolocationAccessStatus.Allowed:

						try
						{
							var geoposition = await GeoService.GetSinglePositionAsync(token);
							Latitude = geoposition.Point.Position.Latitude;
							Longitude = geoposition.Point.Position.Longitude;
						}
						catch (Exception) { }

						break;
					case GeolocationAccessStatus.Denied:

						break;
				}

				await Location.GetWorkspaceLocationsAsync(token, NearbyLocations, searchTerm, Latitude, Longitude, 250, workingOn, 1, 3);
				NrOfNearbyLocations = NearbyLocations.Count();
			}
			catch (Exception) { }
			finally
			{
				_cts = null;
				ShowLocationNearbySpinner = false;
			}
		}

		public async Task GetFavoriteLocations()
		{
			var _cts = new CancellationTokenSource();
			CancellationToken token = _cts.Token;

			ShowLocationFavoritesSpinner = true;

			try
			{
				const string searchTerm = "";
				const string workingOn = "";

				await Location.GetProfileFavoriteLocations(token, FavoriteLocations);
				NrOfFavoriteLocations = FavoriteLocations.Count();
			}
			catch (Exception) { }
			finally
			{
				_cts = null;
				ShowLocationFavoritesSpinner = false;
			}
		}

		public async Task GetTodaysEvents()
		{
			var _cts = new CancellationTokenSource();
			CancellationToken token = _cts.Token;

			try
			{
				await EventCalendar.GetEventsAsync(token, Events, DateTime.Now);
				NrOfEvents = Events.Count();
			}
			catch (Exception) { }
			finally
			{
				_cts = null;
				ShowLocationFavoritesSpinner = false;
			}
		}
	}
}
