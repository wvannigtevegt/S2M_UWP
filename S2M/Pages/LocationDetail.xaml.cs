using S2M.Controls;
using S2M.Models;
using S2M.ViewModel;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace S2M.Pages
{
	public sealed partial class LocationDetail : Page
	{
		private CancellationTokenSource _cts = null;
		private string _selectedDate = "";

		public CheckIn ActiveCheckIn { get; set; }
		public Location LocationObject { get; set; }
		public OpeningHour LocationOpeningHours { get; set; }
		public int ReservationId { get; set; }
		public Reservation ReservationObject { get; set; }
		public LocationDetailViewModel ViewModel { get; set; }

		public LocationDetail()
		{
			this.InitializeComponent();

			ViewModel = new LocationDetailViewModel();
			ViewModel.EnableButton = true;

			DataContext = ViewModel;
		}

		protected async override void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);

			var criteria = (LocationDetailPageCriteria)e.Parameter;
			if (criteria != null)
			{
				if (criteria.Location != null)
				{
					LocationObject = criteria.Location;
				}
				if (criteria.LocationId > 0 && LocationObject == null)
				{
					_cts = new CancellationTokenSource();
					CancellationToken token = _cts.Token;

					try
					{
						LocationObject = await Location.GetLocationById(token, criteria.LocationId);
					}
					catch (Exception) { }
					finally
					{
						_cts = null;
					}
				}
			}

			var localSettings = ApplicationData.Current.LocalSettings;
			if (localSettings.Values["SelectedDate"] != null)
			{
				//_selectedDate = (string)localSettings.Values["SelectedDate"];
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

			var localSettings = ApplicationData.Current.LocalSettings;
			localSettings.Values["SelectedDate"] = ViewModel.SelectedDate.Date.ToString("yyyy-MM-dd");
		}

		private async void Page_Loaded(object sender, RoutedEventArgs e)
		{
			await ViewModel.GetProfileCheckIns();

			LocationNameTextBlock.Text = LocationObject.Name;
			LocationImage.Source = new BitmapImage(new Uri(LocationObject.Image_320));

			if (!string.IsNullOrEmpty(LocationObject.Address))
			{
				LocationAddressTextBlock.Text = LocationObject.Address;
			}
			if (!string.IsNullOrEmpty(LocationObject.Zipcode))
			{
				LocationPostalCodeTextBlock.Text = LocationObject.Zipcode;
			}
			if (!string.IsNullOrEmpty(LocationObject.City))
			{
				var city = LocationObject.City;
				if (!string.IsNullOrEmpty(LocationObject.State))
				{
					city += " " + LocationObject.State;
				}

				LocationCityTextBlock.Text = city;
			}

			var curentDate = DateTime.Now;
			var dates = new ObservableCollection<LocationDay>();
			var lastSelectedDate = new LocationDay();

			int i = 0;
			while (i < 6)
			{
				dates.Add(new LocationDay()
				{
					ActiveCheckIn = CheckIfProfilehasCheckIn(curentDate.AddDays(i)),
					Date = curentDate.AddDays(i)
				});

				i++;
			}

			if (_selectedDate != null)
			{
				var locationDays = dates.Where(d => d.Date.ToString("yyyy-MM-dd") == _selectedDate);
				if (locationDays.Any())
				{
					var locationDay = locationDays.FirstOrDefault();
					if (locationDay != null && locationDay.Date.Year != 1900)
					{
						lastSelectedDate = locationDay;
					}
				}
			}

			ViewModel.Dates = dates;

			if (lastSelectedDate != null && lastSelectedDate.Date.Year != 1)
			{
				ViewModel.SelectedDate = lastSelectedDate;
			}
			else
			{
				ViewModel.SelectedDate = ViewModel.Dates[0];
			}
		}

		private async Task GetLocationOpeningHours()
		{
			_cts = new CancellationTokenSource();
			CancellationToken token = _cts.Token;

			ViewModel.IsOpen = false;
			ViewModel.AlreadyCheckedin = true;

			ViewModel.IsBookable = false;

			try
			{
				LocationOpeningHours = await OpeningHour.GetLocationOpeningHourssAsync(token, LocationObject.Id, ViewModel.SelectedDate.Date);
				if (ViewModel.SelectedDate.Date.Date == DateTime.Now.Date)
				{
					if (LocationOpeningHours.MinTimeOpen > DateTime.Now)
					{
						ViewModel.StartTime = new TimeSpan(LocationOpeningHours.MinTimeOpen.Hour, LocationOpeningHours.MinTimeOpen.Minute, 0);
					}
					else
					{
						ViewModel.StartTime = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, 0);
						ViewModel.StartTime = TimeSpan.FromMinutes(15 * Math.Ceiling(ViewModel.StartTime.TotalMinutes / 15));
					}
				}
				else
				{
					ViewModel.StartTime = new TimeSpan(LocationOpeningHours.MinTimeOpen.Hour, LocationOpeningHours.MinTimeOpen.Minute, 0);
				}
				ViewModel.EndTime = new TimeSpan(LocationOpeningHours.MaxTimeClose.Hour, LocationOpeningHours.MaxTimeClose.Minute, 0);

				if (LocationOpeningHours.NrOfLocations > 0)
				{
					if (ViewModel.SelectedDate.Date.Date != DateTime.Now.Date || (ViewModel.SelectedDate.Date.Date == DateTime.Now.Date && DateTime.Now <= LocationOpeningHours.MaxTimeClose))
					{
						ViewModel.IsOpen = true;
					}
				}

				if (ViewModel.SelectedDate != null && ViewModel.SelectedDate.ActiveCheckIn == null)
				{
					ViewModel.AlreadyCheckedin = false;
				}

				if (ViewModel.IsOpen == false || ViewModel.AlreadyCheckedin == true)
				{
					ViewModel.IsBookable = false;
				}
				else
				{
					ViewModel.IsBookable = true;
				}
			}
			catch (Exception) { }
			finally
			{
				_cts = null;
			}

			SetDateTimeTextBoxes();
		}

		private CheckIn CheckIfProfilehasCheckIn(DateTime date)
		{
			var checkins = ViewModel.ProfileCheckIns.Where(p => Common.DateService.ConvertFromUnixTimestamp(p.StartTimeStamp).Date == date.Date);
			if (checkins.Any())
			{
				return checkins.FirstOrDefault();
			}
			return null;
		}

		private async void LocationCheckInsGridView_ItemClick(object sender, ItemClickEventArgs e)
		{
			var checkIn = (CheckIn)e.ClickedItem;
			var authenticatedProfile = await Common.StorageService.RetrieveObjectAsync<Models.Profile>("Profile");

			if (checkIn.ProfileId == authenticatedProfile.Id)
			{
				Frame.Navigate(typeof(CheckInFinal), checkIn);
			}
			else
			{
				Frame.Navigate(typeof(CheckInDetail), checkIn);
			}


		}

		private async void SearchAvailabilityButton_Click(object sender, RoutedEventArgs e)
		{
			await CheckLocationAvailability();
		}

		private async void AvailableUnitsListView_ItemClick(object sender, ItemClickEventArgs e)
		{

			AvailableUnitsListView.ItemsSource = null;
			AvailableUnitsListView.Visibility = Visibility.Collapsed;

			var selectedUnit = (AvailableUnit)e.ClickedItem;
			_cts = new CancellationTokenSource();
			CancellationToken token = _cts.Token;

			try
			{
				ReservationObject = await CheckinUser(ViewModel.SearchKey, selectedUnit.LocationId, selectedUnit.SearchDateId, selectedUnit.UnitId);

				await ParseReservationData();
			}
			catch (Exception) { }
			finally
			{
				_cts = null;
			}
		}

		private async void DateTimeHyperLinkButton_Click(object sender, RoutedEventArgs e)
		{
			DateTimeDialog dateDialog = new DateTimeDialog();
			dateDialog.LocationId = LocationObject.Id;
			dateDialog.StartTime = ViewModel.StartTime;
			dateDialog.EndTime = ViewModel.EndTime;
			dateDialog.MinStartTime = ViewModel.StartTime;
			dateDialog.MaxEndTimeSpan = ViewModel.EndTime;

			await dateDialog.ShowAsync();
			if (dateDialog.Result == ChangeDateTimeResult.ChangeDateTimeOK)
			{
				var changeCounter = 0;

				if (dateDialog.StartTime != ViewModel.StartTime)
				{
					ViewModel.StartTime = dateDialog.StartTime;
					changeCounter++;
				}
				if (dateDialog.EndTime != ViewModel.EndTime)
				{
					ViewModel.EndTime = dateDialog.EndTime;
					changeCounter++;
				}

				if (changeCounter > 0)
				{
					SetDateTimeTextBoxes();
				}
			}
		}

		private void SetDateTimeTextBoxes()
		{
			TimeTextBlock.Text = string.Format("{0:hh\\:mm}", ViewModel.StartTime) + " - " + string.Format("{0:hh\\:mm}", ViewModel.EndTime);
		}

		private async Task CheckLocationAvailability()
		{
			_cts = new CancellationTokenSource();
			CancellationToken token = _cts.Token;

			try
			{
				DateTimeHyperLinkButton.IsEnabled = false;

				AvailableUnitsListView.Visibility = Visibility.Collapsed;

				ViewModel.EnableButton = false;
				ViewModel.ShowSpinner = true;
				ViewModel.CheckinMessage = "Checking availability...";

				var availability = await Availability.GetAvailableLocations(token, LocationObject.Id, ViewModel.SelectedDate.Date, ViewModel.StartTime, ViewModel.EndTime);
				ViewModel.SearchKey = availability.SearchKey;

				if (availability.Locations.Count > 0)
				{
					var availableLocation = availability.Locations.First();
					var availableUnits = availableLocation.Units;

					if (availableUnits.Count() == 1)
					{
						var selectedUnit = availableUnits.First();
						if (selectedUnit != null)
						{
							ReservationObject = await CheckinUser(availability.SearchKey, availableLocation.LocationId, selectedUnit.SearchDateId, selectedUnit.UnitId);
							await ParseReservationData();
						}
					}
					else
					{
						ViewModel.CheckinMessage = "Please select your space...";

						AvailableUnitsListView.ItemsSource = availableUnits;
						AvailableUnitsListView.Visibility = Visibility.Visible;
					}
				}
				else
				{
					ViewModel.CheckinMessage = "No availability, please try an other date or time";
				}

				ViewModel.ShowSpinner = false;
			}
			catch (Exception ex) { }
			finally
			{
				_cts = null;

				DateTimeHyperLinkButton.IsEnabled = true;
			}

		}

		public async Task<Reservation> CheckinUser(string searchKey, int locationId, int searchDateId, int unitId)
		{
			_cts = new CancellationTokenSource();
			CancellationToken token = _cts.Token;

			try
			{
				var reservation = await Availability.SelectAvailableLocation(token, searchKey, locationId, searchDateId, unitId, 0);
				ReservationObject = reservation;
				ReservationId = reservation.Id;

				return reservation;
			}
			catch (Exception) { }
			finally
			{
				_cts = null;
			}

			return null;
		}

		public async Task ParseReservationData()
		{
			if (ReservationObject != null)
			{
				var _cts = new CancellationTokenSource();
				CancellationToken token = _cts.Token;

				try
				{
					var checkin = await CheckIn.GetCheckInByReservation(token, ReservationObject.Id);
					if (checkin != null)
					{
						ViewModel.ProfileCheckIns.Add(checkin);
						ViewModel.Checkins.Add(checkin);
						ViewModel.SelectedDate.ActiveCheckIn = checkin;

						Frame.Navigate(typeof(CheckInFinal), checkin);
					}
				}
				catch (Exception) { }
				finally
				{
					_cts = null;
				}
			}
		}

		private async void DatesFlipView_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			ViewModel.CheckinMessage = "";

			AvailableUnitsListView.ItemsSource = null;
			AvailableUnitsListView.Visibility = Visibility.Collapsed;

			await ViewModel.GetLocationCheckIns(LocationObject.Id);
			await GetLocationOpeningHours();
		}

		private void LocationNameHyperLinkButton_Click(object sender, RoutedEventArgs e)
		{
			var criteria = new LocationDetailPageCriteria
			{
				LocationId = ViewModel.SelectedDate.ActiveCheckIn.LocationId,
				Location = null
			};

			Frame.Navigate(typeof(LocationDetail), criteria);
		}
	}

	public class LocationDetailPageCriteria
	{
		public int LocationId { get; set; } = 0;
		public Location Location { get; set; } = null;
	}
}
