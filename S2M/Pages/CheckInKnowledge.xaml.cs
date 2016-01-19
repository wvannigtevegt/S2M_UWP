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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace S2M.Pages
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class CheckInKnowledge : Page
	{
		public ObservableCollection<CheckInKnowledgeTag> TagCheckInList { get; set; }

		protected string SearchTerm { get; set; }

		private CancellationTokenSource _cts = null;

		public CheckInKnowledge()
		{
			this.InitializeComponent();

			TagCheckInList = new ObservableCollection<CheckInKnowledgeTag>();
			SearchTerm = "";
		}

		protected async override void OnNavigatedTo(NavigationEventArgs e)
		{
			SearchTerm = (string)e.Parameter;
			if (SearchTerm == null)
			{
				SearchTerm = "";
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
				CheckInKnowledgeProgressRing.IsActive = true;
				CheckInKnowledgeProgressRing.Visibility = Visibility.Visible;

				await CheckInKnowledgeTag.GetLocationCheckInKnowledgeTagsAsync(token, TagCheckInList, 0, SearchTerm, 1, 25);

				CheckInKnowledgeProgressRing.IsActive = false;
				CheckInKnowledgeProgressRing.Visibility = Visibility.Collapsed;
			}
			catch (Exception) { }
			finally
			{
				_cts = null;
			}
		}

		private void CheckInsHyperLinkButton_Click(object sender, RoutedEventArgs e)
		{
			Frame.Navigate(typeof(Pages.CheckIns));
		}

		private void TagCheckInsListView_ItemClick(object sender, ItemClickEventArgs e)
		{
			var tagCheckIn = (CheckInKnowledgeTag)e.ClickedItem;

			var criteria = new CheckInPageCriteria
			{
				CheckInKnowledgeTag = tagCheckIn,
				SearchTerm = ""
			};

			Frame.Navigate(typeof(CheckIns), criteria);
		}
	}
}
