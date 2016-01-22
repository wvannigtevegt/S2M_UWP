using S2M.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace S2M.Pages
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class ChatDetail : Page
	{
		public ObservableCollection<ChatMessage> ChatMessageList { get; set; }

		public Chat ChatObject { get; set; }

		public ChatDetail()
		{
			this.InitializeComponent();

			ChatObject = null;
			ChatMessageList = new ObservableCollection<ChatMessage>();
		}

		protected async override void OnNavigatedTo(NavigationEventArgs e)
		{
			var criteria = (ChatDetailPageCriteria)e.Parameter;
			if (criteria != null)
			{
				if (criteria.Chat != null)
				{
					ChatObject = criteria.Chat;
				}
				if (criteria.ChatId > 0 && ChatObject == null)
				{
					//_cts = new CancellationTokenSource();
					//CancellationToken token = _cts.Token;

					//try
					//{
					ChatObject = await Chat.GetChatByIdAsync(criteria.ChatId);
					//}
					//catch (Exception) { }
					//finally
					//{
					//	_cts = null;
					//}
				}
			}
			if (ChatObject != null)
			{
				foreach (var message in ChatObject.Messages)
				{
					ChatMessageList.Add(message);
				}
			}
			var test = "";
		}

		private async void Page_Loaded(object sender, RoutedEventArgs e)
		{
			GoToLastMessage();
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

		private async Task PostNewMessage()
		{
			var chatMessage = ChatMessageTextBox.Text;
			if (!string.IsNullOrEmpty(chatMessage))
			{
				var chat = await ChatMessage.PostChatMessage(ChatObject.Id, chatMessage);
				if (chat != null)
				{
					ChatMessageTextBox.Text = "";

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

	public class ChatDetailPageCriteria
	{
		public int ChatId { get; set; } = 0;
		public Chat Chat { get; set; } = null;
	}
}
