using S2M.Common;
using S2M.Models;
using S2M.ViewModel;
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
		protected string SearchTerm { get; set; }
		public LocationsViewModel ViewModel { get; set; }

		private CancellationTokenSource _cts = null;

		public Locations()
		{
			this.InitializeComponent();

			ViewModel = new LocationsViewModel();
			DataContext = ViewModel;

			SearchTerm = "";
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
				ViewModel.DeviceIsOffline = false;
			}
			else {
				ViewModel.DeviceIsOffline = true;
			}

			await LoadLocationsAsync(SearchTerm);

			LocationsProgressRing.IsActive = false;
			LocationsProgressRing.Visibility = Visibility.Collapsed;
		}

		private async Task LoadLocationsAsync(string searchTerm = "")
		{
			_cts = new CancellationTokenSource();
			CancellationToken token = _cts.Token;

			ViewModel.ShowLocationDistance = false;

			try
			{
				var accessStatus = await Geolocator.RequestAccessAsync();
				switch (accessStatus)
				{
					case GeolocationAccessStatus.Allowed:

						try
						{
							var geoposition = await GeoService.GetSinglePositionAsync(token);
							ViewModel.Latitude = geoposition.Point.Position.Latitude;
							ViewModel.Longitude = geoposition.Point.Position.Longitude;

							ViewModel.ShowLocationDistance = true;
						}
						catch (Exception) { }

						break;
					case GeolocationAccessStatus.Denied:
						ViewModel.ShowLocationDistance = false;
						break;
				}

				await Location.GetWorkspaceLocationsAsync(token, ViewModel.LocationList, searchTerm, ViewModel.Latitude, ViewModel.Longitude);
				await FillLocationsMap();
			}
			catch (Exception) { }
			finally
			{
				_cts = null;
			}
		}

		private async Task FillLocationsMap()
		{
			foreach (var location in ViewModel.LocationList)
			{
				var pin = new MapIcon()
				{
					Location = new Geopoint(new BasicGeoposition() { Latitude = location.Latitude, Longitude = location.Longitude }),
					Title = location.Name,
					Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/s2mpin.png")),
					NormalizedAnchorPoint = new Point(0.5, 1.0)
				};

				Image pinImage = new Image();
				pinImage.Source = new BitmapImage(new Uri("ms-appx:///Assets/s2mpin.png"));
				pinImage.Width = 20;
				pinImage.Height = 20;
				MapControl.SetLocation(pinImage, new Geopoint(new BasicGeoposition() { Latitude = location.Latitude, Longitude = location.Longitude }));
				MapControl.SetNormalizedAnchorPoint(pinImage, new Point(0.5, 0.5));
				mapsControlLocations.Children.Add(pinImage);
			}

			if (ViewModel.ShowLocationDistance)
			{
				var currentLocation = new Geopoint(new BasicGeoposition() { Latitude = ViewModel.Latitude, Longitude = ViewModel.Longitude });

				await mapsControlLocations.TrySetViewAsync(currentLocation, 15, 0, 0, MapAnimationKind.None);
			}
		}

		private void LocationsGridView_ItemClick(object sender, ItemClickEventArgs e)
		{
			var location = ((Location)e.ClickedItem);
			var criteria = new LocationDetailPageCriteria
			{
				LocationId = 0,
				Location = location
			};

			Frame.Navigate(typeof(LocationDetail), criteria);
		}

		private async void SearchLocations(string searchTerm)
		{
			ViewModel.LocationList.Clear();
			await LoadLocationsAsync(searchTerm);
		}
	}
}
