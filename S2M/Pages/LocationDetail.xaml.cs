using S2M.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace S2M.Pages {
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class LocationDetail : Page {
		public Location LocationObject { get; set; }
		public ObservableCollection<Activity> ActivityList { get; set; }
		public ObservableCollection<CheckIn> CheckInList { get; set; }
		public ObservableCollection<CheckInKnowledgeTag> TagCheckInList { get; set; }

		protected string CartKey { get; set; }
		protected DateTime Date { get; set; }
		protected TimeSpan EndTime { get; set; }
		protected TimeSpan StartTime { get; set; }

		private CancellationTokenSource _cts = null;

		public LocationDetail() {
			this.InitializeComponent();

			ActivityList = new ObservableCollection<Activity>();
			CheckInList = new ObservableCollection<CheckIn>();
			TagCheckInList = new ObservableCollection<CheckInKnowledgeTag>();
		}

		protected async override void OnNavigatedTo(NavigationEventArgs e) {
			var locationObject = (Models.Location)e.Parameter;
			if (locationObject != null) {
				LocationObject = locationObject;
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
			LocationNameTextBlock.Text = LocationObject.Name;

			var addressLine = "";
			if (!string.IsNullOrEmpty(LocationObject.Address)) {
				addressLine += LocationObject.Address;
			}
			if (!string.IsNullOrEmpty(LocationObject.Zipcode)) {
				addressLine += ", " + LocationObject.Zipcode;
			}
			if (!string.IsNullOrEmpty(LocationObject.City)) {
				addressLine += ", " + LocationObject.City;
			}
			if (!string.IsNullOrEmpty(LocationObject.State)) {
				addressLine += " " + LocationObject.State;
			}
			LocationAddressLineTextBlock.Text = addressLine;

			_cts = new CancellationTokenSource();
			CancellationToken token = _cts.Token;

			try {
				var locationText = await LocationText.GetLocationDescriptionAsync(token, LocationObject.Id);
				//LocationDescriptionTextBlock.Text = locationText.Description;

				await CheckIn.GetCheckInsAsync(token, CheckInList, LocationObject.Id);
				await CheckInKnowledgeTag.GetLocationCheckInKnowledgeTagsAsync(TagCheckInList, LocationObject.Id);
			}
			catch (Exception) { }
			finally {
				_cts = null;
			}
		}

		private void LocationCheckInsGridView_ItemClick(object sender, ItemClickEventArgs e) {
			var checkIn = (CheckIn)e.ClickedItem;

			Frame.Navigate(typeof(CheckInDetail), checkIn);
		}

		private void TagCheckInsListView_ItemClick(object sender, ItemClickEventArgs e) {
			var tagCheckIn = (CheckInKnowledgeTag)e.ClickedItem;

			CheckInList.Clear();
			foreach (var checkIn in tagCheckIn.CheckIns) {
				CheckInList.Add(checkIn);
			}
		}

		private async void CheckInButton_Click(object sender, RoutedEventArgs e) {
			CheckInButton.IsEnabled = false;

			_cts = new CancellationTokenSource();
			CancellationToken token = _cts.Token;

			try {
				Date = new DateTime(2016, 2, 12);
				StartTime = new TimeSpan(9, 0, 0);
				EndTime = new TimeSpan(17, 0, 0);

				var availability = await Availability.GetAvailableLocations(token, LocationObject.Id, Date, StartTime, EndTime);
				if (availability.Locations.Count > 0) {
					var availableLocation = availability.Locations.First();
					var availableUnits = availableLocation.Units;


					var availableUnit = availableUnits.First();
					if (availableUnit != null) {
						var cart = await Availability.SelectAvailableLocation(token, availability.SearchKey, availableLocation.LocationId, availableUnit.SearchDateId, availableUnit.UnitId, 0);

						if (cart != null) {
							CartKey = cart.CartKey;
							AvailableUnitsListView.ItemsSource = availableUnits;
							//await cart.FinalizeCart();
						}
					}
				}
			}
			catch (Exception) { }
			finally {
				_cts = null;

				CheckInButton.IsEnabled = true;
			}
		}

		private async void AvailableUnitsListView_ItemClick(object sender, ItemClickEventArgs e) {
			var availableUnit = (AvailableUnit)e.ClickedItem;

			_cts = new CancellationTokenSource();
			CancellationToken token = _cts.Token;

			try {
				await Cart.SetCarUnit(token, CartKey, availableUnit.SearchDateId, availableUnit.UnitId, availableUnit.CurrencyId, (double)availableUnit.Price, availableUnit.TaxId, availableUnit.Crc);
			}
			catch (Exception) { }
			finally {
				_cts = null;
			}
		}
	}
}
