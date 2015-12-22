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
using Windows.Devices.Geolocation;
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
	public sealed partial class CheckIns : Page {
		public ObservableCollection<CheckIn> CheckInList { get; set; }
		public double Latitude { get; set; }
		public double Longitude { get; set; }

		protected string SearchTerm { get; set; }

		private CancellationTokenSource _cts = null;

		public CheckIns() {
			this.InitializeComponent();

			CheckInList = new ObservableCollection<CheckIn>();
			SearchTerm = "";
		}

		protected async override void OnNavigatedTo(NavigationEventArgs e) {
			SearchTerm = (string)e.Parameter;
			if (SearchTerm == null) {
				SearchTerm = "";
			}
		}

		protected override void OnNavigatingFrom(NavigatingCancelEventArgs e) {
			if (_cts != null) {
				_cts.Cancel();
				_cts = null;
			}

			base.OnNavigatingFrom(e);
		}

		private async void Page_Loaded(object sender, RoutedEventArgs e) {
			CheckInsProgressRing.IsActive = true;
			CheckInsProgressRing.Visibility = Visibility.Visible;

			await LoadCheckInsAsync(SearchTerm);

			CheckInsProgressRing.IsActive = false;
			CheckInsProgressRing.Visibility = Visibility.Collapsed;
		}

		private async Task LoadCheckInsAsync(string searchTerm = "") {
			_cts = new CancellationTokenSource();
			CancellationToken token = _cts.Token;

			try {
				var accessStatus = await Geolocator.RequestAccessAsync();
				switch (accessStatus) {
					case GeolocationAccessStatus.Allowed:

						try {
							var geoposition = await GeoService.GetSinglePositionAsync(token);
							Latitude = geoposition.Point.Position.Latitude;
							Longitude = geoposition.Point.Position.Longitude;
						}
						catch (Exception) { }

						break;
					case GeolocationAccessStatus.Denied:
						//ShowLocationDistance = false;
						break;
				}

				await CheckIn.GetCheckInsAsync(token, CheckInList, 0, 0, searchTerm, Latitude, Longitude, 0, "", 0, 0, true);
			}
			catch (Exception) { }
			finally {
				_cts = null;
			}
		}

		private void CheckInsGridView_ItemClick(object sender, ItemClickEventArgs e) {
			var checkIn = (CheckIn)e.ClickedItem;

			Frame.Navigate(typeof(CheckInDetail), checkIn);
		}

		//private void CheckInssAutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args) {

		//}

		//private void CheckInssAutoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args) {

		//}
	}
}
