using S2M.Common;
using S2M.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Security.Credentials;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace S2M {
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainPage : Page {
		public MainPage() {
			this.InitializeComponent();
        }

		private void Page_Loaded(object sender, RoutedEventArgs e) {
			if (ConnectionHelper.CheckForInternetAccess()) {
				GetCredentialsFromVault();
				//ScheduleNotificationButton(); TODO: Make correct service for tile information
			}
			else {
				Frame.Navigate(typeof(Pages.Locations));
			}
        }

		private void textBlockregister_Tapped(object sender, TappedRoutedEventArgs e) {

		}

		private void btnLogin_Click(object sender, RoutedEventArgs e) {
			var username = textBoxUserName.Text;
			var password = textBoxPassword.Password;

			CheckLoginCrendentials(username, password);
		}

		private void GetCredentialsFromVault() {
			var vault = new PasswordVault();
			const string vaultResource = "S2M";

			try {
				var credentialList = vault.FindAllByResource(vaultResource);
				if (credentialList.Any()) {
					var credentials = credentialList.First();
					var username = credentials.UserName;
					var password = vault.Retrieve(vaultResource, username).Password;

					CheckLoginCrendentials(username, password);
				}
			}
			catch (Exception ex) { }
		}

		private async void CheckLoginCrendentials(string username, string password) {
			LoginProgressRing.IsActive = true;
			LoginProgressRing.Visibility = Visibility.Visible;
			LoginInputStackPanel.Visibility = Visibility.Collapsed;

			var login = await Models.Login.LoginUser(username, password);
			if (login != null) {
				if (!string.IsNullOrEmpty(login.ProfileToken)) {
					StorageService.SaveSetting("ProfileToken", login.ProfileToken);
				}
			}

			if (login != null) {
				SaveCredentialsInVault(username, password);

				var authenticatedProfile = await Models.Profile.GetProfile();
				if (authenticatedProfile != null) {
					await Common.StorageService.PersistObjectAsync("Profile", authenticatedProfile);

					LoginProgressRing.IsActive = false;
					LoginProgressRing.Visibility = Visibility.Collapsed;

					Frame.Navigate(typeof(Navigation));
				}
				else {
					var vault = new PasswordVault();
					vault.Remove(new PasswordCredential("S2M", username, password));

					LoginInputStackPanel.Visibility = Visibility.Visible;
				}

				LoginProgressRing.IsActive = false;
				LoginProgressRing.Visibility = Visibility.Collapsed;
			}
		}

		private void SaveCredentialsInVault(string username, string password) {
			var vault = new PasswordVault();
			vault.Add(new PasswordCredential("S2M", username, password));
		}

		//private async void ScheduleNotificationButton() {
		//	//var tileContent = new Uri(Common.StorageService.LoadSetting("ApiUrl") + "/api/livetile/nrofcheckins");
		//	var tileContent = new Uri("http://localhost:50210/api/livetile/nrofcheckins");
		//	var requestedInterval = PeriodicUpdateRecurrence.HalfHour;

		//	var updater = TileUpdateManager.CreateTileUpdaterForApplication();
		//	updater.StartPeriodicUpdate(tileContent, requestedInterval);
		//}
	}
}
