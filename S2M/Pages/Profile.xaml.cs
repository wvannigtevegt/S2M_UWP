using S2M.Models;
using S2M.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace S2M.Pages {
	public sealed partial class Profile : Page {
		public ProfileViewModel ViewModel { get; set; }

		public Profile() {
			this.InitializeComponent();

			ViewModel = new ProfileViewModel();
			DataContext = ViewModel;
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			
		}

		private async void Page_Loaded(object sender, RoutedEventArgs e) {
			this.NavigationCacheMode = NavigationCacheMode.Required;

			Models.Profile _profile = await Common.StorageService.RetrieveObjectAsync<Models.Profile>("Profile"); 
			if (_profile == null) {
				_profile = await Models.Profile.GetProfile();
			}

			if (_profile != null) {
				ViewModel.Profile = _profile;

				ProfileImageBrush.UriSource = new Uri(ViewModel.Profile.ProfileImage_150);

				var tags = ViewModel.Profile.Tags.Split(',').ToList();
				foreach (var tag in tags) {
					ViewModel.Tags.Add(tag);
				}
			}
		}

		private async void AddTagButton_Click(object sender, RoutedEventArgs e) {
			var newTag = AddTagTextBox.Text.ToLower().Trim();

			if (!string.IsNullOrEmpty(newTag)) {
				if (!ViewModel.Tags.Contains(newTag)) {
					ViewModel.Tags.Add(newTag);
				}

				AddTagTextBox.Text = "";

				await SaveProfile();
			}
		}

		private async void DeleteTagButton_Click(object sender, RoutedEventArgs e)
		{
			var deleteTag = ((string)(sender as FrameworkElement).Tag);

			List<string> newTagList = ViewModel.Tags.Where(str => str != deleteTag).ToList();
			ViewModel.Tags.Clear();
			foreach (var tag in newTagList)
			{
				ViewModel.Tags.Add(tag);
			}

			await SaveProfile();
		}

		private async Task SaveProfile()
		{
			ViewModel.Profile.Tags = string.Join(",", ViewModel.Tags);

			await Models.Profile.UpdateProfileAsync(ViewModel.Profile);
		}

		private async void CancelCheckInButton_Click(object sender, RoutedEventArgs e)
		{
			Button _button = (Button)sender;
			var id = int.Parse(_button.Tag.ToString());

			var _cts = new CancellationTokenSource();
			CancellationToken token = _cts.Token;

			try
			{
				var result = await CheckIn.CancelCheckIn(token, id);
				if (result == 1)
				{
					var itemToRemove = ViewModel.Checkins.Single(cl => cl.Id == id);
					ViewModel.Checkins.Remove(itemToRemove);
				}
			}
			catch (Exception) { }
			finally
			{
				_cts = null;
			}
		}

		private async void ProfilePivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			switch (ProfilePivot.SelectedIndex)
			{
				case 0:
					
					break;
				case 1:
					await GetProfileCheckins();
					break;
				case 2:
					await GetProfileChats();
					break;
				case 3:
					await GetProfileContacts();
					break;
			}
		}

		private async Task GetProfileCheckins()
		{
			await ViewModel.GetProfileCheckins();
		}

		private async Task GetProfileChats()
		{
			await ViewModel.GetProfileChats();
		}

		private async Task GetProfileContacts()
		{

		}

		private void CheckinsListView_ItemClick(object sender, ItemClickEventArgs e)
		{
			var checkIn = (CheckIn)e.ClickedItem;
			if (checkIn != null)
			{
				var checkinCriteria = new CheckinFinalPageCriteria
				{
					IsNewCheckIn = false,
					CheckIn = checkIn
				};

				Frame.Navigate(typeof(CheckInFinal), checkinCriteria);
			}
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
	}
}
