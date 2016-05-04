using S2M.Controls;
using S2M.Models;
using S2M.ViewModel;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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

		public CheckIn ActiveCheckIn { get; set; }
		public Location LocationObject { get; set; }
		public OpeningHour LocationOpeningHours { get; set; }
		public int ReservationId { get; set; }
		public Reservation ReservationObject { get; set; }
		public ObservableCollection<CheckIn> CheckInList { get; set; }
		public LocationDetailViewModel ViewModel { get; set; }

		public LocationDetail()
		{
			this.InitializeComponent();

			ViewModel = new LocationDetailViewModel();
			DataContext = ViewModel;

			CheckInList = new ObservableCollection<CheckIn>();
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

			ViewModel.SelectedDate.Date = DateTime.Now;

			int i = 0;
			while (i < 6)
			{
				ViewModel.Dates.Add(new LocationDay()
				{
					ActiveCheckIn = CheckIfProfilehasCheckIn(ViewModel.SelectedDate.Date.AddDays(i)),
					Date = ViewModel.SelectedDate.Date.AddDays(i)
				});

				i++;
			}

			await GetLocationOpeningHours();
			await GetLocationCheckIns();
		}

		private async Task GetLocationOpeningHours()
		{
			_cts = new CancellationTokenSource();
			CancellationToken token = _cts.Token;

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
						ViewModel.IsBookable = true;
					}
				}
			}
			catch (Exception) { }
			finally
			{
				_cts = null;
			}

			await SetDateTimeTextBoxes();
		}

		private async Task GetLocationCheckIns()
		{
			_cts = new CancellationTokenSource();
			CancellationToken token = _cts.Token;

			try
			{
				CheckInList.Clear();

				var newCheckIns = new ObservableCollection<CheckIn>();

				await CheckIn.GetCheckInsAsync(token, newCheckIns, ViewModel.SelectedDate.Date, LocationObject.Id);

				foreach (var newCheckIn in newCheckIns)
				{
					if (!CheckIfCheckInExistsInList(newCheckIn))
					{
						CheckInList.Add(newCheckIn);
					}
				}
			}
			catch (Exception ex) { }
			finally
			{
				_cts = null;
			}
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

		private bool CheckIfCheckInExistsInList(CheckIn checkin)
		{
			var checkIns = CheckInList.Where(c => c.Id == checkin.Id);
			if (checkIns.Any())
			{
				return true;
			}

			return false;
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

		private void TagCheckInsListView_ItemClick(object sender, ItemClickEventArgs e)
		{
			var tagCheckIn = (CheckInKnowledgeTag)e.ClickedItem;

			CheckInList.Clear();
			foreach (var checkIn in tagCheckIn.CheckIns)
			{
				CheckInList.Add(checkIn);
			}
		}

		private async void SearchAvailabilityButton_Click(object sender, RoutedEventArgs e)
		{
			await CheckLocationAvailability();
		}

		private async void AvailableUnitsListView_ItemClick(object sender, ItemClickEventArgs e)
		{
			var selectedUnit = (AvailableUnit)e.ClickedItem;
			_cts = new CancellationTokenSource();
			CancellationToken token = _cts.Token;

			try
			{

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
					await SetDateTimeTextBoxes();
				}
			}
		}

		private async Task SetDateTimeTextBoxes()
		{
			TimeTextBlock.Text = string.Format("{0:hh\\:mm}", ViewModel.StartTime) + " - " + string.Format("{0:hh\\:mm}", ViewModel.EndTime);
		}

		private async Task CheckLocationAvailability()
		{
			CheckInButton.IsEnabled = false;

			_cts = new CancellationTokenSource();
			CancellationToken token = _cts.Token;

			try
			{
				DateTimeHyperLinkButton.IsEnabled = false;
				CheckInProgressRing.IsActive = true;
				CheckInProgressRing.Visibility = Visibility.Visible;

				AvailableUnitsListView.Visibility = Visibility.Collapsed;


				CheckInMessageTextBlock.Text = "Checking availability...";

				var availability = await Availability.GetAvailableLocations(token, LocationObject.Id, ViewModel.SelectedDate.Date, ViewModel.StartTime, ViewModel.EndTime);
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
							if (ReservationObject != null)
							{
								var checkin = await CheckIn.GetCheckInByReservation(token, ReservationId);
								if (checkin != null)
								{
									ViewModel.ProfileCheckIns.Add(checkin);
									CheckInList.Add(checkin);
									ViewModel.SelectedDate.ActiveCheckIn = checkin;

									Frame.Navigate(typeof(CheckInFinal), checkin);
								}
							}
						}
					}
					else
					{
						CheckInMessageTextBlock.Text = "Please select your space...";

						AvailableUnitsListView.ItemsSource = availableUnits;
						AvailableUnitsListView.Visibility = Visibility.Visible;
					}
				}
				else
				{
					CheckInMessageTextBlock.Text = "No availability, please try an other date or time";
				}
			}
			catch (Exception) { }
			finally
			{
				_cts = null;

				DateTimeHyperLinkButton.IsEnabled = true;
				CheckInButton.IsEnabled = true;

				CheckInProgressRing.IsActive = false;
				CheckInProgressRing.Visibility = Visibility.Collapsed;
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

		private async void DatesFlipView_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			await GetLocationCheckIns();
			await GetLocationOpeningHours();
		}

		private void LocationNameHyoperLinkButton_Click(object sender, RoutedEventArgs e)
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
