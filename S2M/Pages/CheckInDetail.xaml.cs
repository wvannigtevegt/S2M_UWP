using S2M.Common;
using S2M.Models;
using S2M.ViewModel;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace S2M.Pages {
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class CheckInDetail : Page {
		public ObservableCollection<ChatMessage> ChatMessageList { get; set; }
		public Chat ChatObject { get; set; }

		private DispatcherTimer _timer;

		public CheckInDetailViewModel ViewModel { get; set; }

		public CheckInDetail() {
			this.InitializeComponent();
			
			ChatMessageList = new ObservableCollection<ChatMessage>();

			ViewModel = new CheckInDetailViewModel();
			DataContext = ViewModel;
		}

		protected async override void OnNavigatedTo(NavigationEventArgs e) {
			var checkInObject = (Models.CheckIn)e.Parameter;
			if (checkInObject != null) {
				ViewModel.SelectedCheckin = checkInObject;
				await ViewModel.GetPublicProfile();

				var tags = checkInObject.Tags.Split(',').ToList();
				foreach (var tag in tags)
				{
					if (!string.IsNullOrEmpty(tag))
					{
						ViewModel.Tags.Add(tag);
					}
				}
					
				ViewModel.TagCount = ViewModel.Tags.Count();
			}
		}

		private async void Page_Loaded(object sender, RoutedEventArgs e) {			
			//CheckInImageBrush.UriSource = new Uri(ViewModel.SelectedCheckin.ProfileImage_150);
			
			var authenticatedProfile = await Common.StorageService.RetrieveObjectAsync<Models.Profile>("Profile");
			if (ViewModel.SelectedCheckin.ProfileId == authenticatedProfile.Id)
			{
				CheckInContactStackPanel.Visibility = Visibility.Collapsed;
			}

			GetCheckInChat();
		}

		private async void GetCheckInChat()
		{
			ChatObject = await Chat.CreateChat(ViewModel.SelectedCheckin.ProfileId);
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
			if (ChatObject == null)
			{
				ChatObject = await Chat.CreateChat(ViewModel.SelectedCheckin.ProfileId);
			}
			if (ChatObject != null)
			{
				CheckInDetailSplitView.IsPaneOpen = !CheckInDetailSplitView.IsPaneOpen;
			}
		}

		private void MeetRequestButton_Click(object sender, RoutedEventArgs e)
		{

		}

		private void CheckInLocationHyperlinkButton_Click(object sender, RoutedEventArgs e)
		{
			var criteria = new LocationDetailPageCriteria
			{
				LocationId = ViewModel.SelectedCheckin.LocationId,
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
