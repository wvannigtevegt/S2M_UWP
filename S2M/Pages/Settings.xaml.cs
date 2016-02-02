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
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace S2M.Pages {
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class Settings : Page {
		public ObservableCollection<Channel> ChannelList { get; set; }
		public ObservableCollection<Country> CountryList { get; set; }

		private CancellationTokenSource _cts = null;

		public Settings() {
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
	}
}
