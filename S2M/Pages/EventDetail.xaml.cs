using S2M.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Notifications;
using NotificationsExtensions.Toasts; // NotificationsExtensions.Win10
using Microsoft.QueryStringDotNET; // QueryString.NET
using System.Threading.Tasks;



// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace S2M.Pages
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class EventDetail : Page
	{
		public Event EventObject { get; set; }
		public ObservableCollection<CheckIn> CheckInList { get; set; }
		public ObservableCollection<CheckInKnowledgeTag> TagCheckInList { get; set; }

		private CancellationTokenSource _cts = null;

		public EventDetail()
		{
			this.InitializeComponent();

			CheckInList = new ObservableCollection<CheckIn>();
			TagCheckInList = new ObservableCollection<CheckInKnowledgeTag>();
		}

		protected async override void OnNavigatedTo(NavigationEventArgs e)
		{
			var eventObject = (Models.Event)e.Parameter;
			if (eventObject != null)
			{
				EventObject = eventObject;

				EventNameTextBlock.Text = EventObject.Name;
				EventLocationTextBlock.Text = EventObject.LocationName;
				EventDataTextBlock.Text = EventObject.Date.ToString();
			}
		}

		protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
		{
			if (_cts != null)
			{
				_cts.Cancel();
				_cts = null;
			}

			base.OnNavigatingFrom(e);
		}

		private async void Page_Loaded(object sender, RoutedEventArgs e)
		{
			_cts = new CancellationTokenSource();
			CancellationToken token = _cts.Token;

			try
			{
				await CheckIn.GetCheckInsAsync(token, CheckInList, 0, EventObject.Id, "", 0, 0, 0, "", 0, 0, false);
				await CheckInKnowledgeTag.GetEventCheckInKnowledgeTagsAsync(token, TagCheckInList, EventObject.Id);


			}
			catch (Exception) { }
			finally
			{
				_cts = null;
			}
		}

		private void CheckIfProfileAlreadyCheckedInEvent()
		{
			var profileCheckIn =
				from pc in CheckInList
				where pc.ProfileId == 0
				select pc;

			if (profileCheckIn.Any())
			{
				EventCheckInButton.Visibility = Visibility.Collapsed;
			}
		}

		private void EventCheckInsGridView_ItemClick(object sender, ItemClickEventArgs e)
		{
			var checkIn = (CheckIn)e.ClickedItem;

			Frame.Navigate(typeof(CheckInDetail), checkIn);
		}

		private void TagCheckInsListView_ItemClick(object sender, ItemClickEventArgs e)
		{
			var tagCheckIn = (CheckInKnowledgeTag)e.ClickedItem;

			CheckInList.Clear();
			foreach (var checkIn in tagCheckIn.CheckIns)
			{
				CheckInList.Add(checkIn);
			}
		}

		private async void EventCheckInButton_Click(object sender, RoutedEventArgs e)
		{
			_cts = new CancellationTokenSource();
			CancellationToken token = _cts.Token;

			try
			{
				var checkIn = await CheckIn.CheckInToEvent(token, EventObject.Id);
				if (checkIn != null)
				{
					ShowToast(checkIn.Id, checkIn.EventName);
				}

			}
			catch (Exception) { }
			finally
			{
				_cts = null;
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
					Text = title
				},

				BodyTextLine1 = new ToastText()
				{
					Text = content
				},

				InlineImages =
				{
					new ToastImage()
					{
						Source = new ToastImageSource(image)
					}
				},

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
