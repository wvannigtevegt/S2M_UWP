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
	public class ProfileViewModel : NotificationBase
	{
		private ObservableCollection<Chat> _chats = new ObservableCollection<Chat>();
		private ObservableCollection<CheckIn> _checkins = new ObservableCollection<CheckIn>();
		private ObservableCollection<Contact> _contacts = new ObservableCollection<Contact>();
		private int _nrOfCheckins;
		private Models.Profile _profile;
		private ObservableCollection<string> _tags = new ObservableCollection<string>();

		public ObservableCollection<Chat> Chats
		{
			get { return _chats; }
			set { SetProperty(ref _chats, value); }
		}

		public ObservableCollection<CheckIn> Checkins
		{
			get { return _checkins; }
			set { SetProperty(ref _checkins, value); }
		}

		public ObservableCollection<Contact> Contacts
		{
			get { return _contacts; }
			set { SetProperty(ref _contacts, value); }
		}

		public int NrOfCheckins
		{
			get
			{
				return _nrOfCheckins;
			}
			set { SetProperty(_nrOfCheckins, value, () => _nrOfCheckins = value); }
		}

		public Models.Profile Profile
		{
			get { return _profile; }
			set { SetProperty(_profile, value, () => _profile = value); }
		}

		public ObservableCollection<string> Tags
		{
			get { return _tags; }
			set { SetProperty(ref _tags, value); }
		}

		public async Task GetProfileCheckins()
		{
			var _cts = new CancellationTokenSource();
			CancellationToken token = _cts.Token;

			try
			{
				var checkinList = new ObservableCollection<CheckIn>();
				await CheckIn.GetProfileCheckInsAsync(token, checkinList);

				foreach (var checkin in checkinList)
				{
					if (!CheckIfCheckInExistsInList(checkin))
					{
						Checkins.Add(checkin);
					}
				}
				NrOfCheckins = Checkins.Count();
			}
			catch (Exception) { }
			finally
			{
				_cts = null;
			}
		}

		public async Task GetProfileChats()
		{
			var _cts = new CancellationTokenSource();
			CancellationToken token = _cts.Token;

			try
			{
				await Chat.GetProfileChatsAsync(Chats);
			}
			catch (Exception) { }
			finally
			{
				_cts = null;
			}
		}

		public async Task GetProfileContacts()
		{
			var _cts = new CancellationTokenSource();
			CancellationToken token = _cts.Token;

			try
			{
				await Contact.GetProfileContacts(token, Contacts);
			}
			catch (Exception) { }
			finally
			{
				_cts = null;
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
