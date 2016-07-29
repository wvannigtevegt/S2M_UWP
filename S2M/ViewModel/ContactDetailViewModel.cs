using S2M.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S2M.ViewModel
{
	public class ContactDetailViewModel : NotificationBase
	{
		private Chat _chatObject = new Chat();
		private CheckIn _currentCheckIn = null;
		private PublicProfile _publicProfile = new PublicProfile();
		private Contact _selectedContact = new Contact();
		private ObservableCollection<string> _tags = new ObservableCollection<string>();
		private int _tagCount;

		public Chat ChatObject
		{
			get { return _chatObject; }
			set { SetProperty(_chatObject, value, () => _chatObject = value); }
		}

		public CheckIn CurrentCheckIn
		{
			get { return _currentCheckIn; }
			set { SetProperty(_currentCheckIn, value, () => _currentCheckIn = value); }
		}

		public PublicProfile PublicProfile
		{
			get { return _publicProfile; }
			set { SetProperty(_publicProfile, value, () => _publicProfile = value); }
		}

		public Contact SelectedContact
		{
			get { return _selectedContact; }
			set { SetProperty(_selectedContact, value, () => _selectedContact = value); }
		}

		public ObservableCollection<string> Tags
		{
			get { return _tags; }
			set { SetProperty(ref _tags, value); }
		}

		public int TagCount
		{
			get
			{
				return _tagCount;
			}
			set { SetProperty(_tagCount, value, () => _tagCount = value); }
		}

		public async Task GetPublicProfile()
		{
			PublicProfile = await PublicProfile.GetProfileByProfileId(SelectedContact.ProfileId);
		}
	}
}
