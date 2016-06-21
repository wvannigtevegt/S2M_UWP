using S2M.Models;
using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;

namespace S2M.ViewModel
{
	public class EventsViewModel : NotificationBase
	{
		private ObservableCollection<EventCalendar> _eventList = new ObservableCollection<EventCalendar>();

		public ObservableCollection<EventCalendar> EventList
		{
			get { return _eventList; }
			set { SetProperty(ref _eventList, value); }
		}

		public async Task LoadEventsAsync()
		{
			var _cts = new CancellationTokenSource();
			CancellationToken token = _cts.Token;

			try
			{
				await Models.EventCalendar.GetEventsAsync(EventList);
			}
			catch (Exception) { }
			finally
			{
				_cts = null;
			}
		}
	}
}
