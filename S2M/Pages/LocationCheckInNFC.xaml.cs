using S2M.Models;
using S2M.ViewModel;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace S2M.Pages
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class LocationCheckInNFC : Page
	{
		private CancellationTokenSource _cts = null;

		public LocationCheckInNFCViewModel ViewModel { get; set; }

		public LocationCheckInNFC()
		{
			this.InitializeComponent();

			ViewModel = new LocationCheckInNFCViewModel();

			DataContext = ViewModel;
		}

		protected async override void OnNavigatedTo(NavigationEventArgs e)
		{
			_cts = new CancellationTokenSource();
			CancellationToken token = _cts.Token;

			try
			{
				ViewModel.LocationId = (int)e.Parameter;
				await GetDataAsync();
			}
			catch (Exception) { }
			finally
			{
				_cts = null;
			}
		}

		private async Task GetDataAsync()
		{
			_cts = new CancellationTokenSource();
			CancellationToken token = _cts.Token;

			try
			{
				await ViewModel.GetLocationAsync();
				await ViewModel.GetCurrentCheckin();


				if (ViewModel.CurrentCheckin != null 
					&& ViewModel.CurrentCheckin.Id > 0 
					&& ViewModel.CurrentCheckin.LocationId == ViewModel.LocationId 
					&& !ViewModel.CurrentCheckin.HasLeft)
				{
					if (ViewModel.CurrentCheckin.IsConfirmed)
					{
						// Check-out 
						var checkoutCheckinObject = await CheckIn.Checkout(token, ViewModel.CurrentCheckin);
						if (checkoutCheckinObject != null && checkoutCheckinObject.Id > 0)
						{
							MessageTextBlock.Text = "Checked-out!";
						}
						else {
							MessageTextBlock.Text = "Cancelled!";
						}
					}

					if (!ViewModel.CurrentCheckin.IsConfirmed)
					{
						ViewModel.CurrentCheckin = await CheckIn.ConfirmCheckIn(token, ViewModel.CurrentCheckin);
						if (ViewModel.CurrentCheckin != null && ViewModel.CurrentCheckin.Id > 0)
						{
							GoToCheckinFinal();
						}
					}
				}
				if (ViewModel.CurrentCheckin == null || ViewModel.CurrentCheckin.Id == 0 || ViewModel.CurrentCheckin.HasLeft)
				{
					await GetOpeningHours(ViewModel.LocationId);
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
				await ViewModel.GetLocationOpeningHours();

				if (ViewModel.LocationOpeningHours.NrOfLocations > 0 && DateTime.Now <= ViewModel.LocationOpeningHours.MaxTimeClose)
				{
					var startTime = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, 0);
					var totalMinutes = (int)(startTime + new TimeSpan(0, 15 / 2, 0)).TotalMinutes;

					startTime = new TimeSpan(0, totalMinutes - totalMinutes % 15, 0);

					var endTime = new TimeSpan(ViewModel.LocationOpeningHours.MaxTimeClose.Hour, ViewModel.LocationOpeningHours.MaxTimeClose.Minute, 0);

					await CheckLocationAvailability(DateTime.Now, startTime, endTime);
				}
				else
				{
					MessageTextBlock.Text = "Location closed! Please try another date & time";
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

			try
			{
				var availability = await Availability.GetAvailableLocations(token, ViewModel.Location.Id, date, startTime, endTime);
				if (availability.Locations.Count > 0)
				{
					var availableLocations = availability.Locations;
					var availableLocation = availableLocations.Where(a => a.LocationId == ViewModel.Location.Id).FirstOrDefault();
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
									var checkin = await CheckIn.GetCheckInByReservation(token, reservationObject.Id);
									if (checkin != null)
									{
										ViewModel.CurrentCheckin = await CheckIn.ConfirmCheckIn(token, checkin);

										GoToCheckinFinal();
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
			}
		}

		private void GoToCheckinFinal()
		{
			var checkinCriteria = new CheckinFinalPageCriteria
			{
				IsNewCheckIn = true,
				CheckIn = ViewModel.CurrentCheckin
			};

			Frame.Navigate(typeof(CheckInFinal), checkinCriteria);
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
