using S2M.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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

namespace S2M.Pages {
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class Events : Page {
		public ObservableCollection<Event> EventList { get; set; }

		public Events() {
			this.InitializeComponent();

			EventList = new ObservableCollection<Event>();
		}

		private async void Page_Loaded(object sender, RoutedEventArgs e) {
			EventsProgressRing.IsActive = true;
			EventsProgressRing.Visibility = Visibility.Visible;

			await Event.GetEventsAsync(EventList);

			EventsProgressRing.IsActive = false;
			EventsProgressRing.Visibility = Visibility.Collapsed;
		}

		private void EventsGridView_ItemClick(object sender, ItemClickEventArgs e) {
			var eventObj = ((Event)e.ClickedItem);

			Frame.Navigate(typeof(EventDetail), eventObj);
		}
	}
}
