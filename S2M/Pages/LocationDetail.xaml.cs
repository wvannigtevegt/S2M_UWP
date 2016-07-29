using S2M.Controls;
using S2M.Models;
using S2M.ViewModel;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace S2M.Pages
{
	public sealed partial class LocationDetail : Page
	{
		private CancellationTokenSource _cts = null;
		private string _selectedDate = "";
		private bool _setDatesDone { get; set; }

		public CheckIn ActiveCheckIn { get; set; }
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
					ViewModel.Location = criteria.Location;
					ViewModel.IsBookmarked = criteria.Location.IsBookmarked;
				}
				if (criteria.LocationId > 0 && (ViewModel.Location == null || ViewModel.Location.Id == 0))
				{
					ViewModel.LocationId = criteria.LocationId;

					_cts = new CancellationTokenSource();
					CancellationToken token = _cts.Token;

					try
					{
						ViewModel.Location = await Location.GetLocationById(token, criteria.LocationId);
					}
					catch (Exception ex) { }
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

			//var localSettings = ApplicationData.Current.LocalSettings;
			//localSettings.Values["SelectedDate"] = ViewModel.SelectedDate.Date.ToString("yyyy-MM-dd");
		}

		private async void Page_Loaded(object sender, RoutedEventArgs e)
		{
			var curentDate = DateTime.Now;
			var dates = new ObservableCollection<LocationDay>();
			var lastSelectedDate = new LocationDay();

			_setDatesDone = false;

			int i = 0;
			while (i < 7)
			{
				dates.Add(new LocationDay()
				{
					ActiveCheckIn = null,
					Date = curentDate.AddDays(i)
				});

				i++;
			}


			//var localSettings = ApplicationData.Current.LocalSettings;
			//if (localSettings.Values["SelectedDate"] != null)
			//{
			//	_selectedDate = (string)localSettings.Values["SelectedDate"];

			//	var locationDays = dates.Where(d => d.Date.ToString("yyyy-MM-dd") == _selectedDate);
			//	if (locationDays.Any())
			//	{
			//		var locationDay = locationDays.FirstOrDefault();
			//		if (locationDay != null && locationDay.Date.Year != 1900)
			//		{
			//			lastSelectedDate = locationDay;
			//		}
			//	}
			//}

			ViewModel.Dates = dates;
			DatesFlipView.SelectedIndex = 1;

			_setDatesDone = true;

			DatesFlipView.SelectedIndex = 0;

			//if (lastSelectedDate != null && lastSelectedDate.Date.Year != 1)
			//{
			//	ViewModel.SelectedDate = lastSelectedDate;
			//}
			//else
			//{
			//	ViewModel.SelectedDate = ViewModel.Dates[0];
			//}


			await ViewModel.GetProfileCheckIns();

		}

		private void SetAvailabilityMessage(string message)
		{
			AvailabilityMessageStackPanelEnterStoryboard.Begin();

			ViewModel.AvailabilityMessage = message;
		}

		private void SetDateTimeTextBoxes()
		{
			AvailabilityMessageStackPanelExitStoryboard.Begin();

			TimeTextBlock.Text = string.Format("{0:hh\\:mm}", ViewModel.StartTime) + " - " + string.Format("{0:hh\\:mm}", ViewModel.EndTime);
		}

		private async Task<bool> CheckIfProfilehasCheckIn()
		{
			var _cts = new CancellationTokenSource();
			CancellationToken token = _cts.Token;

			var hasCheckIn = true;

			try
			{
				var date = ViewModel.SelectedDate.Date;
				var startTime = new DateTime(date.Year, date.Month, date.Day, ViewModel.StartTime.Hours, ViewModel.StartTime.Minutes, 0);
				var endTime = new DateTime(date.Year, date.Month, date.Day, ViewModel.EndTime.Hours, ViewModel.EndTime.Minutes, 0);
				hasCheckIn = await CheckIn.CheckOverlap(token, startTime, endTime);
				
			}
			catch (Exception ex) { }
			finally
			{
				_cts = null;
			}

			return hasCheckIn;
		}

		private async void LocationCheckInsGridView_ItemClick(object sender, ItemClickEventArgs e)
		{
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

		private async void SearchAvailabilityButton_Click(object sender, RoutedEventArgs e)
		{
			await CheckLocationAvailability();
		}

		private async void AvailableUnitsListView_ItemClick(object sender, ItemClickEventArgs e)
		{
			ViewModel.AvailableUnits = null;
			ViewModel.ShowWorkspaceSelection = false;

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
			dateDialog.LocationId = ViewModel.Location.Id;
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

		private async Task CheckLocationAvailability()
		{
			_cts = new CancellationTokenSource();
			CancellationToken token = _cts.Token;

			try
			{
				DateTimeHyperLinkButton.IsEnabled = false;

				ViewModel.EnableButton = false;
				ViewModel.ShowDateTimeCheckin = true;
				ViewModel.ShowSpinner = true;
				ViewModel.ShowWorkspaceSelection = false;

				var availability = await Availability.GetAvailableLocations(token, ViewModel.Location.Id, ViewModel.SelectedDate.Date, ViewModel.StartTime, ViewModel.EndTime);
				ViewModel.SearchKey = availability.SearchKey;

				if (availability.Locations.Count > 0)
				{
					ViewModel.AvailableUnits = new ObservableCollection<AvailableUnit>();

					var availableLocation = availability.Locations.First();
					foreach (var unit in availableLocation.Units)
					{
						ViewModel.AvailableUnits.Add(unit);
					}

					if (ViewModel.AvailableUnits.Count() == 1)
					{
						var selectedUnit = ViewModel.AvailableUnits.First();
						if (selectedUnit != null)
						{
							ReservationObject = await CheckinUser(availability.SearchKey, availableLocation.LocationId, selectedUnit.SearchDateId, selectedUnit.UnitId);
							await ParseReservationData();
						}
					}
					else
					{
						ViewModel.ShowDateTimeCheckin = false;
						ViewModel.ShowWorkspaceSelection = true;
					}
				}
				else
				{
					ViewModel.ShowDateTimeCheckin = false;
					//ViewModel.AvailabilityMessage = ;
					SetAvailabilityMessage("No availability, please try an other date or time");
				}


			}
			catch (Exception ex) { }
			finally
			{
				_cts = null;

				DateTimeHyperLinkButton.IsEnabled = true;
				ViewModel.ShowSpinner = false;
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

						var checkinCriteria = new CheckinFinalPageCriteria
						{
							IsNewCheckIn = true,
							CheckIn = checkin
						};

						Frame.Navigate(typeof(CheckInFinal), checkinCriteria);
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
			if (_setDatesDone)
			{
				AvailabilityMessageStackPanelExitStoryboard.Begin();

				ViewModel.AvailabilityMessage = "";
				ViewModel.AvailableUnits = null;
				ViewModel.EnableButton = true;
				ViewModel.ShowDateTimeCheckin = true;
				ViewModel.ShowWorkspaceSelection = false;
				ViewModel.IsOpen = false;
				ViewModel.AlreadyCheckedin = false;
				ViewModel.IsBookable = false;

				await ViewModel.GetLocationOpeningHours();
				await ViewModel.GetLocationCheckIns();

				await ParseDateData();
			}
		}

		private async Task ParseDateData()
		{
			var message = "";

			if (ViewModel.SelectedDate != null)
			{
				if (ViewModel.SelectedDate.Date.Date >= DateTime.Now.Date)
				{
					if (ViewModel.SelectedDate.Date.Date == DateTime.Now.Date)
					{
						if (ViewModel.LocationOpeningHours.MinTimeOpen > DateTime.Now)
						{
							ViewModel.StartTime = new TimeSpan(ViewModel.LocationOpeningHours.MinTimeOpen.Hour, ViewModel.LocationOpeningHours.MinTimeOpen.Minute, 0);
						}
						else
						{
							ViewModel.StartTime = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, 0);
							ViewModel.StartTime = TimeSpan.FromMinutes(15 * Math.Ceiling(ViewModel.StartTime.TotalMinutes / 15));
						}
					}
					else
					{
						ViewModel.StartTime = new TimeSpan(ViewModel.LocationOpeningHours.MinTimeOpen.Hour, ViewModel.LocationOpeningHours.MinTimeOpen.Minute, 0);
					}
					ViewModel.EndTime = new TimeSpan(ViewModel.LocationOpeningHours.MaxTimeClose.Hour, ViewModel.LocationOpeningHours.MaxTimeClose.Minute, 0);

					if (ViewModel.LocationOpeningHours.NrOfLocations > 0)
					{
						if (ViewModel.SelectedDate.Date.Date != DateTime.Now.Date || (ViewModel.SelectedDate.Date.Date == DateTime.Now.Date && DateTime.Now <= ViewModel.LocationOpeningHours.MaxTimeClose))
						{
							ViewModel.IsOpen = true;
						}
					}
				}

				ViewModel.AlreadyCheckedin = false;

				var hasCheckin = await CheckIfProfilehasCheckIn();
				if (hasCheckin)
				{
					ViewModel.AlreadyCheckedin = true;
				}
			}


			if (ViewModel.IsOpen && !ViewModel.AlreadyCheckedin)
			{
				ViewModel.IsBookable = true;
			}
			else
			{
				ViewModel.IsBookable = false;

				if (!ViewModel.IsOpen)
				{
					message = "Location is closed!";
				}
				if (ViewModel.AlreadyCheckedin)
				{
					message = "You're already checked-in";
				}
			}

			if (ViewModel.IsBookable)
			{
				SetDateTimeTextBoxes();
			}

			if (!ViewModel.IsBookable)
			{
				SetAvailabilityMessage(message);
			}
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

		private async void FavoriteHyperLinkButton_Click(object sender, RoutedEventArgs e)
		{
			var _cts = new CancellationTokenSource();
			CancellationToken token = _cts.Token;

			var location = ViewModel.Location;

			try
			{
				await Location.LocationBookmarked(token, ViewModel.Location.Id);
				location.IsBookmarked = true;
				ViewModel.IsBookmarked = true;
			}
			catch (Exception) { }
			finally
			{
				_cts = null;
				ViewModel.Location = location;
			}
		}

		private async void NoFavoriteHyperLinkButton_Click(object sender, RoutedEventArgs e)
		{
			var _cts = new CancellationTokenSource();
			CancellationToken token = _cts.Token;

			var location = ViewModel.Location;

			try
			{
				await Location.LocationNotBookmarked(token, ViewModel.Location.Id);
				location.IsBookmarked = false;
				ViewModel.IsBookmarked = false;
			}
			catch (Exception) { }
			finally
			{
				_cts = null;
				ViewModel.Location = location;
			}
		}

		private async void LocationDescriptionHyperLinkButton_Click(object sender, RoutedEventArgs e)
		{
			LocationDescriptionPopup.Width = ActualWidth;
			LocationDescriptionPopup.Height = ActualHeight;

			await ViewModel.GetLocationDescription();

			LocationDescriptionPopup.IsOpen = true;
		}

		private void CloseLocationDescriptionButton_Click(object sender, RoutedEventArgs e)
		{
			LocationDescriptionPopup.IsOpen = false;
		}
	}

	public class LocationDetailPageCriteria
	{
		public int LocationId { get; set; } = 0;
		public Location Location { get; set; } = null;
	}
}
