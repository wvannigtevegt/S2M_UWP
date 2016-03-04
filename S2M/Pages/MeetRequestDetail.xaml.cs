using S2M.Models;
using System.Collections.ObjectModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace S2M.Pages
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MeetRequestDetail : Page
	{
		public ObservableCollection<ChatMessage> ChatMessageList { get; set; }
		public MeetRequest MeetRequestObject { get; set; }

		public MeetRequestDetail()
		{
			this.InitializeComponent();

			MeetRequestObject = null;
			ChatMessageList = new ObservableCollection<ChatMessage>();
		}

		protected async override void OnNavigatedTo(NavigationEventArgs e)
		{
			var criteria = (MeetRequestDetailPageCriteria)e.Parameter;
			if (criteria != null)
			{
				if (criteria.MeetRequest != null)
				{
					MeetRequestObject = criteria.MeetRequest;
				}
				if (criteria.MeetRequestId > 0 && MeetRequestObject == null)
				{
					//_cts = new CancellationTokenSource();
					//CancellationToken token = _cts.Token;

					//try
					//{
					//MeetRequestObject = await MeetRequest.GetMeetRequestByIdAsync(criteria.MeetRequestId);
					//}
					//catch (Exception) { }
					//finally
					//{
					//	_cts = null;
					//}
				}
			}
			if (MeetRequestObject != null)
			{
				if (MeetRequestObject.MeetRequestAccepted > 0)
				{
					ChatMessagesListView.Visibility = Visibility.Visible;

					foreach (var message in MeetRequestObject.Messages)
					{
						ChatMessageList.Add(message);
					}

					GoToLastMessage();
				}
				
			}
		}

		private async void Page_Loaded(object sender, RoutedEventArgs e)
		{
			
		}

		private void GoToLastMessage()
		{
			var selectedIndex = ChatMessagesListView.Items.Count - 1;
			ChatMessagesListView.SelectedIndex = selectedIndex;
			ChatMessagesListView.UpdateLayout();

			ChatMessagesListView.ScrollIntoView(ChatMessagesListView.SelectedItem);
			ChatMessagesListView.UpdateLayout();
		}

		private async void AcceptRequestButton_Click(object sender, RoutedEventArgs e)
		{
			await MeetRequest.AcceptMeetRequest(MeetRequestObject.Id);
		}

		private async void DeclineRequestButton_Click(object sender, RoutedEventArgs e)
		{
			await MeetRequest.DeclineMeetRequest(MeetRequestObject.Id);

			Frame.Navigate(typeof(Archive));
		}
	}

	public class MeetRequestDetailPageCriteria
	{
		public int MeetRequestId { get; set; } = 0;
		public MeetRequest MeetRequest { get; set; } = null;
	}
}
