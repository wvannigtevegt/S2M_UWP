using S2M.Models;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace S2M.Controls
{
	public sealed partial class MeetRequestTemplate : UserControl
	{
		public MeetRequest MeetRequestObject { get { return this.DataContext as MeetRequest; } }

		public MeetRequestTemplate()
		{
			this.InitializeComponent();

			this.DataContextChanged += (s, e) => Bindings.Update();
		}

		private async void AcceptRequestButton_Click(object sender, RoutedEventArgs e)
		{
			await MeetRequest.AcceptMeetRequest(MeetRequestObject.Id);

			var criteria = new Pages.MeetRequestDetailPageCriteria
			{
				MeetRequest = MeetRequestObject,
				MeetRequestId = 0
			};

			//Navigation.Frame.Navigate(typeof(Pages.MeetRequestDetail), criteria);
		}

		private async void DeclineRequestButton_Click(object sender, RoutedEventArgs e)
		{
			await MeetRequest.DeclineMeetRequest(MeetRequestObject.Id);
		}
	}
}
