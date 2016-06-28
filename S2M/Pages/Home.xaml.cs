using S2M.Models;
using S2M.ViewModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
		public HomeViewModel ViewModel { get; set; }

		private CancellationTokenSource _cts = null;
		private int _selectedPivotIndex = 0;

		public Home()
		{
			this.InitializeComponent();

			ViewModel = new HomeViewModel();

			DataContext = ViewModel;
		}

		protected async override void OnNavigatedTo(NavigationEventArgs e)
		{
			
		}

		protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
		{
			if (_cts != null)
			{
				_cts.Cancel();
				_cts = null;
			}
		}

		private async void Page_Loaded(object sender, RoutedEventArgs e)
		{
			await GetCurrentCheckin();

			await GetNearbyLocations();
			await GetTodaysEvents();
		}
		
		private async Task GetCurrentCheckin()
		{
			ViewModel.CurrentCheckin = await ViewModel.GetCurrentCheckin();
		}

		private async Task GetNearbyLocations()
		{
			await ViewModel.GetNearbyLocations();
			await ViewModel.GetFavoriteLocations();
		}

		private async Task GetTodaysEvents()
		{
			await ViewModel.GetTodaysEvents();
		}

		private void CurrentCheckinGrid_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
		{
			var checkinCriteria = new CheckinFinalPageCriteria
			{
				CheckIn = ViewModel.CurrentCheckin,
				IsNewCheckIn = false,
				ShowMatches = true
			};

			Frame.Navigate(typeof(CheckInFinal), checkinCriteria);
		}

		private void NearbyLocationsFlipView_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
		{
			var index = (sender as FlipView).SelectedIndex;
			if (ViewModel.NearbyLocations.Any()) {
				var selectedLocation = ViewModel.NearbyLocations[index];
				if (selectedLocation != null)
				{
					GoToLocationDetail(selectedLocation);
				}
			}
		}

		private void FavoriteLocationsFlipView_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
		{
			var index = (sender as FlipView).SelectedIndex;
			if (ViewModel.FavoriteLocations.Any())
			{
				var selectedLocation = ViewModel.FavoriteLocations[index];
				if (selectedLocation != null)
				{
					GoToLocationDetail(selectedLocation);
				}
			}
		}

		private void GoToLocationDetail(Location location)
		{
			var criteria = new LocationDetailPageCriteria
			{
				LocationId = 0,
				Location = location
			};

			Frame.Navigate(typeof(LocationDetail), criteria);
		}

		private void EventsGridView_ItemClick(object sender, ItemClickEventArgs e)
		{
			var eventObj = ((EventCalendar)e.ClickedItem);

			Frame.Navigate(typeof(EventDetail), eventObj);
		}
	}
}
