using S2M.Controls;
using S2M.Models;
using S2M.ViewModel;
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
	public sealed partial class CheckInFinal : Page
	{
		private CancellationTokenSource _cts = null;
		private bool _initialLoad { get; set; }

		public CheckInFinalViewModel ViewModel { get; set; }

		public CheckInFinal()
		{
			this.InitializeComponent();

			ViewModel = new CheckInFinalViewModel();
			DataContext = ViewModel;
		}

		protected async override void OnNavigatedTo(NavigationEventArgs e)
		{
			_initialLoad = true;

			var checkinCriteria = (CheckinFinalPageCriteria)e.Parameter;
			var checkin = checkinCriteria.CheckIn;
			if (checkin != null)
			{
				ViewModel.CurrentCheckin = checkin;
				ViewModel.IsNewCheckin = checkinCriteria.IsNewCheckIn;

				if (checkin.ReservationId > 0)
				{
					await ViewModel.GetLocationOptions();
				}

				if(string.IsNullOrEmpty(checkin.WorkingOn))
				{

				}

				if(string.IsNullOrEmpty(ViewModel.CurrentCheckin.WorkingOn))
				{
					ViewModel.EditWorkingOn = true;
				}

				if (Common.DateService.ConvertFromUnixTimestamp(ViewModel.CurrentCheckin.StartTimeStamp).Date > DateTime.Now.Date)
				{
					ViewModel.ShowCancelLink = true;
					ViewModel.ShowCheckoutLink = false;
				}

				if (Common.DateService.ConvertFromUnixTimestamp(ViewModel.CurrentCheckin.StartTimeStamp).Date == DateTime.Now.Date 
						&& !ViewModel.CurrentCheckin.HasLeft)
				{
					if (DateTime.Now < Common.DateService.ConvertFromUnixTimestamp(ViewModel.CurrentCheckin.StartTimeStamp)) {
						ViewModel.ShowCancelLink = true;
						ViewModel.ShowCheckoutLink = false;
					}
					else {
						ViewModel.ShowCancelLink = false;
						ViewModel.ShowCheckoutLink = true;
					}
				}
				if (Common.DateService.ConvertFromUnixTimestamp(checkin.StartTimeStamp).Date == DateTime.Now.Date && string.IsNullOrEmpty(checkin.WorkingOn))
				{
					await ShowWorkingOnDialog();
				} 
			}

			await ViewModel.GetCheckinRecommendations();

			_initialLoad = false;
		}

		

		private async void OptionToggleSwitch_Toggled(object sender, RoutedEventArgs e)
		{
			if (!_initialLoad)
			{
				var toggleSwitch = sender as ToggleSwitch;
				if (toggleSwitch != null)
				{
					var optionId = int.Parse(toggleSwitch.Tag.ToString());
					var option = ViewModel.OptionList.Where(ol => ol.OptionId == optionId).First();
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
									await Option.SaveOptionToReservation(token, ViewModel.CurrentCheckin.ReservationId, option);
								}
								else
								{
									await Option.DeleteOptionFromReservation(token, ViewModel.CurrentCheckin.ReservationId, option.OptionId);
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
		}

		private async void EditWorkingOnHyperLinkButton_Click(object sender, RoutedEventArgs e)
		{
			await ShowWorkingOnDialog();
		}

		private async Task ShowWorkingOnDialog()
		{
			WorkingOnContentDialog dialog = new WorkingOnContentDialog();
			dialog.WorkingOn = ViewModel.CurrentCheckin.WorkingOn;
			//dialog.MinWidth = this.ActualWidth;
			//dialog.MaxWidth = this.ActualWidth;

			await dialog.ShowAsync();

			if (dialog.Result == ChangeWorkingOnResult.ChangeWorkingOnOK)
			{
				ViewModel.CurrentCheckin.WorkingOn = dialog.WorkingOn;

				var _cts = new CancellationTokenSource();
				CancellationToken token = _cts.Token;

				try
				{
					ViewModel.CurrentCheckin = await CheckIn.UpdateCheckIn(token, ViewModel.CurrentCheckin);
					await ViewModel.GetCheckinRecommendations();
				}
				catch (Exception) { }
				finally
				{
					_cts = null;
				}
			}
		}

		private async void CheckoutHyperLinkButton_Click(object sender, RoutedEventArgs e)
		{
			var _cts = new CancellationTokenSource();
			CancellationToken token = _cts.Token;

			try
			{
				await CheckIn.Checkout(token, ViewModel.CurrentCheckin);
				if (Frame.CanGoBack)
				{
					Frame.GoBack();
				}

			}
			catch (Exception) { }
			finally
			{
				_cts = null;
			}
		}

		private void CloseMessageHyperLinkButton_Click(object sender, RoutedEventArgs e)
		{
			SystemMessageRelativePanelExitStoryboard.Begin();
			ViewModel.IsNewCheckin = false;
		}

		private void OpenExtrasButton_Click(object sender, RoutedEventArgs e)
		{
			CheckInFinalSplitView.IsPaneOpen = !CheckInFinalSplitView.IsPaneOpen;
		}

		private void CheckinsListView_ItemClick(object sender, ItemClickEventArgs e)
		{
			var checkin = (CheckIn)e.ClickedItem;
			if (checkin != null)
			{
				Frame.Navigate(typeof(CheckInDetail), checkin);
			}
		}
	}

	public class CheckinFinalPageCriteria
	{
		public CheckIn CheckIn { get; set; } = null;
		public bool IsNewCheckIn { get; set; } = false;
		public bool ShowMatches { get; set; } = false;
	}
}
