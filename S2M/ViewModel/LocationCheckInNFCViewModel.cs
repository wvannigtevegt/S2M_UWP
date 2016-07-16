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
	public class LocationCheckInNFCViewModel : NotificationBase
	{
		private ObservableCollection<AvailableUnit> _availableUnits = new ObservableCollection<AvailableUnit>();
		private CheckIn _currentCheckin = null;
		private bool _isOpen;
		private Location _location = new Location();
		private int _locationId;
		private OpeningHour _locationOpeningHours { get; set; }
		private bool _showCheckinsSpinner;

		public ObservableCollection<AvailableUnit> AvailableUnits
		{
			get { return _availableUnits; }
			set { SetProperty(ref _availableUnits, value); }
		}

		public CheckIn CurrentCheckin
		{
			get { return _currentCheckin; }
			set { SetProperty(_currentCheckin, value, () => _currentCheckin = value); }
		}

		public bool IsOpen
		{
			get { return _isOpen; }
			set { SetProperty(_isOpen, value, () => _isOpen = value); }
		}

		public Location Location
		{
			get { return _location; }
			set { SetProperty(_location, value, () => _location = value); }
		}

		public int LocationId
		{
			get { return _locationId; }
			set { SetProperty(_locationId, value, () => _locationId = value); }
		}

		public OpeningHour LocationOpeningHours
		{
			get { return _locationOpeningHours; }
			set { SetProperty(_locationOpeningHours, value, () => _locationOpeningHours = value); }
		}

		public bool ShowCheckinsSpinner
		{
			get
			{
				return _showCheckinsSpinner;
			}
			set { SetProperty(_showCheckinsSpinner, value, () => _showCheckinsSpinner = value); }
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
		public async Task GetLocationAsync()
		{
			var _cts = new CancellationTokenSource();
			CancellationToken token = _cts.Token;

			ShowCheckinsSpinner = true;

			try
			{
				Location = await Location.GetLocationById(token, LocationId);
			}
			catch (Exception) { }
			finally
			{
				_cts = null;
				ShowCheckinsSpinner = false;
			}
		}

		public async Task GetLocationOpeningHours()
		{
			var _cts = new CancellationTokenSource();
			CancellationToken token = _cts.Token;

			try
			{
				LocationOpeningHours = await OpeningHour.GetLocationOpeningHourssAsync(token, LocationId, DateTime.Now);
			}
			catch (Exception) { }
			finally
			{
				_cts = null;
			}
		}
	}
}
