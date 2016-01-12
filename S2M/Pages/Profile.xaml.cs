using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace S2M.Pages {
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class Profile : Page {
		protected Models.Profile ProfileObject { get; set; }
		protected ObservableCollection<string> TagList { get; set; }

		public Profile() {
			this.InitializeComponent();

			ProfileObject = new Models.Profile();
			TagList = new ObservableCollection<string>();
        }

		private async void Page_Loaded(object sender, RoutedEventArgs e) {
			Models.Profile _profile = await Common.StorageService.RetrieveObjectAsync<Models.Profile>("Profile"); 
			if (_profile == null) {
				_profile = await Models.Profile.GetProfile();
			}

			if (_profile != null) {
				ProfileObject = _profile;

				ProfileImageBrush.UriSource = new Uri(ProfileObject.ProfileImage_150);
				textBoxProfileFirstName.Text = ProfileObject.FirstName;
				textBoxProfileLastName.Text = ProfileObject.LastName;
				textBoxProfileAddress.Text = ProfileObject.Address;
				textBoxProfilePostalcode.Text = ProfileObject.Postalcode;
				textBoxProfileCity.Text = ProfileObject.City;

				var tags = ProfileObject.Tags.Split(',').ToList();
				foreach (var tag in tags) {
					TagList.Add(tag);
				}
				TagsListView.ItemsSource = TagList;
			}
		}

		private async void LstTags_ItemClick(object sender, ItemClickEventArgs e) {
			var deleteTag = ((string)e.ClickedItem); ;

			List<string> newTagList = TagList.Where(str => str != deleteTag).ToList();
			TagList.Clear();
			foreach (var tag in newTagList) {
				TagList.Add(tag);
			}

			//TagsListView.ItemsSource = TagList;
		}

		private async void SaveAppBarButton_Click(object sender, RoutedEventArgs e) {
			ProfileObject.FirstName = textBoxProfileFirstName.Text;
			ProfileObject.LastName = textBoxProfileLastName.Text;
			ProfileObject.Address = textBoxProfileAddress.Text;
			ProfileObject.Postalcode = textBoxProfilePostalcode.Text;
			ProfileObject.City = textBoxProfileCity.Text;
			ProfileObject.Tags = string.Join(",", TagList);

			await Models.Profile.UpdateProfileAsync(ProfileObject);
		}

		private void AddTagButton_Click(object sender, RoutedEventArgs e) {
			var newTag = AddTagTextBox.Text.ToLower().Trim();

			if (!string.IsNullOrEmpty(newTag)) {
				if (!TagList.Contains(newTag)) {
					TagList.Add(newTag);
				}

				AddTagTextBox.Text = "";
			}
		}
	}
}
