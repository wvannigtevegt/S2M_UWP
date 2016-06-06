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
	public class CheckInFinalViewModel : NotificationBase
	{
		private CheckIn _currentCheckIn = new CheckIn();
		private bool _editWorkingOn { get; set; }
		private bool _isNewCheckin { get; set; }
		private int _nrOfOptions { get; set; }
		private ObservableCollection<Option> _optionList = new ObservableCollection<Option>();
		private ObservableCollection<CheckIn> _recommendedCheckins = new ObservableCollection<CheckIn>();
		private int _reservationId { get; set; }
		private bool _showCancelLink { get; set; }
		private bool _showCheckoutLink { get; set; }

		public CheckIn CurrentCheckin
		{
			get { return _currentCheckIn; }
			set { SetProperty(_currentCheckIn, value, () => _currentCheckIn = value); }
		}

		public bool EditWorkingOn
		{
			get { return _editWorkingOn; }
			set { SetProperty(_editWorkingOn, value, () => _editWorkingOn = value); }
		}

		public bool IsNewCheckin
		{
			get { return _isNewCheckin; }
			set { SetProperty(_isNewCheckin, value, () => _isNewCheckin = value); }
		}

		public int NrOfOptions
		{
			get { return _nrOfOptions; }
			set { SetProperty(_nrOfOptions, value, () => _nrOfOptions = value); }
		}

		public ObservableCollection<Option> OptionList
		{
			get { return _optionList; }
			set { SetProperty(ref _optionList, value); }
		}

		public ObservableCollection<CheckIn> RecommendedCheckins
		{
			get { return _recommendedCheckins; }
			set { SetProperty(ref _recommendedCheckins, value); }
		}

		public int ReservationId
		{
			get { return _reservationId; }
			set { SetProperty(_reservationId, value, () => _reservationId = value); }
		}

		public bool ShowCancelLink
		{
			get { return _showCancelLink; }
			set { SetProperty(_showCancelLink, value, () => _showCancelLink = value); }
		}

		public bool ShowCheckoutLink
		{
			get { return _showCheckoutLink; }
			set { SetProperty(_showCheckoutLink, value, () => _showCheckoutLink = value); }
		}

		public async Task GetCheckinRecommendations()
		{
			var _cts = new CancellationTokenSource();
			CancellationToken token = _cts.Token;

			try
			{
				ObservableCollection<CheckIn> checkins = new ObservableCollection<CheckIn>();

				await CheckIn.GetCheckinRecommendationsAsync(token, checkins, CurrentCheckin.LocationId, 
																Common.DateService.ConvertFromUnixTimestamp(CurrentCheckin.StartTimeStamp).Date, 
																CurrentCheckin.WorkingOn, 1, 3);

				//await CheckIn.GetCheckInsAsync(token, checkins, 
				//				Common.DateService.ConvertFromUnixTimestamp(CurrentCheckin.StartTimeStamp), 
				//				CurrentCheckin.LocationId, CurrentCheckin.EventId, "", 0, 0, 0, CurrentCheckin.WorkingOn, 1, 3, true);

				foreach(var recommendedCheckin in RecommendedCheckins)
				{
					var duplicateCheckIns = checkins.Where(c => c.Id == recommendedCheckin.Id);
					if (!duplicateCheckIns.Any())
					{
						RecommendedCheckins.Remove(recommendedCheckin);
					}
				}

				foreach(var checkin in checkins)
				{
					if (!CheckIfCheckInExistsInList(checkin))
					{
						RecommendedCheckins.Add(checkin);
					}
				}
			}
			catch (Exception) { }
			finally
			{
				_cts = null;
			}
		}

		public async Task GetLocationOptions()
		{
			var _cts = new CancellationTokenSource();
			CancellationToken token = _cts.Token;

			try
			{
				if (CurrentCheckin.ReservationId > 0)
				{
					await Option.GetLocationOptionsAsync(token, CurrentCheckin.ReservationId, OptionList);
					NrOfOptions = OptionList.Count();
				}
			}
			catch (Exception) { }
			finally
			{
				_cts = null;
			}
		}

		private bool CheckIfCheckInExistsInList(CheckIn checkin)
		{
			var checkIns = RecommendedCheckins.Where(c => c.Id == checkin.Id);
			if (checkIns.Any())
			{
				return true;
			}

			return false;
		}
	}
}
