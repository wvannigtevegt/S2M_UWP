using S2M.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace S2M.Pages
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class Archive : Page
	{
		public ObservableCollection<Chat> ChatList { get; set; }
		public ObservableCollection<CheckIn> CheckInList { get; set; }
		public ObservableCollection<MeetRequest> MeetRequestList { get; set; }

		private CancellationTokenSource _cts = null;

		public Archive()
		{
			this.InitializeComponent();

			ChatList = new ObservableCollection<Chat>();
			CheckInList = new ObservableCollection<CheckIn>();
			MeetRequestList = new ObservableCollection<MeetRequest>();
		}

		protected async override void OnNavigatedTo(NavigationEventArgs e)
		{
			var localSettings = ApplicationData.Current.LocalSettings;
			if (localSettings.Values["ArchivePivotSelectedIndex"] != null)
			{
				ArchivePivot.SelectedIndex = (int)localSettings.Values["ArchivePivotSelectedIndex"];
			}
		}

		protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
		{
			if (_cts != null)
			{
				_cts.Cancel();
				_cts = null;
			}

			var localSettings = ApplicationData.Current.LocalSettings;
			localSettings.Values["ArchivePivotSelectedIndex"] = ArchivePivot.SelectedIndex;

			base.OnNavigatingFrom(e);
		}

		private async void Page_Loaded(object sender, RoutedEventArgs e)
		{

		}

		private async void ArchivePivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			await LoadArchiveData();
		}

		private async Task LoadArchiveData()
		{
			_cts = new CancellationTokenSource();
			CancellationToken token = _cts.Token;

			ChatsProgressRing.IsActive = true;
			ChatsProgressRing.Visibility = Visibility.Visible;

			try
			{
				switch (ArchivePivot.SelectedIndex)
				{
					case 0:
						await GetArchiveChats(token);
						break;
					case 1:
						await GetArchiveMeetRequests(token);
						break;
					case 2:
						await GetArchiveCheckIns(token);
						break;
					case 3:
						break;
				}


			}
			catch (Exception) { }
			finally
			{
				_cts = null;

				ChatsProgressRing.IsActive = false;
				ChatsProgressRing.Visibility = Visibility.Collapsed;
			}
		}

		private async Task GetArchiveChats(CancellationToken token)
		{
			ChatList.Clear();
			await Chat.GetProfileChatsAsync(ChatList);
		}
		private async Task GetArchiveMeetRequests(CancellationToken token)
		{
			MeetRequestList.Clear();
			await MeetRequest.GetProfileMeetRequestsAsync(MeetRequestList);
		}
		private async Task GetArchiveCheckIns(CancellationToken token)
		{
			CheckInList.Clear();
			await CheckIn.GetProfileCheckInsAsync(token, CheckInList);
		}

		private void ChatsListView_ItemClick(object sender, ItemClickEventArgs e)
		{
			var chat = (Chat)e.ClickedItem;
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

		private void ReservationsListView_ItemClick(object sender, ItemClickEventArgs e)
		{
			var checkIn = (CheckIn)e.ClickedItem;

			Frame.Navigate(typeof(CheckInFinal), checkIn);
		}

		private async void CancelCheckInButton_Click(object sender, RoutedEventArgs e)
		{
			Button _button = (Button)sender;
			var id = int.Parse(_button.Tag.ToString());

			_cts = new CancellationTokenSource();
			CancellationToken token = _cts.Token;

			try
			{
				var result = await CheckIn.CancelCheckIn(token, id);
				if (result == 1)
				{
					var itemToRemove = CheckInList.Single(cl => cl.Id == id);
					CheckInList.Remove(itemToRemove);

					var currentCheckIn = await Common.StorageService.RetrieveObjectAsync<Models.CurrentCheckIn>("CurrentCheckIn");
					if (currentCheckIn != null)
					{
						if (currentCheckIn.CheckIn.Id == id)
						{
							await Common.StorageService.DeleteObjectAsync("CurrentCheckIn");
						}
					}
				}
			}
			catch (Exception) { }
			finally
			{
				_cts = null;
			}
		}

		private void MeetRequestsListView_ItemClick(object sender, ItemClickEventArgs e)
		{
			var meetRequest = (MeetRequest)e.ClickedItem;
			if (meetRequest != null)
			{
				var criteria = new MeetRequestDetailPageCriteria
				{
					MeetRequest = meetRequest,
					MeetRequestId = 0
				};

				Frame.Navigate(typeof(MeetRequestDetail), criteria);
			}
		}
	}
}
