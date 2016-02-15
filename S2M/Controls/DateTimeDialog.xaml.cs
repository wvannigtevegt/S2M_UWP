using S2M.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Content Dialog item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace S2M.Controls
{
	public enum ChangeDateTimeResult
	{
		ChangeDateTimeOK,
		ChangeDateTimeCancel,
		Nothing
	}

	public sealed partial class DateTimeDialog : ContentDialog
	{
		public ChangeDateTimeResult Result { get; private set; }
		public DateTime Date { get; set; }
		public int LocationId { get; set; }
		public TimeSpan StartTime { get; set; }
		public TimeSpan EndTime { get; set; }

		private CancellationTokenSource _cts = null;

		public DateTimeDialog()
		{
			this.InitializeComponent();

			this.Opened += DateTimeDialog_Opened;
			this.Closing += DateTimeDialog_Closing;
		}

		private void DateTimeDialog_Opened(ContentDialog sender, ContentDialogOpenedEventArgs args)
		{
			//StartDateDatePicker.Date = Date;
			//StartDateDatePicker.MinYear = DateTimeOffset.Now;

			StartDateCalendarView.SelectionMode = CalendarViewSelectionMode.Single;
			StartDateCalendarView.MinDate = DateTime.Now;
			StartDateCalendarView.SelectedDates.Add(Date);

			StartTimeTimePicker.ClockIdentifier = Windows.Globalization.ClockIdentifiers.TwentyFourHour;
			StartTimeTimePicker.Time = StartTime;

			EndTimeTimePicker.ClockIdentifier = Windows.Globalization.ClockIdentifiers.TwentyFourHour;
			EndTimeTimePicker.Time = EndTime;
		}

		private void DateTimeDialog_Closing(ContentDialog sender, ContentDialogClosingEventArgs args)
		{
		}

		private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
		{
			this.Result = ChangeDateTimeResult.ChangeDateTimeOK;
		}

		private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
		{
			this.Result = ChangeDateTimeResult.ChangeDateTimeCancel;
		}

		private async void StartDateDatePicker_DateChanged(object sender, DatePickerValueChangedEventArgs e)
		{
			Date = e.NewDate.DateTime;
			if (Date.Date == DateTime.Now.Date)
			{
				StartTime = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, 0);
				StartTime = TimeSpan.FromMinutes(15 * Math.Ceiling(StartTime.TotalMinutes / 15));
				StartTimeTimePicker.Time = StartTime;
				EndTime = new TimeSpan(17, 0, 0);
				EndTimeTimePicker.Time = EndTime;
			}
			else
			{
				StartTime = new TimeSpan(9, 0, 0);
				StartTimeTimePicker.Time = StartTime;
				EndTime = new TimeSpan(17, 0, 0);
				EndTimeTimePicker.Time = EndTime;
			}

			await GetOpeningHours();
		}

		private void StartTimeTimePicker_TimeChanged(object sender, TimePickerValueChangedEventArgs e)
		{
			StartTime = e.NewTime;
		}

		private void EndTimeTimePicker_TimeChanged(object sender, TimePickerValueChangedEventArgs e)
		{
			EndTime = e.NewTime;
		}

		private async Task GetOpeningHours()
		{
			_cts = new CancellationTokenSource();
			CancellationToken token = _cts.Token;

			try
			{
				var openingHour = await OpeningHour.GetLocationOpeningHourssAsync(token, LocationId, Date);
				if (openingHour != null && openingHour.NrOfLocations > 0)
				{
					//StartTimeTimePicker.Mi
				}
			}
			catch (Exception) { }
			finally
			{
				_cts = null;
			}
		}

		private async void StartDateCalendarView_SelectedDatesChanged(CalendarView sender, CalendarViewSelectedDatesChangedEventArgs args)
		{
			if (args.AddedDates.Any())
			{
				var dateTimeOffSet = args.AddedDates.First();
				Date = new DateTime(dateTimeOffSet.Year, dateTimeOffSet.Month, dateTimeOffSet.Day);

				if (Date.Date == DateTime.Now.Date)
				{
					StartTime = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, 0);
					StartTime = TimeSpan.FromMinutes(15 * Math.Ceiling(StartTime.TotalMinutes / 15));
					StartTimeTimePicker.Time = StartTime;
					EndTime = new TimeSpan(17, 0, 0);
					EndTimeTimePicker.Time = EndTime;
				}
				else
				{
					StartTime = new TimeSpan(9, 0, 0);
					StartTimeTimePicker.Time = StartTime;
					EndTime = new TimeSpan(17, 0, 0);
					EndTimeTimePicker.Time = EndTime;
				}

				await GetOpeningHours();
			}
			
		}
	}
}
