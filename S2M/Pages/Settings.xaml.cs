using Microsoft.WindowsAzure.Messaging;
using S2M.Common;
using S2M.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Networking.PushNotifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace S2M.Pages
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class Settings : Page
	{
		public ObservableCollection<Channel> ChannelList { get; set; }
		public ObservableCollection<Country> CountryList { get; set; }

		private CancellationTokenSource _cts = null;

		public Settings()
		{
			this.InitializeComponent();

			ChannelList = new ObservableCollection<Channel>();
			CountryList = new ObservableCollection<Country>();
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

		private async void Page_Loaded(object sender, RoutedEventArgs e)
		{
			await LoadChannelsAsync();
			await LoadCountriesAsync();

			var _recieveNotifications = StorageService.LoadSetting("RecieveNotifications");
			if (_recieveNotifications == "1")
			{
				NotificationsToggleSwitch.IsOn = true;
			}
			else
			{
				NotificationsToggleSwitch.IsOn = false;
			}
		}

		private async Task LoadChannelsAsync()
		{
			_cts = new CancellationTokenSource();
			CancellationToken token = _cts.Token;

			try
			{
				var channelId = int.Parse(StorageService.LoadSetting("ChannelId"));
				await Channel.GetActiveChannels(token, ChannelList);

				ChannelComboBox.ItemsSource = ChannelList;

				var selectedChannel = ChannelList.Where(c => c.Id == channelId).Single();

				ChannelComboBox.SelectedItem = selectedChannel;
			}
			catch (Exception) { }
			finally
			{
				_cts = null;
			}
		}

		private async Task LoadCountriesAsync()
		{
			_cts = new CancellationTokenSource();
			CancellationToken token = _cts.Token;

			try
			{
				var countryId = int.Parse(StorageService.LoadSetting("CountryId"));

				var noCountry = new Country
				{
					Id = 0,
					Name = "All"
				};
				CountryList.Add(noCountry);

				await Country.GetActiveCountries(token, CountryList);

				CountryComboBox.ItemsSource = CountryList;

				var selectedCountry = CountryList.Where(c => c.Id == countryId).Single();

				CountryComboBox.SelectedItem = selectedCountry;
			}
			catch (Exception) { }
			finally
			{
				_cts = null;
			}
		}

		private async Task SubscripeToNotifications()
		{
			var channel = await PushNotificationChannelManager.CreatePushNotificationChannelForApplicationAsync();

			var profileKey = "";
			Models.Profile _profile = await Common.StorageService.RetrieveObjectAsync<Models.Profile>("Profile");
			if (_profile != null)
			{
				profileKey = _profile.Key;
			}

			if (!string.IsNullOrEmpty(profileKey))
			{
				var hub = new NotificationHub("notifications", "Endpoint=sb://seats2meet.servicebus.windows.net/;SharedAccessKeyName=DefaultListenSharedAccessSignature;SharedAccessKey=bJT+rNOH5GzBi04UQG2hnnF/mhKHh5FM424nQhBD3M8=");
				var tags = new List<string>();
				tags.Add(profileKey);
				var result = await hub.RegisterNativeAsync(channel.Uri, tags);
			}

		}

		private async Task UnSubscripeToNotifications()
		{
			var channel = await PushNotificationChannelManager.CreatePushNotificationChannelForApplicationAsync();

			var hub = new NotificationHub("notifications", "Endpoint=sb://seats2meet.servicebus.windows.net/;SharedAccessKeyName=DefaultListenSharedAccessSignature;SharedAccessKey=bJT+rNOH5GzBi04UQG2hnnF/mhKHh5FM424nQhBD3M8=");
			//var r = await hub.
			await hub.UnregisterNativeAsync();//.UnregisterAllAsync(channel.Uri);

			//var tags = new List<string>();
			//tags.Add(profileKey);
			//var result = await hub.UnregisterNativeAsync();
		}

		private void ChannelComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (ChannelComboBox.SelectedIndex != -1)
			{
				var channel = (Channel)ChannelComboBox.SelectedValue;

				Common.StorageService.SaveSetting("ChannelId", channel.Id.ToString());
			}
		}

		private void CountryComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (CountryComboBox.SelectedIndex != -1)
			{
				var country = (Country)CountryComboBox.SelectedValue;

				Common.StorageService.SaveSetting("CountryId", country.Id.ToString());
			}
		}

		private async void NotificationsToggleSwitch_Toggled(object sender, RoutedEventArgs e)
		{
			var toggleSwitch = sender as ToggleSwitch;
			if (toggleSwitch != null)
			{
				if (toggleSwitch.IsOn)
				{
					StorageService.SaveSetting("RecieveNotifications", "1");
					await SubscripeToNotifications();
				}
				else
				{
					StorageService.SaveSetting("RecieveNotifications", "0");
					await UnSubscripeToNotifications();
				}
			}
		}
	}
}