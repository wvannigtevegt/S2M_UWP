using S2M.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S2M.ViewModel
{
	public class RegisterViewModel : NotificationBase
	{
		private string _email;
		private bool _enableButton;
		private string _firstName;
		private string _lastName;
		private string _password;
		private Profile _profile;
		private bool _showSpinner;
		private ObservableCollection<string> _tags = new ObservableCollection<string>();

		public string Email
		{
			get { return _email; }
			set { SetProperty(_email, value, () => _email = value); }
		}

		public bool EnableButton
		{
			get { return _enableButton; }
			set { SetProperty(_enableButton, value, () => _enableButton = value); }
		}

		public string FirstName
		{
			get { return _firstName; }
			set { SetProperty(_firstName, value, () => _firstName = value); }
		}

		public string LastName
		{
			get { return _lastName; }
			set { SetProperty(_lastName, value, () => _lastName = value); }
		}

		public string Password
		{
			get { return _password; }
			set { SetProperty(_password, value, () => _password = value); }
		}

		public Models.Profile Profile
		{
			get { return _profile; }
			set { SetProperty(_profile, value, () => _profile = value); }
		}

		public bool ShowSpinner
		{
			get
			{
				return _showSpinner;
			}
			set { SetProperty(_showSpinner, value, () => _showSpinner = value); }
		}

		public ObservableCollection<string> Tags
		{
			get { return _tags; }
			set { SetProperty(ref _tags, value); }
		}

		public async Task RegisterNewProfile()
		{
			ShowSpinner = true;
			EnableButton = false;

			Profile = await Profile.RegisterNewProfile(Email, Password, FirstName, LastName, string.Join(",", Tags));

			ShowSpinner = false;
		}
	}
}
