using S2M.Models;
using S2M.ViewModel;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace S2M.Pages
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class ContactDetail : Page
	{
		public ContactDetailViewModel ViewModel { get; set; }

		public ContactDetail()

		{
			this.InitializeComponent();

			ViewModel = new ContactDetailViewModel();
			DataContext = ViewModel;
		}

		protected async override void OnNavigatedTo(NavigationEventArgs e)
		{
			var contactObject = (Models.Contact)e.Parameter;
			if (contactObject != null)
			{
				ViewModel.Tags = new ObservableCollection<string>();
				ViewModel.SelectedContact = contactObject;
				await ViewModel.GetPublicProfile();

				if (!string.IsNullOrWhiteSpace(contactObject.Tags))
				{
					var tags = contactObject.Tags.Split(',').ToList();
					foreach (var tag in tags)
					{
						if (!string.IsNullOrEmpty(tag))
						{
							ViewModel.Tags.Add(tag);
						}
					}
				}

				ViewModel.TagCount = ViewModel.Tags.Count();
			}
		}

		private async void StartChatButton_Click(object sender, RoutedEventArgs e)
		{
			ViewModel.ChatObject = await Chat.CreateChat(ViewModel.SelectedContact.ProfileId);
			if (ViewModel.ChatObject != null)
			{
				ChatUserControl.ChatObject = ViewModel.ChatObject;
				ContactDetailSplitView.IsPaneOpen = !ContactDetailSplitView.IsPaneOpen;
			}
		}
	}

}
