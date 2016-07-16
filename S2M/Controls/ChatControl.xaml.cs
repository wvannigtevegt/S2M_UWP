using S2M.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace S2M.Controls
{
	public sealed partial class ChatControl : UserControl
	{
		public static readonly DependencyProperty ChatObjectProperty = DependencyProperty.Register("ChatObject", typeof(Chat), typeof(ChatControl), new PropertyMetadata(null));

		public ObservableCollection<ChatMessage> ChatMessageList = new ObservableCollection<ChatMessage>();
		public Chat ChatObject
		{
			get { return (Chat)GetValue(ChatObjectProperty); }
			set { SetValue(ChatObjectProperty, value); }
		}

		public ChatControl()
		{
			this.InitializeComponent();
			(this.Content as FrameworkElement).DataContext = this;
		}

		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
			if (ChatObject != null && ChatObject.Messages.Any())
			{
				foreach(var message in ChatObject.Messages)
				{
					ChatMessageList.Add(message);
				}

				GoToLastMessage();
			}
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
						}
					}
					GoToLastMessage();
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

			ChatMessagesListView.ScrollIntoView(ChatMessagesListView.SelectedItem);
			ChatMessagesListView.UpdateLayout();
		}
	}

	public class ChatTemplateSelector : Common.TemplateSelector
	{

		public DataTemplate ImageLeft
		{
			get;
			set;
		}

		public DataTemplate ImageRight
		{
			get;
			set;
		}

		public override DataTemplate SelectTemplate(object item, DependencyObject container)
		{
			var chatMessage = item as ChatMessage;
			if (chatMessage != null)
			{
				return chatMessage.ProfileId != 96 ? ImageLeft : ImageRight;
			}
			return null;
		}
	}
}
