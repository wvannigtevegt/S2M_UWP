using S2M.Models;
using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace S2M.Pages {
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class Home : Page {
		protected ObservableCollection<CheckIn> CheckInRecommendations { get; set; }
		protected ObservableCollection<Location> LocationRecommendations { get; set; }
		protected string SearchTerm { get; set; }
		protected string WorkingOn { get; set; }

		private CancellationTokenSource _cts = null;

		public Home() {
			this.InitializeComponent();

			CheckInRecommendations = new ObservableCollection<CheckIn>();
			LocationRecommendations = new ObservableCollection<Location>();
			SearchTerm = "";
			WorkingOn = "Javascript";
		}

		protected override void OnNavigatingFrom(NavigatingCancelEventArgs e) {
			if (_cts != null) {
				_cts.Cancel();
				_cts = null;
			}

			base.OnNavigatingFrom(e);
		}

		private void Page_Loaded(object sender, RoutedEventArgs e) {

		}

		private async void SetWorkingOnButton_Click(object sender, RoutedEventArgs e) {
			WorkingOn = WorkingOnTextBox.Text;

			await LoadRequiredData();
		}

		private async void RecommendationsPivot_SelectionChanged(object sender, SelectionChangedEventArgs e) {
			await LoadRequiredData();
		}

		private async Task LoadRequiredData() {
			_cts = new CancellationTokenSource();
			CancellationToken token = _cts.Token;

			try {
				switch (RecommendationsPivot.SelectedIndex) {
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
			finally {
				_cts = null;
			}
		}

		private async Task GetCheckinRecommendations(CancellationToken token) {
			CheckInRecommendations.Clear();
			await CheckIn.GetCheckInsAsync(token, CheckInRecommendations, 0, 0, SearchTerm, 0, 0, 0, WorkingOn, 1, 10, false);			
		}
		private async Task GetLocationRecommendations(CancellationToken token) {
			LocationRecommendations.Clear();
			await Location.GetLocationRecommendationsAsync(token, LocationRecommendations, 0, 0, 0, WorkingOn, 1, 10);
		}

		private void CheckInRecommendationsGridView_ItemClick(object sender, ItemClickEventArgs e) {
			var checkIn = (CheckIn)e.ClickedItem;

			Frame.Navigate(typeof(CheckInDetail), checkIn);
		}

		private void LocationRecommendationsGridView_ItemClick(object sender, ItemClickEventArgs e) {
			var location = (Location)e.ClickedItem;

			Frame.Navigate(typeof(LocationDetail), location);
		}
	}
}
