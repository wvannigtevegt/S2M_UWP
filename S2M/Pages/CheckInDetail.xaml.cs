using S2M.Models;
using S2M.ViewModel;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace S2M.Pages
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class CheckInDetail : Page {
		public CheckInDetailViewModel ViewModel { get; set; }

		public CheckInDetail() {
			this.InitializeComponent();

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
			var authenticatedProfile = await Common.StorageService.RetrieveObjectAsync<Models.Profile>("Profile");
			if (ViewModel.SelectedCheckin.ProfileId == authenticatedProfile.Id)
			{
				CheckInContactStackPanel.Visibility = Visibility.Collapsed;
			}
		}

		private async void StartChatButton_Click(object sender, RoutedEventArgs e) {
			ViewModel.ChatObject = await Chat.CreateChat(ViewModel.SelectedCheckin.ProfileId);
			if (ViewModel.ChatObject != null)
			{
				ChatUserControl.ChatObject = ViewModel.ChatObject;
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

		
	}
}
