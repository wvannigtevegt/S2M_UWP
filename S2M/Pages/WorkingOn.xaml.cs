using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace S2M.Pages {
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class WorkingOn : Page {
		protected string WorkingOnString { get; set; }

		public WorkingOn() {
			this.InitializeComponent();

			WorkingOnString = "";
		}

		private void Page_Loaded(object sender, RoutedEventArgs e) {

		}

		private async  void SaveWorkingOnButton_Click(object sender, RoutedEventArgs e) {
			WorkingOnString = WorkingOnTextBox.Text;
			if (!string.IsNullOrEmpty(WorkingOnString)) {
				var workingOnObj = new Models.WorkingOn() {
					Text = WorkingOnString,
					EnteredOn = DateTime.Now
				};

				await Common.StorageService.PersistObjectAsync("WorkingOn", workingOnObj);

				Frame.Navigate(typeof(Home));
			}
		}
	}
}
