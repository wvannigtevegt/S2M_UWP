using S2M.Models;
using System;
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
	public sealed partial class LocationCheckInNFC : Page
	{
		protected CheckIn CheckInObject { get; set; }
		protected Location LocationObject { get; set; }
		protected OpeningHour LocationOpeningHours { get; set; }

		private CancellationTokenSource _cts = null;

		public LocationCheckInNFC()
		{
			this.InitializeComponent();
		}

		protected async override void OnNavigatedTo(NavigationEventArgs e)
		{
			_cts = new CancellationTokenSource();
			CancellationToken token = _cts.Token;

			try
			{
				var locationId = (int)e.Parameter;
				await GetDataAsync(locationId);
			}
			catch (Exception) { }
			finally
			{
				_cts = null;
			}
		}

		private async Task GetDataAsync(int locationId)
		{
			_cts = new CancellationTokenSource();
			CancellationToken token = _cts.Token;

			LocationCheckInProgressRing.IsActive = true;
			LocationCheckInProgressRing.Visibility = Visibility.Visible;

			try
			{
				LocationObject = await Location.GetLocationById(token, locationId);
				if (LocationObject != null && LocationObject.Id > 0)
				{
					LocationNameTextBlock.Text = LocationObject.Name;
				}
				LocationCheckInProgressRing.IsActive = false;
				LocationCheckInProgressRing.Visibility = Visibility.Collapsed;


				CheckInObject = await CheckIn.GetCurrentCheckIn(token);
				if (CheckInObject != null && CheckInObject.Id > 0 && CheckInObject.LocationId == locationId && !CheckInObject.HasLeft)
				{
					if (CheckInObject.IsConfirmed)
					{
						// Check-out 
						var checkoutCheckinObject = await CheckIn.Checkout(token, CheckInObject);
						if (checkoutCheckinObject != null && checkoutCheckinObject.Id > 0)
						{
							MessageTextBlock.Text = "Checked-out!";
						}
						else {
							MessageTextBlock.Text = "Cancelled!";
						}
					}

					if (!CheckInObject.IsConfirmed)
					{
						var confirmCheckinObject = await CheckIn.ConfirmCheckIn(token, CheckInObject);
						if (confirmCheckinObject != null && confirmCheckinObject.Id > 0)
						{
							// Check-in is confirmed
							MessageTextBlock.Text = "Confirmed!";

							// TODO: check or set working on and get matches
						}
					}
				}
				if (CheckInObject == null || CheckInObject.Id == 0 || CheckInObject.HasLeft)
				{
					await GetOpeningHours(locationId);
				}
			}
			catch (Exception) { }
			finally
			{
				_cts = null;
			}
		}

		private async Task GetOpeningHours(int locationId)
		{
			_cts = new CancellationTokenSource();
			CancellationToken token = _cts.Token;

			try
			{
				LocationOpeningHours = await OpeningHour.GetLocationOpeningHourssAsync(token, locationId, DateTime.Now);
				if (LocationOpeningHours.NrOfLocations > 0 && DateTime.Now <= LocationOpeningHours.MaxTimeClose)
				{
					var startTime = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, 0);
					//startTime = TimeSpan.FromMinutes(15 * Math.Ceiling(startTime.TotalMinutes / 15));
					var totalMinutes = (int)(startTime + new TimeSpan(0, 15 / 2, 0)).TotalMinutes;

					startTime = new TimeSpan(0, totalMinutes - totalMinutes % 15, 0);

					var endTime = new TimeSpan(LocationOpeningHours.MaxTimeClose.Hour, LocationOpeningHours.MaxTimeClose.Minute, 0);

					await CheckLocationAvailability(DateTime.Now, startTime, endTime);
				}
			}
			catch (Exception) { }
			finally
			{
				_cts = null;
			}
		}

		private async Task CheckLocationAvailability(DateTime date, TimeSpan startTime, TimeSpan endTime)
		{
			_cts = new CancellationTokenSource();
			CancellationToken token = _cts.Token;

			LocationCheckInProgressRing.IsActive = true;
			LocationCheckInProgressRing.Visibility = Visibility.Visible;

			try
			{
				var availability = await Availability.GetAvailableLocations(token, LocationObject.Id, date, startTime, endTime);
				if (availability.Locations.Count > 0)
				{
					var availableLocations = availability.Locations;
					var availableLocation = availableLocations.Where(a => a.LocationId == LocationObject.Id).FirstOrDefault();
					if (availableLocation != null)
					{
						if (availableLocation.Units.Count() == 1)
						{
							var selectedUnit = availableLocation.Units.First();
							if (selectedUnit != null)
							{
								var reservationObject = await CheckinUser(availability.SearchKey, availableLocation.LocationId, selectedUnit.SearchDateId, selectedUnit.UnitId);
								if (reservationObject != null)
								{
									MessageTextBlock.Text = "Checked-in!! ReservationId is: " + reservationObject.Id;

									var checkin = await CheckIn.GetCheckInByReservation(token, reservationObject.Id);
									if (checkin != null)
									{
										CheckInObject = await CheckIn.ConfirmCheckIn(token, checkin);
									}

									// TODO: check or set working on and get matches
								}
							}
						}
						else
						{
							// TODO: show interface for selecting the workspace type
						}
					}
				}
			}
			catch (Exception) { }
			finally
			{
				_cts = null;

				LocationCheckInProgressRing.IsActive = false;
				LocationCheckInProgressRing.Visibility = Visibility.Collapsed;
			}
		}

		private async Task<Reservation> CheckinUser(string searchKey, int locationId, int searchDateId, int unitId)
		{
			_cts = new CancellationTokenSource();
			CancellationToken token = _cts.Token;

			try
			{
				var reservation = await Availability.SelectAvailableLocation(token, searchKey, locationId, searchDateId, unitId, 0);
				return reservation;
			}
			catch (Exception) { }
			finally
			{
				_cts = null;
			}

			return null;
		}

		private async Task GetMatchingCheckins()
		{

		}
	}
}
