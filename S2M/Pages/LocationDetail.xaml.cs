using S2M.Controls;
using S2M.Models;
using System;
using System.Collections.ObjectModel;
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
	public sealed partial class LocationDetail : Page
	{
		public Location LocationObject { get; set; }
		public ObservableCollection<Activity> ActivityList { get; set; }
		public Cart CartObject { get; set; }
		public ObservableCollection<CheckIn> CheckInList { get; set; }
		public ObservableCollection<Option> OptionList { get; set; }
		public ObservableCollection<CheckInKnowledgeTag> TagCheckInList { get; set; }

		protected string CartKey { get; set; }
		protected DateTime Date { get; set; }
		protected TimeSpan EndTime { get; set; }
		protected TimeSpan StartTime { get; set; }

		private CancellationTokenSource _cts = null;

		public LocationDetail()
		{
			this.InitializeComponent();

			ActivityList = new ObservableCollection<Activity>();
			CheckInList = new ObservableCollection<CheckIn>();
			OptionList = new ObservableCollection<Option>();
			TagCheckInList = new ObservableCollection<CheckInKnowledgeTag>();
		}

		protected async override void OnNavigatedTo(NavigationEventArgs e)
		{
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
			LocationNameTextBlock.Text = LocationObject.Name;

			var addressLine = "";
			if (!string.IsNullOrEmpty(LocationObject.Address))
			{
				addressLine += LocationObject.Address;
			}
			if (!string.IsNullOrEmpty(LocationObject.Zipcode))
			{
				addressLine += ", " + LocationObject.Zipcode;
			}
			if (!string.IsNullOrEmpty(LocationObject.City))
			{
				addressLine += ", " + LocationObject.City;
			}
			if (!string.IsNullOrEmpty(LocationObject.State))
			{
				addressLine += " " + LocationObject.State;
			}
			LocationAddressLineTextBlock.Text = addressLine;

			Date = DateTime.Now;
			
			StartTime = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, 0);
			StartTime = TimeSpan.FromMinutes(15 * Math.Ceiling(StartTime.TotalMinutes / 15));
			EndTime = new TimeSpan(17, 0, 0);

			await SetDateTimeTextBoxes();

			_cts = new CancellationTokenSource();
			CancellationToken token = _cts.Token;

			try
			{
				var locationText = await LocationText.GetLocationDescriptionAsync(token, LocationObject.Id);
				//LocationDescriptionTextBlock.Text = locationText.Description;

				await CheckIn.GetCheckInsAsync(token, CheckInList, LocationObject.Id);
				await CheckInKnowledgeTag.GetLocationCheckInKnowledgeTagsAsync(token, TagCheckInList, LocationObject.Id);
			}
			catch (Exception) { }
			finally
			{
				_cts = null;
			}
		}

		private void LocationCheckInsGridView_ItemClick(object sender, ItemClickEventArgs e)
		{
			var checkIn = (CheckIn)e.ClickedItem;

			Frame.Navigate(typeof(CheckInDetail), checkIn);
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
				await Cart.SetCarUnit(token, CartKey, selectedUnit.SearchDateId, selectedUnit.UnitId, selectedUnit.CurrencyId, (double)selectedUnit.Price, selectedUnit.TaxId, selectedUnit.Crc);
			}
			catch (Exception) { }
			finally
			{
				_cts = null;
			}
		}

		private async void OptionToggleSwitch_Toggled(object sender, RoutedEventArgs e)
		{
			var toggleSwitch = sender as ToggleSwitch;
			if (toggleSwitch != null)
			{
				var optionId = int.Parse(toggleSwitch.Tag.ToString());
				var option = OptionList.Where(ol => ol.OptionId == optionId).First();
				if (option != null)
				{
					if (option.IsEnabled)
					{
						_cts = new CancellationTokenSource();
						CancellationToken token = _cts.Token;

						try
						{
							if (toggleSwitch.IsOn)
							{
								await Option.SaveOptionToCart(token, CartObject.CartKey, option);
							}
							else
							{
								await Option.DeleteOptionFromCart(token, CartObject.CartKey, option.OptionId);
							}
						}
						catch (Exception) { }
						finally
						{
							_cts = null;
						}
					}
				}
			}
		}

		private async void FinalizeCartButton_Click(object sender, RoutedEventArgs e)
		{
			_cts = new CancellationTokenSource();
			CancellationToken token = _cts.Token;

			CheckInProgressRing.IsActive = true;
			CheckInProgressRing.Visibility = Visibility.Visible;

			SearchAvailabilityButton.Visibility = Visibility.Collapsed;
			FinalizeCartButton.Visibility = Visibility.Collapsed;
			AvailableUnitsListView.Visibility = Visibility.Collapsed;
			OptionsListView.Visibility = Visibility.Collapsed;

			try
			{
				await CartObject.FinalizeCart(token);
			}
			catch (Exception) { }
			finally
			{
				_cts = null;

				SearchAvailabilityButton.Visibility = Visibility.Visible;

				CheckInProgressRing.IsActive = false;
				CheckInProgressRing.Visibility = Visibility.Collapsed;
			}
		}

		private async void DateTimeHyperLinkButton_Click(object sender, RoutedEventArgs e)
		{
			Controls.DateTimeDialog dateDialog = new Controls.DateTimeDialog();
			dateDialog.Date = Date;
			dateDialog.LocationId = LocationObject.Id;
			dateDialog.StartTime = StartTime;
			dateDialog.EndTime = EndTime;

			await dateDialog.ShowAsync();
			if (dateDialog.Result == ChangeDateTimeResult.ChangeDateTimeOK)
			{
				var changeCounter = 0;

				if (dateDialog.Date != Date)
				{
					Date = dateDialog.Date;
					changeCounter++;
				}
				if (dateDialog.StartTime != StartTime)
				{
					StartTime = dateDialog.StartTime;
					changeCounter++;
				}
				if (dateDialog.EndTime != EndTime)
				{
					EndTime = dateDialog.EndTime;
					changeCounter++;
				}

				if (changeCounter > 0)
				{
					await SetDateTimeTextBoxes();
					await CheckLocationAvailability();
				}
			}
		}

		private async Task SetDateTimeTextBoxes()
		{
			DateTextBlock.Text = Date.ToString("dd MMM yyyy");
			TimeTextBlock.Text = string.Format("{0:hh\\:mm}", StartTime) + " - " + string.Format("{0:hh\\:mm}", EndTime);

			//await CheckLocationAvailability();
		}

		private async Task CheckLocationAvailability()
		{
			SearchAvailabilityButton.IsEnabled = false;

			_cts = new CancellationTokenSource();
			CancellationToken token = _cts.Token;

			try
			{

				DateTimeHyperLinkButton.IsEnabled = false;
				CheckInProgressRing.IsActive = true;
				CheckInProgressRing.Visibility = Visibility.Visible;

				NoAvailabilityTextBlock.Visibility = Visibility.Collapsed;
				AvailableUnitsListView.Visibility = Visibility.Collapsed;
				OptionsListView.Visibility = Visibility.Collapsed;
				FinalizeCartButton.Visibility = Visibility.Collapsed;
				
				OptionList.Clear();

				var availability = await Availability.GetAvailableLocations(token, LocationObject.Id, Date, StartTime, EndTime);
				if (availability.Locations.Count > 0)
				{
					var availableLocation = availability.Locations.First();
					var availableUnits = availableLocation.Units;

					var selectedUnit = availableUnits.First();
					if (selectedUnit != null)
					{
						CartObject = await Availability.SelectAvailableLocation(token, availability.SearchKey, availableLocation.LocationId, selectedUnit.SearchDateId, selectedUnit.UnitId, 0);
						if (CartObject != null)
						{
							SearchIdTextBlock.Text = selectedUnit.SearchDateId.ToString(); // TODO: remove test value

							AvailableUnitsListView.ItemsSource = availableUnits;
							AvailableUnitsListView.SelectedItem = selectedUnit;

							AvailableUnitsListView.Visibility = Visibility.Visible;

							await Option.GetLocationOptionsAsync(token, CartObject.CartKey, OptionList);
							if (OptionList.Any())
							{
								OptionsListView.Visibility = Visibility.Visible;
								OptionsListView.ItemsSource = OptionList;
							}

							FinalizeCartButton.Visibility = Visibility.Visible;
						}
					}
				}
				else
				{
					NoAvailabilityTextBlock.Visibility = Visibility.Visible;
				}
			}
			catch (Exception) { }
			finally
			{
				_cts = null;

				DateTimeHyperLinkButton.IsEnabled = true;
				SearchAvailabilityButton.IsEnabled = true;

				CheckInProgressRing.IsActive = false;
				CheckInProgressRing.Visibility = Visibility.Collapsed;
			}

		}
	}

	public class LocationDetailPageCriteria
	{
		public int LocationId { get; set; } = 0;
		public Location Location { get; set; } = null;
	}
}
