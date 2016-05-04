using S2M.Common;
using S2M.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace S2M.Pages {
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class CheckInDetail : Page {
		public ObservableCollection<ChatMessage> ChatMessageList { get; set; }
		public Chat ChatObject { get; set; }
		public CheckIn CheckInObject { get; set; }
		public ObservableCollection<Activity> ActivityList { get; set; }

		private DispatcherTimer _timer;

		public CheckInDetail() {
			this.InitializeComponent();
			
			ActivityList = new ObservableCollection<Activity>();
			ChatMessageList = new ObservableCollection<ChatMessage>();
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
			//CheckInWorkingOnTextBlock.Text = CheckInObject.WorkingOn;

			CheckInLocationTextBlock.Text = CheckInObject.LocationName;
			
			if (CheckInObject.IsConfirmed && !CheckInObject.HasLeft)
			{
				IsConfirmedFontIcon.Foreground = new SolidColorBrush(Colors.Green);
			}

			if (CheckInObject.HasLeft)
			{
				IsConfirmedFontIcon.Foreground = new SolidColorBrush(Colors.Gray);
			}

			var authenticatedProfile = await Common.StorageService.RetrieveObjectAsync<Models.Profile>("Profile");
			if (CheckInObject.ProfileId == authenticatedProfile.Id)
			{
				CheckInContactStackPanel.Visibility = Visibility.Collapsed;
			}

			//CheckInDateTextBlock.Text = DateService.ConvertFromUnixTimestamp(CheckInObject.StartTimeStamp).ToString("yyyy-MM-dd");
			CheckInTimeTextBlock.Text = DateService.ConvertFromUnixTimestamp(CheckInObject.StartTimeStamp).ToLocalTime().ToString("HH:mm") + " - " +
											DateService.ConvertFromUnixTimestamp(CheckInObject.EndTimeStamp).ToLocalTime().ToString("HH:mm");

			//CheckInTagsListView.ItemsSource = CheckInObject.Tags;

			GetCheckInChat();
		}

		private async void GetCheckInChat()
		{
			ChatObject = await Chat.CreateChat(CheckInObject.ProfileId);
			if (ChatObject != null)
			{
				foreach (var message in ChatObject.Messages)
				{
					ChatMessageList.Add(message);
				}

				GoToLastMessage();

				//_timer = new DispatcherTimer()
				//{
				//	Interval = TimeSpan.FromSeconds(3)
				//};
				//_timer.Tick += UpdateChatMessages;
				//_timer.Start();
			}
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

		private void MeetRequestButton_Click(object sender, RoutedEventArgs e)
		{

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

		private async void PostMessageButton_Click(object sender, RoutedEventArgs e)
		{
			await PostNewMessage();
		}

		private async void ChatMessageTextBox_KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
		{
			if (e.Key == Windows.System.VirtualKey.Enter)
			{
				await PostNewMessage();
				e.Handled = true;
			}
		}

		private void UpdateChatMessages(object sender, object o)
		{
			//var newMessages = ChatMessage
		}

		private async Task PostNewMessage()
		{
			var chatMessage = ChatMessageTextBox.Text;
			ChatMessageTextBox.Text = "";

			if (!string.IsNullOrEmpty(chatMessage))
			{
				var chat = await ChatMessage.PostChatMessage(ChatObject.Id, chatMessage);
				if (chat != null)
				{
					foreach (var message in chat.Messages)
					{
						if (!ChatMessageAlreadyExists(message))
						{
							ChatMessageList.Add(message);

							GoToLastMessage();
						}
					}
				}
			}
		}

		private bool ChatMessageAlreadyExists(ChatMessage message)
		{
			if (ChatMessageList.Where(cm => cm.Id == message.Id).Any())
			{
				return true;
			}
			return false;
		}

		private void GoToLastMessage()
		{
			var selectedIndex = ChatMessagesListView.Items.Count - 1;
			ChatMessagesListView.SelectedIndex = selectedIndex;
			ChatMessagesListView.UpdateLayout();

			ChatMessagesListView.ScrollIntoView(ChatMessagesListView.SelectedItem);
			ChatMessagesListView.UpdateLayout();
		}
	}
}
