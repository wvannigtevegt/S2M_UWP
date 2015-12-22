using System;
using System.Collections.Generic;
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
		protected List<string> TagList { get; set; }

		public Profile() {
			this.InitializeComponent();

			ProfileObject = new Models.Profile();
			TagList = new List<string>();
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

				TagList = ProfileObject.Tags.Split(',').ToList();
				LstTags.ItemsSource = TagList;
			}
		}

		private void AddTagToProfile(string tag) {
			if (!string.IsNullOrEmpty(tag.Trim())) {
				TagList.Add(tag);

				LstTags.ItemsSource = TagList;
            }
		}

		private async void LstTags_ItemClick(object sender, ItemClickEventArgs e) {
			var tag = ((string)e.ClickedItem); ;

			List<string> newTagList = TagList.Where(str => str != tag).ToList();

			TagList = newTagList;
			LstTags.ItemsSource = TagList;
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
	}
}
