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
		private ObservableCollection<LocationDay> _dates = new ObservableCollection<LocationDay>();
		private TimeSpan _endTime;
		private bool _isBookable;
		private ObservableCollection<CheckIn> _profileCheckIns = new ObservableCollection<CheckIn>();
		private LocationDay _selectedDate = new LocationDay();
		private TimeSpan _startTime;

		public ObservableCollection<LocationDay> Dates
		{
			get { return _dates; }
			set { SetProperty(ref _dates, value); }
		}

		public TimeSpan EndTime
		{
			get { return _endTime; }
			set { SetProperty(_endTime, value, () => _endTime = value); }
		} 

		public bool IsBookable
		{
			get { return _isBookable; }
			set { SetProperty(_isBookable, value, () => _isBookable = value); }
		}

		public ObservableCollection<CheckIn> ProfileCheckIns
		{
			get { return _profileCheckIns; }
			set { SetProperty(ref _profileCheckIns, value); }
		}

		public LocationDay SelectedDate
		{
			get { return _selectedDate; }
			set { SetProperty(_selectedDate, value, () => _selectedDate = value); }
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
	}
}
