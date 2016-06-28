using S2M.Common;
using S2M.Models;
using S2M.ViewModel;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace S2M.Pages
{
	public sealed partial class CheckIns : Page {
		public CheckInsViewModel ViewModel { get; set; }
		public double Latitude { get; set; }
		public double Longitude { get; set; }

		protected string SearchTerm { get; set; }

		private CancellationTokenSource _cts = null;

		public CheckIns() {
			this.InitializeComponent();

			ViewModel = new CheckInsViewModel();
			DataContext = ViewModel;

			SearchTerm = "";
		}

		protected async override void OnNavigatedTo(NavigationEventArgs e) {
			var criteria = (CheckInPageCriteria)e.Parameter;
			if (criteria != null)
			{
				if (criteria.CheckInKnowledgeTag.CheckIns.Any())
				{
					ViewModel.Checkins.Clear();
					foreach (var checkIn in criteria.CheckInKnowledgeTag.CheckIns)
					{
						ViewModel.Checkins.Add(checkIn);
					}
				}

				SearchTerm = criteria.SearchTerm;
				if (SearchTerm == null)
				{
					SearchTerm = "";
				}
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
			var curentDate = DateTime.Now;
			var dates = new ObservableCollection<LocationDay>();

			int i = 0;
			while (i < 6)
			{
				dates.Add(new LocationDay()
				{
					ActiveCheckIn = null,
					Date = curentDate.AddDays(i)
				});

				i++;
			}
			ViewModel.Dates = dates;
			ViewModel.SelectedDate = dates[0];

			ViewModel.PageIsLoaded = true;
			ViewModel.LoadCheckInsAsync();
		}

		private async void DatesFlipView_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			await ViewModel.LoadCheckInsAsync();
		}

		private async void CheckInsGridView_ItemClick(object sender, ItemClickEventArgs e) {
			var checkIn = (CheckIn)e.ClickedItem;
			var authenticatedProfile = await Common.StorageService.RetrieveObjectAsync<Models.Profile>("Profile");

			if (checkIn.ProfileId == authenticatedProfile.Id)
			{
				var checkinCriteria = new CheckinFinalPageCriteria
				{
					IsNewCheckIn = false,
					CheckIn = checkIn
				};

				Frame.Navigate(typeof(CheckInFinal), checkinCriteria);
			}
			else
			{
				Frame.Navigate(typeof(CheckInDetail), checkIn);
			}
		}


		//private void CheckInssAutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args) {

		//}

		//private void CheckInssAutoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args) {

		//}
	}

	public class CheckInPageCriteria
	{
		public CheckInKnowledgeTag CheckInKnowledgeTag { get; set; }
		public string SearchTerm { get; set; }
	}
}
