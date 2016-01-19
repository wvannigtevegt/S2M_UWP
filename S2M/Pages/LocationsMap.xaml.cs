using S2M.Common;
using S2M.Models;
using System;
using System.Collections.ObjectModel;
using System.Threading;
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
	public sealed partial class LocationsMap : Page {
		public ObservableCollection<Location> LocationList { get; set; }
		public double Latitude { get; set; }
		public double Longitude { get; set; }
		public bool ShowLocationDistance { get; set; }

		private CancellationTokenSource _cts = null;

		public LocationsMap() {
			this.InitializeComponent();

			LocationList = new ObservableCollection<Location>();
		}

		protected async override void OnNavigatedTo(NavigationEventArgs e) {
			var locationList = (ObservableCollection<Location>)e.Parameter;
			if (locationList != null) {
				LocationList = locationList;
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
						ShowLocationDistance = false;
						break;
				}
			}
			catch (Exception) { }
			finally {
				_cts = null;
			}

			foreach (var location in LocationList) {
				var pin = new MapIcon() {
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

			if (ShowLocationDistance) {
				var currentLocation = new Geopoint(new BasicGeoposition() { Latitude = Latitude, Longitude = Longitude });

				await mapsControlLocations.TrySetViewAsync(currentLocation, 15, 0, 0, MapAnimationKind.None);
			}
		}

		private void ListHyperLinkButton_Click(object sender, RoutedEventArgs e) {
			Frame.Navigate(typeof(Locations));
		}
	}
}
