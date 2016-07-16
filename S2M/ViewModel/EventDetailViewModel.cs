using S2M.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace S2M.ViewModel
{
	public class EventDetailViewModel : NotificationBase
	{
		private bool _alreadyCheckedin;
		private ObservableCollection<CheckIn> _checkins = new ObservableCollection<CheckIn>();
		private bool _enableButton;
		private EventCalendar _event = new EventCalendar();
		private int _nrOfCheckins;
		private Profile _profile = new Profile();
		private bool _showCheckinButton;
		private bool _showCheckinsSpinner;
		private bool _showNoCheckins;
		private bool _showSpinner;

		public bool AlreadyCheckedin
		{
			get { return _alreadyCheckedin; }
			set { SetProperty(_alreadyCheckedin, value, () => _alreadyCheckedin = value); }
		}

		public ObservableCollection<CheckIn> Checkins
		{
			get { return _checkins; }
			set { SetProperty(ref _checkins, value); }
		}

		public bool EnableButton
		{
			get { return _enableButton; }
			set { SetProperty(_enableButton, value, () => _enableButton = value); }
		}

		public EventCalendar Event
		{
			get { return _event; }
			set { SetProperty(_event, value, () => _event = value); }
		}

		public int NrOfCheckins
		{
			get
			{
				return _nrOfCheckins;
			}
			set { SetProperty(_nrOfCheckins, value, () => _nrOfCheckins = value); }
		}

		public Profile Profile
		{
			get { return _profile; }
			set { SetProperty(_profile, value, () => _profile = value); }
		}

		public bool ShowCheckinButton
		{
			get
			{
				return _showCheckinButton;
			}
			set { SetProperty(_showCheckinButton, value, () => _showCheckinButton = value); }
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

		public bool ShowSpinner
		{
			get
			{
				return _showSpinner;
			}
			set { SetProperty(_showSpinner, value, () => _showSpinner = value); }
		}

		public async Task GetEventCheckIns()
		{
			var _cts = new CancellationTokenSource();
			CancellationToken token = _cts.Token;

			ShowCheckinsSpinner = true;

			try
			{
				Checkins.Clear();

				var newCheckIns = new ObservableCollection<CheckIn>();


				await CheckIn.GetCheckInsEventDateAsync(token, newCheckIns, Event);

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
			catch (Exception ex) { }
			finally
			{
				_cts = null;
				ShowCheckinsSpinner = false;
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
