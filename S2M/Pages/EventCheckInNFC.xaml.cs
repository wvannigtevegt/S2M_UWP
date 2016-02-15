using S2M.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace S2M.Pages
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class EventCheckInNFC : Page
	{
		public EventCalendar EventObject { get; set; }
		private CancellationTokenSource _cts = null;

		public EventCheckInNFC()
		{
			this.InitializeComponent();
		}

		protected async override void OnNavigatedTo(NavigationEventArgs e)
		{
			_cts = new CancellationTokenSource();
			CancellationToken token = _cts.Token;

			var eventId = (int)e.Parameter;
			EventObject = await EventCalendar.GetEventCalendarById(token, eventId);
			if (EventObject != null && EventObject.Id > 0)
			{
				EventCheckInProgressRing.IsActive = true;
				EventCheckInProgressRing.Visibility = Visibility.Visible;

				var checkIn = await CheckIn.CheckInToEvent(token, EventObject.Id);
				if (checkIn != null)
				{
					EventCheckInProgressRing.IsActive = false;
					EventCheckInProgressRing.Visibility = Visibility.Collapsed;

					Frame.Navigate(typeof(EventDetail), EventObject);
				}

				EventCheckInProgressRing.IsActive = false;
				EventCheckInProgressRing.Visibility = Visibility.Collapsed;
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
	}
}
