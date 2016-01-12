﻿using S2M.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace S2M.Pages {
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class EventDetail : Page {
		public Event EventObject { get; set; }
		public ObservableCollection<CheckIn> CheckInList { get; set; }
		public ObservableCollection<CheckInKnowledgeTag> TagCheckInList { get; set; }

		private CancellationTokenSource _cts = null;

		public EventDetail() {
			this.InitializeComponent();

			CheckInList = new ObservableCollection<CheckIn>();
			TagCheckInList = new ObservableCollection<CheckInKnowledgeTag>();
		}

		protected async override void OnNavigatedTo(NavigationEventArgs e) {
			var eventObject = (Models.Event)e.Parameter;
			if (eventObject != null) {
				EventObject = eventObject;

				EventNameTextBlock.Text = EventObject.Name;
				EventLocationTextBlock.Text = EventObject.LocationName;
				EventDataTextBlock.Text = EventObject.Date.ToString();
			}
		}

		protected override void OnNavigatingFrom(NavigatingCancelEventArgs e) {
			if (_cts != null) {
				_cts.Cancel();
				_cts = null;
			}

			base.OnNavigatingFrom(e);
		}

		private async void Page_Loaded(object sender, RoutedEventArgs e) {
			_cts = new CancellationTokenSource();
			CancellationToken token = _cts.Token;

			try {
				await CheckIn.GetCheckInsAsync(token, CheckInList, 0, EventObject.Id, "", 0, 0, 0, "", 0, 0, false);
				await CheckInKnowledgeTag.GetEventCheckInKnowledgeTagsAsync(TagCheckInList, EventObject.Id);


			}
			catch (Exception) { }
			finally {
				_cts = null;
			}
		}

		private void CheckIfProfileAlreadyCheckedInEvent() {
			var profileCheckIn =
				from pc in CheckInList
				where pc.ProfileId == 0
				select pc;

			if (profileCheckIn.Any()) {
				EventCheckInButton.Visibility = Visibility.Collapsed;
			}
		}

		private void EventCheckInsGridView_ItemClick(object sender, ItemClickEventArgs e) {
			var checkIn = (CheckIn)e.ClickedItem;

			Frame.Navigate(typeof(CheckInDetail), checkIn);
		}

		private void TagCheckInsListView_ItemClick(object sender, ItemClickEventArgs e) {
			var tagCheckIn = (CheckInKnowledgeTag)e.ClickedItem;

			CheckInList.Clear();
			foreach (var checkIn in tagCheckIn.CheckIns) {
				CheckInList.Add(checkIn);
			}
		}

		private async void EventCheckInButton_Click(object sender, RoutedEventArgs e) {
			_cts = new CancellationTokenSource();
			CancellationToken token = _cts.Token;

			try {
				var checkIn = await CheckIn.CheckInToEvent(token, EventObject.Id);
			}
			catch (Exception) { }
			finally {
				_cts = null;
			}
		}
	}
}
