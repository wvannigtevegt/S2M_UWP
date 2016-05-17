﻿using S2M.Models;
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

		public CheckInFinalViewModel ViewModel { get; set; }

		public CheckInFinal()
		{
			this.InitializeComponent();

			ViewModel = new CheckInFinalViewModel();
			DataContext = ViewModel;
		}

		protected async override void OnNavigatedTo(NavigationEventArgs e)
		{
			var checkin = (Models.CheckIn)e.Parameter;
			if (checkin != null)
			{
				ViewModel.CheckIn = checkin;

				if (checkin.ReservationId > 0)
				{
					ViewModel.ReservationId = checkin.ReservationId;
					await GetLocationOptions(checkin.ReservationId);
				}
			}
		}

		private async Task GetLocationOptions(int reservationId)
		{
			_cts = new CancellationTokenSource();
			CancellationToken token = _cts.Token;

			try
			{
				await Option.GetLocationOptionsAsync(token, reservationId, ViewModel.OptionList);
				if (ViewModel.OptionList.Any())
				{
					OptionsListView.ItemsSource = ViewModel.OptionList;
				}
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
								await Option.SaveOptionToReservation(token, ViewModel.ReservationId, option);
							}
							else
							{
								await Option.DeleteOptionFromReservation(token, ViewModel.ReservationId, option.OptionId);
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

		private void SetWorkingOnButton_Click(object sender, RoutedEventArgs e)
		{
			ViewModel.EditWorkingOn = true;
		}

		private async void SaveWorkingOnButton_Click(object sender, RoutedEventArgs e)
		{
			ViewModel.EditWorkingOn = false;

			var _cts = new CancellationTokenSource();
			CancellationToken token = _cts.Token;

			try
			{
				await CheckIn.UpdateCheckIn(token, ViewModel.CheckIn);
			}
			catch (Exception) { }
			finally
			{
				_cts = null;
			}
		}

		private async void CheckoutHyperLinkButton_Click(object sender, RoutedEventArgs e)
		{
			var _cts = new CancellationTokenSource();
			CancellationToken token = _cts.Token;

			try
			{
				await CheckIn.Checkout(token, ViewModel.CheckIn);
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
	}
}
