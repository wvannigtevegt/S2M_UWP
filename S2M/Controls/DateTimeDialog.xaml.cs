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
		public int LocationId { get; set; }
		public TimeSpan MinStartTime { get; set; }
		public TimeSpan MaxEndTimeSpan { get; set; }
		public TimeSpan StartTime { get; set; }
		public TimeSpan EndTime { get; set; }

		public DateTimeDialog()
		{
			this.InitializeComponent();

			this.Opened += DateTimeDialog_Opened;
			this.Closing += DateTimeDialog_Closing;
		}

		private void DateTimeDialog_Opened(ContentDialog sender, ContentDialogOpenedEventArgs args)
		{
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

		private void StartTimeTimePicker_TimeChanged(object sender, TimePickerValueChangedEventArgs e)
		{
			if (e.NewTime >= MinStartTime)
			{
				StartTime = e.NewTime;
			}
			else
			{
				StartTimeTimePicker.Time = MinStartTime;
			}
		}

		private void EndTimeTimePicker_TimeChanged(object sender, TimePickerValueChangedEventArgs e)
		{
			if (e.NewTime <= MaxEndTimeSpan)
			{
				EndTime = e.NewTime;
			}
			else
			{
				EndTimeTimePicker.Time = MaxEndTimeSpan;
			}
		}
			
	}
}
