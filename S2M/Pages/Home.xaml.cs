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
			if (ViewModel.CurrentCheckin == null)
			{
				await GetLastCheckin();
			}

			await GetLocationSuggestions();
			await GetTodaysEvents();
		}
		
		private async Task GetCurrentCheckin()
		{
			await ViewModel.GetCurrentCheckin();
		}

		private async Task GetLastCheckin()
		{
			await ViewModel.GetLastCheckin();
		}

		private async Task GetLocationSuggestions()
		{
			await ViewModel.GetFavoriteLocations();
			if (ViewModel.NrOfSuggestedLocations > 0)
			{
				ViewModel.ShowFavoriteLocations = true;
			}
			else
			{
				ViewModel.ShowFavoriteLocations = false;
				await ViewModel.GetNearbyLocations();
				if (ViewModel.NrOfSuggestedLocations > 0)
				{
					ViewModel.ShowNearbyLocations = true;
				}
			}
		}

		private async Task GetTodaysEvents()
		{
			await ViewModel.GetTodaysEvents();
		}

		private void SuggestedLocationButton_Click(object sender, RoutedEventArgs e)
		{
			var locationId = (int)((Button)sender).Tag;
			var selectedLocations = ViewModel.SuggestedLocations.Where(l => l.Id == locationId);
			if (selectedLocations.Any())
			{
				var selectedLocation = selectedLocations.FirstOrDefault();
				GoToLocationDetail(selectedLocation);
			}
		}

		private void TodaysEventsButton_Click(object sender, RoutedEventArgs e)
		{
			var eventId = (int)((Button)sender).Tag;
			var selectedEvents = ViewModel.Events.Where(ev => ev.Id == eventId);
			if (selectedEvents.Any())
			{
				var selectedEvent = selectedEvents.FirstOrDefault();
				GoToEventDetail(selectedEvent);
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

		private void GoToEventDetail(EventCalendar selectedEvent)
		{
			Frame.Navigate(typeof(EventDetail), selectedEvent);
		}

		private void EventsGridView_ItemClick(object sender, ItemClickEventArgs e)
		{
			var eventObj = ((EventCalendar)e.ClickedItem);

			Frame.Navigate(typeof(EventDetail), eventObj);
		}

		private void ShowMatchesCurrenCheckInButton_Click(object sender, RoutedEventArgs e)
		{
			var checkinCriteria = new CheckinFinalPageCriteria
			{
				CheckIn = ViewModel.CurrentCheckin,
				IsNewCheckIn = false,
				ShowMatches = true
			};

			Frame.Navigate(typeof(CheckInFinal), checkinCriteria);
		}

		private void LastCheckinCheckinNowButton_Click(object sender, RoutedEventArgs e)
		{
			var criteria = new LocationDetailPageCriteria
			{
				LocationId = ViewModel.LastCheckin.LocationId,
				Location = null
			};

			Frame.Navigate(typeof(LocationDetail), criteria);
		}
	}
}
