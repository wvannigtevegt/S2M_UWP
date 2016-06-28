using S2M.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace S2M.ViewModel
{
	public class LocationDetailViewModel : NotificationBase
	{
		private bool _alreadyCheckedin;
		private string _availavilityMessage;
		private ObservableCollection<AvailableUnit> _availableUnits = new ObservableCollection<AvailableUnit>();
		private ObservableCollection<CheckIn> _checkins = new ObservableCollection<CheckIn>();
		private ObservableCollection<LocationDay> _dates = new ObservableCollection<LocationDay>();
		private bool _enableButton;
		private TimeSpan _endTime;
		private ObservableCollection<EventCalendar> _events = new ObservableCollection<EventCalendar>();
		private bool _isBookable;
		private bool _isBookmarked;
		private bool _isOpen;
		private Location _location = new Location();
		private LocationText _locationDescription;
		private int _nrOfCheckins;
		private ObservableCollection<CheckIn> _profileCheckIns = new ObservableCollection<CheckIn>();
		private string _searchKey;
		private LocationDay _selectedDate = new LocationDay();
		private bool _showDateTimeCheckin;
		private bool _showCheckinsSpinner;
		private bool _showNoCheckins;
		private bool _showSpinner;
		private bool _showWorkspaceSelection;
		private TimeSpan _startTime;

		private CancellationTokenSource _locationCheckinCts = new CancellationTokenSource();

		public ObservableCollection<AvailableUnit> AvailableUnits
		{
			get { return _availableUnits; }
			set { SetProperty(ref _availableUnits, value); }
		}

		public ObservableCollection<CheckIn> Checkins
		{
			get { return _checkins; }
			set { SetProperty(ref _checkins, value); }
		}

		public string AvailabilityMessage
		{
			get { return _availavilityMessage; }
			set { SetProperty(_availavilityMessage, value, () => _availavilityMessage = value); }
		}

		public ObservableCollection<LocationDay> Dates
		{
			get { return _dates; }
			set { SetProperty(ref _dates, value); }
		}

		public bool EnableButton
		{
			get { return _enableButton; }
			set { SetProperty(_enableButton, value, () => _enableButton = value); }
		}

		public TimeSpan EndTime
		{
			get { return _endTime; }
			set { SetProperty(_endTime, value, () => _endTime = value); }
		}

		public ObservableCollection<EventCalendar> Events
		{
			get { return _events; }
			set { SetProperty(ref _events, value); }
		}

		public bool AlreadyCheckedin
		{
			get { return _alreadyCheckedin; }
			set { SetProperty(_alreadyCheckedin, value, () => _alreadyCheckedin = value); }
		}

		public bool IsBookable
		{
			get { return _isBookable; }
			set { SetProperty(_isBookable, value, () => _isBookable = value); }
		}

		public bool IsBookmarked
		{
			get { return _isBookmarked; }
			set { SetProperty(_isBookmarked, value, () => _isBookmarked = value); }
		}

		public bool IsOpen
		{
			get { return _isOpen; }
			set { SetProperty(_isOpen, value, () => _isOpen = value); }
		}

		public int NrOfCheckins
		{
			get {
				return _nrOfCheckins;
			}
			set { SetProperty(_nrOfCheckins, value, () => _nrOfCheckins = value); }
		}

		public Location Location
		{
			get { return _location; }
			set { SetProperty(_location, value, () => _location = value); }
		}

		public LocationText LocationDescription
		{
			get { return _locationDescription; }
			set { SetProperty(_locationDescription, value, () => _locationDescription = value); }
		}

		public ObservableCollection<CheckIn> ProfileCheckIns
		{
			get { return _profileCheckIns; }
			set { SetProperty(ref _profileCheckIns, value); }
		}

		public string SearchKey
		{
			get { return _searchKey; }
			set { SetProperty(_searchKey, value, () => _searchKey = value); }
		}


		public LocationDay SelectedDate
		{
			get { return _selectedDate; }
			set { SetProperty(_selectedDate, value, () => _selectedDate = value); }
		}

		public bool ShowNoCheckins
		{
			get
			{
				return _showNoCheckins;
			}
			set { SetProperty(_showNoCheckins, value, () => _showNoCheckins = value); }
		}

		public bool ShowCheckinsSpinner
		{
			get
			{
				return _showCheckinsSpinner;
			}
			set { SetProperty(_showCheckinsSpinner, value, () => _showCheckinsSpinner = value); }
		}

		public bool ShowDateTimeCheckin
		{
			get
			{
				return _showDateTimeCheckin;
			}
			set { SetProperty(_showDateTimeCheckin, value, () => _showDateTimeCheckin = value); }
		}

		public bool ShowSpinner
		{
			get
			{
				return _showSpinner;
			}
			set { SetProperty(_showSpinner, value, () => _showSpinner = value); }
		}

		public bool ShowWorkspaceSelection
		{
			get
			{
				return _showWorkspaceSelection;
			}
			set { SetProperty(_showWorkspaceSelection, value, () => _showWorkspaceSelection = value); }
		}

		public TimeSpan StartTime
		{
			get { return _startTime; }
			set { SetProperty(_startTime, value, () => _startTime = value); }
		}

		public async Task GetProfileCheckIns()
		{
			var _cts = new CancellationTokenSource();
			CancellationToken token = _cts.Token;

			try
			{
				await CheckIn.GetProfileCheckInsAsync(token, ProfileCheckIns);
			}
			catch (Exception ex) { }
			finally
			{
				_cts = null;
			}
		}

		public async Task GetLocationCheckIns()
		{
			//_locationCheckinCts.Cancel();
			_locationCheckinCts = new CancellationTokenSource();
			CancellationToken token = _locationCheckinCts.Token;

			ShowCheckinsSpinner = true;

			try
			{
				Checkins.Clear();

				var newCheckIns = new ObservableCollection<CheckIn>();

				if (SelectedDate != null)
				{
					await CheckIn.GetCheckInsAsync(token, newCheckIns, SelectedDate.Date, Location.Id);

					foreach (var newCheckIn in newCheckIns)
					{
						if (!CheckIfCheckInExistsInList(newCheckIn))
						{
							Checkins.Add(newCheckIn);
						}
					}
					NrOfCheckins = Checkins.Count();
					if (NrOfCheckins == 0)
					{
						ShowNoCheckins = true;
					}
					else
					{
						ShowNoCheckins = false;
					}
				}
			}
			catch (Exception ex) { }
			finally
			{
				_locationCheckinCts = null;
				ShowCheckinsSpinner = false;
			}
		}

		public async Task GetLocationEvents()
		{
			var _cts = new CancellationTokenSource();
			CancellationToken token = _cts.Token;

			try
			{
				await EventCalendar.GetEventsAsync(token, Events, DateTime.Now, Location.Id, "");
			}
			catch (Exception ex) { }
			finally
			{
				_cts = null;
			}
		}

		public async Task GetLocationDescription()
		{
			var _locationDescriptionCts = new CancellationTokenSource();
			CancellationToken token = _locationDescriptionCts.Token;

			try
			{
				var description = await LocationText.GetLocationDescriptionAsync(token, Location.Id);
				if (description != null)
				{
					LocationDescription = description;
				}
			}
			catch (Exception ex) { }
			finally
			{
				_locationDescriptionCts = null;
			}
		}

		private bool CheckIfCheckInExistsInList(CheckIn checkin)
		{
			var checkIns = Checkins.Where(c => c.Id == checkin.Id);
			if (checkIns.Any())
			{
				return true;
			}

			return false;
		}
	}
}
