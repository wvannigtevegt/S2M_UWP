using S2M.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace S2M.Pages
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class ChatDetail : Page
	{
		public Chat ChatObject { get; set; }

		private DispatcherTimer _timer;

		public ChatDetail()
		{
			this.InitializeComponent();

			ChatObject = null;
		}

		protected async override void OnNavigatedTo(NavigationEventArgs e)
		{
			var criteria = (ChatDetailPageCriteria)e.Parameter;
			if (criteria != null)
			{
				if (criteria.Chat != null)
				{
					ChatObject = criteria.Chat;
				}
				if (criteria.ChatId > 0 && ChatObject == null)
				{
					ChatObject = await Chat.GetChatByIdAsync(criteria.ChatId);
				}
			}
			if (ChatObject != null)
			{
				ChatUserControl.ChatObject = ChatObject;
			}
		}

		private async void Page_Loaded(object sender, RoutedEventArgs e)
		{

		}

		private void CheckInsFontIcon_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
		{

		}
	}

	public class ChatDetailPageCriteria
	{
		public int ChatId { get; set; } = 0;
		public Chat Chat { get; set; } = null;
	}
}
