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
	public sealed partial class Archive : Page {
		public ObservableCollection<Chat> ChatList { get; set; }

		public Archive() {
			this.InitializeComponent();

			ChatList = new ObservableCollection<Chat>();
		}

		private async void Page_Loaded(object sender, RoutedEventArgs e) {

			ChatsProgressRing.IsActive = true;
			ChatsProgressRing.Visibility = Visibility.Visible;

			await Chat.GetProfileChatsAsync(ChatList);

			ChatsProgressRing.IsActive = false;
			ChatsProgressRing.Visibility = Visibility.Collapsed;

		}

		private void ChatsListView_ItemClick(object sender, ItemClickEventArgs e) {
			var chat = (Chat)e.ClickedItem;
			if (chat != null) {
				Frame.Navigate(typeof(ChatDetail), chat);
			}
		}
	}
}
