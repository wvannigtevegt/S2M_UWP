using NotificationsExtensions.Toasts; // using NotificationsExtensions.Win10;
using S2M.Models;
using S2M.ViewModel;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;



// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace S2M.Pages
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class EventDetail : Page
	{
		public CheckIn CheckInObject { get; set; }

		public EventDetailViewModel ViewModel { get; set; }

		public EventDetail()
		{
			this.InitializeComponent();

			ViewModel = new EventDetailViewModel();
			DataContext = ViewModel;
		}

		protected async override void OnNavigatedTo(NavigationEventArgs e)
		{
			var eventObject = (EventCalendar)e.Parameter;
			if (eventObject != null)
			{
				ViewModel.Event = eventObject;
			}
		}

		protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
		{
			base.OnNavigatingFrom(e);
		}

		private async void Page_Loaded(object sender, RoutedEventArgs e)
		{
			Models.Profile _profile = await Common.StorageService.RetrieveObjectAsync<Models.Profile>("Profile");
			if (_profile == null)
			{
				_profile = await Models.Profile.GetProfile();
			}

			if (_profile != null)
			{
				ViewModel.Profile = _profile;
			}

			GetEventCheckInData();
		}

		private async void GetEventCheckInData()
		{
			await ViewModel.GetEventCheckIns();

			CheckIfProfileAlreadyCheckedInEvent();
		}

		private void CheckIfProfileAlreadyCheckedInEvent()
		{
			var profileCheckIn =
				from pc in ViewModel.Checkins
				where pc.ProfileId == ViewModel.Profile.Id
				select pc;

			if (profileCheckIn.Any())
			{
				CheckInObject = profileCheckIn.First();
				if (CheckInObject != null)
				{
					ViewModel.AlreadyCheckedin = true;
					ViewModel.ShowCheckinButton = false;
				}
				else
				{
					ViewModel.ShowCheckinButton = true;
					ViewModel.EnableButton = true;
				}
			}
			else
			{
				ViewModel.ShowCheckinButton = true;
				ViewModel.EnableButton = true;
			}
		}

		private async void EventCheckInsGridView_ItemClick(object sender, ItemClickEventArgs e)
		{
			var checkIn = (CheckIn)e.ClickedItem;

			if (checkIn.ProfileId == ViewModel.Profile.Id)
			{
				var checkinCriteria = new CheckinFinalPageCriteria
				{
					IsNewCheckIn = false,
					CheckIn = checkIn
				};

				Frame.Navigate(typeof(CheckInFinal), checkinCriteria);
			}
			else
			{
				Frame.Navigate(typeof(CheckInDetail), checkIn);
			}
		}

		private void TagCheckInsListView_ItemClick(object sender, ItemClickEventArgs e)
		{
			var tagCheckIn = (CheckInKnowledgeTag)e.ClickedItem;

			ViewModel.Checkins.Clear();
			foreach (var checkIn in tagCheckIn.CheckIns)
			{
				ViewModel.Checkins.Add(checkIn);
			}
		}

		private async void EventCheckInButton_Click(object sender, RoutedEventArgs e)
		{
			var _cts = new CancellationTokenSource();
			CancellationToken token = _cts.Token;

			ViewModel.ShowSpinner = true;
			ViewModel.EnableButton = false;

			try
			{
				var checkin = await CheckIn.CheckInToEvent(token, ViewModel.Event.Id, "");
				if (checkin != null)
				{
					var checkinCriteria = new CheckinFinalPageCriteria
					{
						IsNewCheckIn = true,
						CheckIn = checkin
					};

					Frame.Navigate(typeof(CheckInFinal), checkinCriteria);
				}

			}
			catch (Exception) { }
			finally
			{
				_cts = null;
				ViewModel.ShowSpinner = false;
			}
		}

		private async static void ShowToast(int checkInId, string eventName)
		{
			// In a real app, these would be initialized with actual data
			string title = "Check-in completed!";
			string content = "You checked-in to " + eventName;
			string image = "http://blogs.msdn.com/cfs-filesystemfile.ashx/__key/communityserver-blogs-components-weblogfiles/00-00-01-71-81-permanent/2727.happycanyon1_5B00_1_5D00_.jpg";
			string logo = "ms-appdata:///local/StoreLogo.scale-100.png";
			//int conversationId = 384928;

			// Construct the visuals of the toast
			ToastVisual visual = new ToastVisual()
			{
				TitleText = new ToastText()
				{
					Text = title,
				},
				BodyTextLine1 = new ToastText()
				{
					Text = content
				},

				//InlineImages =
				//{
				//	new ToastImage()
				//	{
				//		Source = new ToastImageSource(image)
				//	}
				//},

				AppLogoOverride = new ToastAppLogo()
				{
					Source = new ToastImageSource(logo),
					Crop = ToastImageCrop.Circle
				}
			};

			// Construct the actions for the toast (inputs and buttons)
			//ToastActionsCustom actions = new ToastActionsCustom()
			//{
			//	Inputs =
			//	{
			//		new ToastTextBox("tbReply")
			//		{
			//			PlaceholderContent = "Type a response"
			//		}
			//	},

			//	Buttons =
			//	{
			//		new ToastButton("Reply", new QueryString()
			//		{
			//			{ "action", "reply" },
			//			{ "conversationId", conversationId.ToString() }

			//		}.ToString())
			//		{
			//			ActivationType = ToastActivationType.Background,
			//			ImageUri = "Assets/Reply.png",

			//                     // Reference the text box's ID in order to
			//                     // place this button next to the text box
			//                     TextBoxId = "tbReply"
			//		},

			//		new ToastButton("Like", new QueryString()
			//		{
			//			{ "action", "like" },
			//			{ "conversationId", conversationId.ToString() }

			//		}.ToString())
			//		{
			//			ActivationType = ToastActivationType.Background
			//		},

			//		new ToastButton("View", new QueryString()
			//		{
			//			{ "action", "viewImage" },
			//			{ "imageUrl", image }

			//		}.ToString())
			//	}
			//};


			// Now we can construct the final toast content
			ToastContent toastContent = new ToastContent()
			{
				Visual = visual//,
							   //Actions = actions,

				// Arguments when the user taps body of toast
				//Launch = new QueryString()
				//{
				//	{ "action", "viewConversation" },
				//	{ "conversationId", conversationId.ToString() }

				//}.ToString()
			};


			// And create the toast notification
			ToastNotification notification = new ToastNotification(toastContent.GetXml());


			// And then send the toast
			ToastNotificationManager.CreateToastNotifier().Show(notification);
		}
	}
}
