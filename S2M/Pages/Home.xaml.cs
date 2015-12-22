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

namespace S2M.Pages {
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class Home : Page {
		public ObservableCollection<CheckIn> CheckInRecommendations { get; set; }
		public string WorkingOn { get; set; }

		protected string SearchTerm { get; set; }

		private CancellationTokenSource _cts = null;

		public Home() {
			this.InitializeComponent();

			CheckInRecommendations = new ObservableCollection<CheckIn>();
			SearchTerm = "";
			WorkingOn = "Javascript";
		}

		protected override void OnNavigatingFrom(NavigatingCancelEventArgs e) {
			if (_cts != null) {
				_cts.Cancel();
				_cts = null;
			}

			base.OnNavigatingFrom(e);
		}

		private async void SetWorkingOnButton_Click(object sender, RoutedEventArgs e) {
			WorkingOn = WorkingOnTextBox.Text;

			_cts = new CancellationTokenSource();
			CancellationToken token = _cts.Token;

			try {
				CheckInRecommendations.Clear();
				await CheckIn.GetCheckInsAsync(token, CheckInRecommendations, 0, 0, SearchTerm, 0, 0, 0, WorkingOn, 1, 5, false);
			}
			catch (Exception) { }
			finally {
				_cts = null;
			}
		}

		private void Page_Loaded(object sender, RoutedEventArgs e) {

		}

		private void CheckInsGridView_ItemClick(object sender, ItemClickEventArgs e) {

		}
	}
}
