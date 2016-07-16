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
		private double _latitude;
		private CheckIn _lastCheckin = null;
		private double _longitude;
		private int _nrOfEvents;
		private int _nrOfSuggestedLocations;
		private bool _showFavoriteLocations;
		private bool _showNearbyLocations;
		private bool _showLocationSuggestionsSpinner;
		private ObservableCollection<Location> _suggestedLocations = new ObservableCollection<Location>();

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

		public double Latitude
		{
			get { return _latitude; }
			set { SetProperty(_latitude, value, () => _latitude = value); }
		}

		public CheckIn LastCheckin
		{
			get { return _lastCheckin; }
			set { SetProperty(_lastCheckin, value, () => _lastCheckin = value); }
		}

		public double Longitude
		{
			get { return _longitude; }
			set { SetProperty(_longitude, value, () => _longitude = value); }
		}

		public int NrOfEvents
		{
			get { return _nrOfEvents; }
			set { SetProperty(_nrOfEvents, value, () => _nrOfEvents = value); }
		}

		public int NrOfSuggestedLocations
		{
			get { return _nrOfSuggestedLocations; }
			set { SetProperty(_nrOfSuggestedLocations, value, () => _nrOfSuggestedLocations = value); }
		}

		public bool ShowLocationSuggestionsSpinner
		{
			get
			{
				return _showLocationSuggestionsSpinner;
			}
			set { SetProperty(_showLocationSuggestionsSpinner, value, () => _showLocationSuggestionsSpinner = value); }
		}

		public bool ShowFavoriteLocations
		{
			get
			{
				return _showFavoriteLocations;
			}
			set { SetProperty(_showFavoriteLocations, value, () => _showFavoriteLocations = value); }
		}

		public bool ShowNearbyLocations
		{
			get
			{
				return _showNearbyLocations;
			}
			set { SetProperty(_showNearbyLocations, value, () => _showNearbyLocations = value); }
		}

		public ObservableCollection<Location> SuggestedLocations
		{
			get { return _suggestedLocations; }
			set { SetProperty(ref _suggestedLocations, value); }
		}

		public async Task GetCurrentCheckin()
		{
			var _cts = new CancellationTokenSource();
			CancellationToken token = _cts.Token;

			try
			{
				CurrentCheckin = await CheckIn.GetProfileCurrentCheckIn(token);
			}
			catch (Exception) { }
			finally
			{
				_cts = null;
			}
		}

		public async Task GetLastCheckin()
		{
			var _cts = new CancellationTokenSource();
			CancellationToken token = _cts.Token;

			try
			{
				LastCheckin = await CheckIn.GetProfileLastCheckIn(token);
			}
			catch (Exception) { }
			finally
			{
				_cts = null;
			}
		}

		public async Task GetNearbyLocations()
		{
			var _cts = new CancellationTokenSource();
			CancellationToken token = _cts.Token;

			ShowLocationSuggestionsSpinner = true;

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

				await Location.GetWorkspaceLocationsAsync(token, SuggestedLocations, searchTerm, Latitude, Longitude, 250, workingOn, 1, 3);
				NrOfSuggestedLocations = SuggestedLocations.Count();
			}
			catch (Exception) { }
			finally
			{
				_cts = null;
				ShowLocationSuggestionsSpinner = false;
			}
		}

		public async Task GetFavoriteLocations()
		{
			var _cts = new CancellationTokenSource();
			CancellationToken token = _cts.Token;

			ShowLocationSuggestionsSpinner = true;

			try
			{
				const string searchTerm = "";
				const string workingOn = "";

				await Location.GetProfileFavoriteLocations(token, SuggestedLocations);
				NrOfSuggestedLocations = SuggestedLocations.Count();
			}
			catch (Exception) { }
			finally
			{
				_cts = null;
				ShowLocationSuggestionsSpinner = false;
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
			}
		}
	}
}
