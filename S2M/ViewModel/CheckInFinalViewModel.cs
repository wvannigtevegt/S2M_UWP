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
		private bool _editWorkingOn;
		private bool _isNewCheckin;
		private int _nrOfOptions;
		private ObservableCollection<Option> _optionList = new ObservableCollection<Option>();
		private ObservableCollection<CheckIn> _recommendedCheckins = new ObservableCollection<CheckIn>();
		private int _reservationId;
		private bool _showCancelLink;
		private bool _showCheckinsSpinner;
		private bool _showCheckoutLink;
		private bool _showNoCheckins;

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

		public bool ShowCheckinsSpinner
		{
			get
			{
				return _showCheckinsSpinner;
			}
			set { SetProperty(_showCheckinsSpinner, value, () => _showCheckinsSpinner = value); }
		}

		public bool ShowCheckoutLink
		{
			get { return _showCheckoutLink; }
			set { SetProperty(_showCheckoutLink, value, () => _showCheckoutLink = value); }
		}

		public bool ShowNoCheckins
		{
			get
			{
				return _showNoCheckins;
			}
			set { SetProperty(_showNoCheckins, value, () => _showNoCheckins = value); }
		}

		public async Task GetCheckinRecommendations()
		{
			var _cts = new CancellationTokenSource();
			CancellationToken token = _cts.Token;

			RecommendedCheckins.Clear();
			ShowCheckinsSpinner = true;
			ShowNoCheckins = false;

			try
			{
				ObservableCollection<CheckIn> checkins = new ObservableCollection<CheckIn>();

				await CheckIn.GetCheckinRecommendationsAsync(token, checkins, CurrentCheckin.LocationId, 
																Common.DateService.ConvertFromUnixTimestamp(CurrentCheckin.StartTimeStamp).Date, 
																CurrentCheckin.WorkingOn, 1, 3);

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

				if (RecommendedCheckins.Count() == 0)
				{
					ShowNoCheckins = true;
				}
			}
			catch (Exception) { }
			finally
			{
				_cts = null;
				ShowCheckinsSpinner = false;
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
