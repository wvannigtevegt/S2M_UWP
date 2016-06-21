using S2M.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S2M.ViewModel
{
	public class CheckInDetailViewModel : NotificationBase
	{
		private Chat _chatObject = new Chat();
		private CheckIn _selectedCheckIn = new CheckIn();
		private PublicProfile _publicProfile = new PublicProfile();
		private ObservableCollection<string> _tags = new ObservableCollection<string>();
		private int _tagCount;

		public Chat ChatObject
		{
			get { return _chatObject; }
			set { SetProperty(_chatObject, value, () => _chatObject = value); }
		}

		public PublicProfile PublicProfile
		{
			get { return _publicProfile; }
			set { SetProperty(_publicProfile, value, () => _publicProfile = value); }
		}

		public CheckIn SelectedCheckin
		{
			get { return _selectedCheckIn; }
			set { SetProperty(_selectedCheckIn, value, () => _selectedCheckIn = value); }
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
			PublicProfile = await Models.PublicProfile.GetProfileByProfileId(SelectedCheckin.ProfileId);
		}
	}
}
