using S2M.Common;
using S2M.Models;
using System;
using System.Collections.ObjectModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace S2M.Pages {
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class CheckInDetail : Page {
		public CheckIn CheckInObject { get; set; }
		public ObservableCollection<Activity> ActivityList { get; set; }

		public CheckInDetail() {
			this.InitializeComponent();
			
			ActivityList = new ObservableCollection<Activity>();
		}

		protected async override void OnNavigatedTo(NavigationEventArgs e) {
			var checkInObject = (Models.CheckIn)e.Parameter;
			if (checkInObject != null) {
				CheckInObject = checkInObject;
			}
		}

		private async void Page_Loaded(object sender, RoutedEventArgs e) {			
			CheckInNameTextBlock.Text = CheckInObject.ProfileName;
			CheckInImageBrush.UriSource = new Uri(CheckInObject.ProfileImage_150);
			CheckInWorkingOnTextBlock.Text = CheckInObject.WorkingOn;

			if (CheckInObject.LocationId == 0)
			{
				CheckInLocationTextBlock.Text = CheckInObject.LocationName;
				CheckInLocationTextBlock.Visibility = Visibility.Visible;
				CheckInLocationHyperlinkButton.Visibility = Visibility.Collapsed;
			}

			if (CheckInObject.LocationId > 0)
			{
				CheckInLocationHyperlinkButton.Content = CheckInObject.LocationName;
				CheckInLocationHyperlinkButton.Visibility = Visibility.Visible;
				CheckInLocationTextBlock.Visibility = Visibility.Collapsed;
			}
			
			//CheckInDateTextBlock.Text = DateService.ConvertFromUnixTimestamp(CheckInObject.StartTimeStamp).ToString("yyyy-MM-dd");
			CheckInTimeTextBlock.Text = DateService.ConvertFromUnixTimestamp(CheckInObject.StartTimeStamp).ToString("HH:mm") + " - " +
											DateService.ConvertFromUnixTimestamp(CheckInObject.EndTimeStamp).ToString("HH:mm");

			CheckInTagsListView.ItemsSource = CheckInObject.Tags;
		}

		private async void StartChatButton_Click(object sender, RoutedEventArgs e) {
			var chat = await Chat.CreateChat(CheckInObject.ProfileId);
			if (chat != null)
			{
				var criteria = new ChatDetailPageCriteria
				{
					Chat = chat,
					ChatId = 0
				};

				Frame.Navigate(typeof(ChatDetail), criteria);
			}
		}

		private void CheckInLocationHyperlinkButton_Click(object sender, RoutedEventArgs e)
		{
			var criteria = new LocationDetailPageCriteria
			{
				LocationId = CheckInObject.LocationId,
				Location = null
			};

			Frame.Navigate(typeof(LocationDetail), criteria);
		}
	}
}
