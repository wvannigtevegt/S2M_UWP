using S2M.Common;
using S2M.Models;
using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace S2M.Pages
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class Home : Page
	{
		protected ObservableCollection<CheckIn> CheckInRecommendations { get; set; }
		protected ObservableCollection<Location> LocationRecommendations { get; set; }
		protected double Latitude { get; set; }
		protected double Longitude { get; set; }
		protected string SearchTerm { get; set; }
		protected string WorkingOn { get; set; }

		private CancellationTokenSource _cts = null;
		private int _selectedPivotIndex = 0;

		public Home()
		{
			this.InitializeComponent();

			CheckInRecommendations = new ObservableCollection<CheckIn>();
			LocationRecommendations = new ObservableCollection<Location>();
			SearchTerm = "";
			WorkingOn = "";
		}

		protected async override void OnNavigatedTo(NavigationEventArgs e)
		{
			var workingOn = await Common.StorageService.RetrieveObjectAsync<Models.WorkingOn>("WorkingOn");
			if (workingOn != null)
			{
				WorkingOn = workingOn.Text.ToLower().Trim();
				WorkingOnTextBlock.Text = WorkingOn;
			}

			var localSettings = ApplicationData.Current.LocalSettings;
			if (localSettings.Values["HomePivotSelectedIndex"] != null)
			{
				var selectedPivot = localSettings.Values["HomePivotSelectedIndex"];
				if (selectedPivot != null)
				{
					int.TryParse(selectedPivot.ToString(), out _selectedPivotIndex);
				}
			}
		}

		protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
		{
			if (_cts != null)
			{
				_cts.Cancel();
				_cts = null;
			}

			var localSettings = ApplicationData.Current.LocalSettings;
			localSettings.Values["HomePivotSelectedIndex"] = RecommendationsPivot.SelectedIndex.ToString();

			//base.OnNavigatingFrom(e);
		}

		private async void Page_Loaded(object sender, RoutedEventArgs e)
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

						break;
				}



				RecommendationsPivot.SelectedIndex = _selectedPivotIndex;
			}
			catch (Exception) { }
			finally
			{
				_cts = null;
			}
		}

		private async void RecommendationsPivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			await LoadRecommendationsData();
		}

		private async Task LoadRecommendationsData()
		{
			_cts = new CancellationTokenSource();
			CancellationToken token = _cts.Token;

			try
			{
				switch (RecommendationsPivot.SelectedIndex)
				{
					case 0:
						await GetLocationRecommendations(token);
						break;
					case 1:
						await GetCheckinRecommendations(token);
						break;
					case 2:
						break;
				}
			}
			catch (Exception) { }
			finally
			{
				_cts = null;
			}
		}

		private async Task GetCheckinRecommendations(CancellationToken token)
		{
			CheckInRecommendations.Clear();
			await CheckIn.GetCheckInsAsync(token, CheckInRecommendations, DateTime.Now, 0, 0, SearchTerm, 0, 0, 0, WorkingOn, 1, 10, false);
		}
		private async Task GetLocationRecommendations(CancellationToken token)
		{
			LocationRecommendations.Clear();
			await Location.GetLocationRecommendationsAsync(token, LocationRecommendations, Latitude, Longitude, 0, WorkingOn, 1, 10);

			LocationRecommendationsGridView.ItemsSource = LocationRecommendations;
		}

		private void CheckInRecommendationsGridView_ItemClick(object sender, ItemClickEventArgs e)
		{
			var checkIn = (CheckIn)e.ClickedItem;

			Frame.Navigate(typeof(CheckInDetail), checkIn);
		}

		private void LocationRecommendationsGridView_ItemClick(object sender, ItemClickEventArgs e)
		{
			var location = (Location)e.ClickedItem;
			var criteria = new LocationDetailPageCriteria
			{
				LocationId = 0,
				Location = location
			};

			Frame.Navigate(typeof(LocationDetail), criteria);
		}

		private void SetWorkingOnButton_Click(object sender, RoutedEventArgs e)
		{
			Frame.Navigate(typeof(WorkingOn));
		}
	}
}
