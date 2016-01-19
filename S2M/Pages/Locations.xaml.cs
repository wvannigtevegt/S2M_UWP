using S2M.Common;
using S2M.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace S2M.Pages
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class Locations : Page
	{
		public ObservableCollection<Location> LocationList { get; set; }
		public bool DeviceIsOffline { get; set; }
		public double Latitude { get; set; }
		public double Longitude { get; set; }
		public bool ShowLocationDistance { get; set; }

		protected string SearchTerm { get; set; }

		private CancellationTokenSource _cts = null;

		public Locations()
		{
			this.InitializeComponent();

			LocationList = new ObservableCollection<Location>();
			SearchTerm = "";
			ShowLocationDistance = true;
		}

		protected async override void OnNavigatedTo(NavigationEventArgs e)
		{
			SearchTerm = (string)e.Parameter;
			if (SearchTerm == null)
			{
				SearchTerm = "";
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

		private async void Page_Loaded(object sender, RoutedEventArgs e)
		{
			LocationsProgressRing.IsActive = true;
			LocationsProgressRing.Visibility = Visibility.Visible;

			if (ConnectionHelper.CheckForInternetAccess())
			{
				DeviceIsOffline = false;
			}
			else {
				DeviceIsOffline = true;
			}

			await LoadLocationsAsync(SearchTerm);

			LocationsProgressRing.IsActive = false;
			LocationsProgressRing.Visibility = Visibility.Collapsed;
		}

		private async Task LoadLocationsAsync(string searchTerm = "")
		{
			_cts = new CancellationTokenSource();
			CancellationToken token = _cts.Token;

			try
			{
				var accessStatus = await Geolocator.RequestAccessAsync();
				switch (accessStatus)
				{
					case GeolocationAccessStatus.Allowed:

						try
						{
							var geoposition = await GeoService.GetSinglePositionAsync(token);
							Latitude = geoposition.Point.Position.Latitude;
							Longitude = geoposition.Point.Position.Longitude;
						}
						catch (Exception) { }

						break;
					case GeolocationAccessStatus.Denied:
						ShowLocationDistance = false;
						break;
				}

				await Location.GetWorkspaceLocationsAsync(token, LocationList, searchTerm, Latitude, Longitude);
			}
			catch (Exception) { }
			finally
			{
				_cts = null;
			}
		}

		private void LocationsGridView_ItemClick(object sender, ItemClickEventArgs e)
		{
			var location = ((Location)e.ClickedItem);

			Frame.Navigate(typeof(LocationDetail), location);
		}

		//private void LocationsAutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args) {
		//	LocationsAutoSuggestBox.ItemsSource = LocationList.Where(s => s.Name.ToLower().StartsWith(sender.Text.ToLower())).Select(s => s.Name).ToList();
		//}

		//private void LocationsAutoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args) {
		//	SearchLocations(sender.Text);
		//}

		private async void SearchLocations(string searchTerm)
		{
			LocationList.Clear();
			await LoadLocationsAsync(searchTerm);
		}

		private void MapHyperlInkButton_Click(object sender, RoutedEventArgs e)
		{
			Frame.Navigate(typeof(LocationsMap), LocationList);
		}
	}
}
