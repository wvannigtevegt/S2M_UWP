using S2M.Models;
using System.Collections.ObjectModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace S2M.Pages
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class Events : Page {
		public ObservableCollection<EventCalendar> EventList { get; set; }

		public Events() {
			this.InitializeComponent();

			EventList = new ObservableCollection<EventCalendar>();
		}

		private async void Page_Loaded(object sender, RoutedEventArgs e) {
			EventsProgressRing.IsActive = true;
			EventsProgressRing.Visibility = Visibility.Visible;

			await EventCalendar.GetEventsAsync(EventList);

			EventsProgressRing.IsActive = false;
			EventsProgressRing.Visibility = Visibility.Collapsed;
		}

		private void EventsGridView_ItemClick(object sender, ItemClickEventArgs e) {
			var eventObj = ((EventCalendar)e.ClickedItem);

			Frame.Navigate(typeof(EventDetail), eventObj);
		}
	}
}
