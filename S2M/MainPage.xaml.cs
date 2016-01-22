using Microsoft.WindowsAzure.Messaging;
using S2M.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Windows.Networking.PushNotifications;
using Windows.Security.Credentials;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace S2M
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainPage : Page
	{
		private CancellationTokenSource _cts = null;
		private NavigationPageCriteria NavigationPageCriteria { get; set; }

		public MainPage()
		{
			this.InitializeComponent();
		}

		protected async override void OnNavigatedTo(NavigationEventArgs e)
		{
			var criteria = (NavigationPageCriteria)e.Parameter;
			if (criteria != null)
			{
				NavigationPageCriteria = criteria;
			}
		}

		protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
		{
			if (_cts != null)
			{
				_cts.Cancel();
				_cts = null;
			}

			base.OnNavigatingFrom(e);
		}

		private void Page_Loaded(object sender, RoutedEventArgs e)
		{
			if (ConnectionHelper.CheckForInternetAccess())
			{
				GetCredentialsFromVault();
				//ScheduleNotificationButton(); TODO: Make correct service for tile information
			}
			else {
				Frame.Navigate(typeof(Pages.Locations));
			}
		}

		private void textBlockregister_Tapped(object sender, TappedRoutedEventArgs e)
		{

		}

		private void btnLogin_Click(object sender, RoutedEventArgs e)
		{
			var username = textBoxUserName.Text;
			var password = textBoxPassword.Password;

			CheckLoginCrendentials(username, password);
		}

		private void GetCredentialsFromVault()
		{
			var vault = new PasswordVault();
			const string vaultResource = "S2M";

			try
			{
				var credentialList = vault.FindAllByResource(vaultResource);
				if (credentialList.Any())
				{
					var credentials = credentialList.First();
					var username = credentials.UserName;
					var password = vault.Retrieve(vaultResource, username).Password;

					CheckLoginCrendentials(username, password);
				}
			}
			catch (Exception ex) { }
		}

		private async void CheckLoginCrendentials(string username, string password)
		{
			LoginProgressRing.IsActive = true;
			LoginProgressRing.Visibility = Visibility.Visible;
			LoginInputStackPanel.Visibility = Visibility.Collapsed;

			_cts = new CancellationTokenSource();
			CancellationToken token = _cts.Token;

			try
			{
				var login = await Models.Login.LoginUser(username, password);
				if (login != null)
				{
					await Models.ProfileDevice.RegisterProfileDevice(token);

					if (!string.IsNullOrEmpty(login.ProfileToken))
					{
						StorageService.SaveSetting("ProfileToken", login.ProfileToken);

						InitNotificationsAsync(login.ProfileKey);
					}
				}


				if (login != null)
				{
					SaveCredentialsInVault(username, password);

					var authenticatedProfile = await Models.Profile.GetProfile();
					if (authenticatedProfile != null)
					{
						await Common.StorageService.PersistObjectAsync("Profile", authenticatedProfile);

						LoginProgressRing.IsActive = false;
						LoginProgressRing.Visibility = Visibility.Collapsed;

						Frame.Navigate(typeof(Navigation), NavigationPageCriteria);
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
			catch (Exception) { }
			finally
			{
				_cts = null;
			}
		}

		private void SaveCredentialsInVault(string username, string password)
		{
			var vault = new PasswordVault();
			vault.Add(new PasswordCredential("S2M", username, password));
		}

		private async void InitNotificationsAsync(string profileKey)
		{
			var channel = await PushNotificationChannelManager.CreatePushNotificationChannelForApplicationAsync();

			var hub = new NotificationHub("notifications", "Endpoint=sb://seats2meet.servicebus.windows.net/;SharedAccessKeyName=DefaultListenSharedAccessSignature;SharedAccessKey=bJT+rNOH5GzBi04UQG2hnnF/mhKHh5FM424nQhBD3M8=");
			var tags = new List<string>();
			tags.Add(profileKey);
			var result = await hub.RegisterNativeAsync(channel.Uri, tags);

			// Displays the registration ID so you know it was successful
			if (result.RegistrationId != null)
			{
				//var dialog = new MessageDialog("Registration successful: " + result.RegistrationId);
				//dialog.Commands.Add(new UICommand("OK"));
				//await dialog.ShowAsync();
			}

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
