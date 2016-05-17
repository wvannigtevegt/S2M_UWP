using S2M.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace S2M.ViewModel
{
	public class LocationDetailViewModel : NotificationBase
	{
		private bool _alreadyCheckedin;
		private ObservableCollection<CheckIn> _checkins = new ObservableCollection<CheckIn>();
		private string _checkinMessage;
		private ObservableCollection<LocationDay> _dates = new ObservableCollection<LocationDay>();
		private bool _enableButton;
		private TimeSpan _endTime;
		private bool _isBookable;
		private bool _isOpen;
		private int _nrOfCheckins;
		private ObservableCollection<CheckIn> _profileCheckIns = new ObservableCollection<CheckIn>();
		private string _searchKey { get; set; }
		private LocationDay _selectedDate = new LocationDay();
		private bool _showSpinner;
		private TimeSpan _startTime;

		private CancellationTokenSource _locationCheckinCts = new CancellationTokenSource();

		public ObservableCollection<CheckIn> Checkins
		{
			get { return _checkins; }
			set { SetProperty(ref _checkins, value); }
		}

		public string CheckinMessage
		{
			get { return _checkinMessage; }
			set { SetProperty(_checkinMessage, value, () => _checkinMessage = value); }
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

		public bool ShowSpinner
		{
			get
			{
				return _showSpinner;
			}
			set { SetProperty(_showSpinner, value, () => _showSpinner = value); }
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

		public async Task GetLocationCheckIns(int locationId)
		{
			//_locationCheckinCts.Cancel();
			_locationCheckinCts = new CancellationTokenSource();
			CancellationToken token = _locationCheckinCts.Token;

			try
			{
				Checkins.Clear();

				var newCheckIns = new ObservableCollection<CheckIn>();

				if (SelectedDate != null)
				{
					await CheckIn.GetCheckInsAsync(token, newCheckIns, SelectedDate.Date, locationId);

					foreach (var newCheckIn in newCheckIns)
					{
						if (!CheckIfCheckInExistsInList(newCheckIn))
						{
							Checkins.Add(newCheckIn);
						}
					}
					NrOfCheckins = Checkins.Count();
				}
			}
			catch (Exception ex) { }
			finally
			{
				_locationCheckinCts = null;
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
