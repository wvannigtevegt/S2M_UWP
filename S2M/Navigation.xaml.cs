using S2M.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace S2M {
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class Navigation : Page {
		protected string CurrentPage { get; set; }
		protected Profile ProfileObject { get; set; }
		protected string SearchTerm { get; set; }

		private CancellationTokenSource _cts = null;

		public Navigation() {
			this.InitializeComponent();

			ProfileObject = new Profile();

			bool isHardwareButtonsAPIPresent = Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons");

			if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons")) {
				Windows.Phone.UI.Input.HardwareButtons.BackPressed += HardwareButtons_BackPressed;
			}

			//var currentView = SystemNavigationManager.GetForCurrentView();
			//if (isHardwareButtonsAPIPresent) {
			//	//Windows.Phone.UI.Input.HardwareButtons.CameraPressed += backButton_Tapped;
			//}
			//currentView.BackRequested += backButton_Tapped;
			NavigationFrame.Navigated += OnNavigated;

			SystemNavigationManager.GetForCurrentView().BackRequested += SystemNavigationManager_BackRequested;
			if (NavigationFrame.CanGoBack) {
				SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
			}
		}

		protected override void OnNavigatingFrom(NavigatingCancelEventArgs e) {
			if (_cts != null) {
				_cts.Cancel();
				_cts = null;
			}

			base.OnNavigatingFrom(e);
		}

		private async void Page_Loaded(object sender, RoutedEventArgs e) {
			ProfileObject = await Profile.GetProfile();

			ProfileImageBrush.ImageSource = new BitmapImage(new Uri(ProfileObject.ProfileImage_84));
			LocationNameTextBlock.Text = ProfileObject.FullName;

			await Common.StorageService.DeleteObjectAsync("WorkingOn"); //TODO: Remove
			var workingOn = await Common.StorageService.RetrieveObjectAsync<Models.WorkingOn>("WorkingOn");
			if (workingOn == null) {
				NavigationFrame.Navigate(typeof(Pages.WorkingOn));
			}
			else {
				
				if (string.IsNullOrEmpty(workingOn.Text) || workingOn.EnteredOn.Date != DateTime.Now.Date) {
					NavigationFrame.Navigate(typeof(Pages.WorkingOn));
				}
				else {
					GoToPage("Home");
				}
			}
		}

		private void OnNavigated(object sender, NavigationEventArgs e) {
			// Each time a navigation event occurs, update the Back button's visibility
			SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
				NavigationFrame.CanGoBack ?
				AppViewBackButtonVisibility.Visible :
				AppViewBackButtonVisibility.Collapsed;
		}

		private void HamburgerButton_Click(object sender, RoutedEventArgs e) {
			NavigationSplitView.IsPaneOpen = !NavigationSplitView.IsPaneOpen;
		}

		private void SystemNavigationManager_BackRequested(object sender, BackRequestedEventArgs e) {
			if (!e.Handled && NavigationFrame.CanGoBack) {
				var pageToGoTo = NavigationFrame.BackStack.Last().SourcePageType;
				NavigationFrame.GoBack();

				e.Handled = true;

				var page = pageToGoTo.Name;
				GoToPage(page);
			}
		}

		private void HardwareButtons_BackPressed(object sender, Windows.Phone.UI.Input.BackPressedEventArgs e) {
			if (!e.Handled && NavigationFrame.CanGoBack) {
				var pageToGoTo = NavigationFrame.BackStack.Last().SourcePageType;
				NavigationFrame.GoBack();

				e.Handled = true;

				var page = pageToGoTo.Name;
				GoToPage(page);
			}
		}

		private async void LogoffHyperLinkButton_Click(object sender, RoutedEventArgs e) {
			await Models.Login.LogOff();

			Frame.Navigate(typeof(MainPage));
		}

		private void RadioButtonPaneItem_Click(object sender, RoutedEventArgs e) {
			var radioButton = sender as RadioButton;
			if (radioButton != null) {
				var page = radioButton.Tag.ToString();
				GoToPage(page);
			}
		}

		private void GoToPage(string page) {
			NavigationSplitView.IsPaneOpen = false;
			CurrentPage = page;

			switch (page) {
				case "Home":
					HomeRadioButton.IsChecked = true;
					NavigationHeaderTextBlock.Text = "Recommended for you";
					NavigationFrame.Navigate(typeof(Pages.Home));

					//.Visibility = Visibility.Collapsed;
					break;
				case "Locations":
					LocationsRadioButton.IsChecked = true;
					NavigationHeaderTextBlock.Text = "Locations";
					NavigationFrame.Navigate(typeof(Pages.Locations), SearchTerm);

					//SearchButton.Visibility = Visibility.Visible;
					break;
				case "CheckIns":
					CheckInsRadioButton.IsChecked = true;
					NavigationHeaderTextBlock.Text = "CheckIns";
					NavigationFrame.Navigate(typeof(Pages.CheckIns), SearchTerm);

					//SearchButton.Visibility = Visibility.Visible;
					break;
				case "Events":
					EventsRadioButton.IsChecked = true;
					NavigationHeaderTextBlock.Text = "Events";
					NavigationFrame.Navigate(typeof(Pages.Events), SearchTerm);

					//SearchButton.Visibility = Visibility.Visible;
					break;
				case "Archive":
					ArchiveRadioButton.IsChecked = true;
					NavigationHeaderTextBlock.Text = "Archive";
					NavigationFrame.Navigate(typeof(Pages.Archive));

					//SearchButton.Visibility = Visibility.Collapsed;
					break;
				case "Settings":
					ArchiveRadioButton.IsChecked = true;
					NavigationHeaderTextBlock.Text = "Settings";
					NavigationFrame.Navigate(typeof(Pages.Settings));

					SharedAutoSuggestBox.Visibility = Visibility.Collapsed;
					SearchButton.Visibility = Visibility.Collapsed;
					break;
				case "Profile":
					ProfileRadioButton.IsChecked = true;
					NavigationHeaderTextBlock.Text = "Events";
					NavigationFrame.Navigate(typeof(Pages.Profile));

					//SearchButton.Visibility = Visibility.Collapsed;
					break;
			}

			ResetPageHeader();
		}

		private void SearchButton_Click(object sender, RoutedEventArgs e) {
			SharedAutoSuggestBox.Visibility = Visibility.Visible;
			SearchButton.Visibility = Visibility.Collapsed;
			NavigationHeaderTextBlock.Visibility = Visibility.Collapsed;
		}

		private void ResetPageHeader() {
			//SharedAutoSuggestBox.Text = "";

			SharedAutoSuggestBox.Visibility = Visibility.Visible;
			SearchButton.Visibility = Visibility.Visible;
			NavigationHeaderTextBlock.Visibility = Visibility.Visible;
		}

		private void SharedAutoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args) {
			SearchTerm = sender.Text;

			switch (CurrentPage) {
				case "Home":
					break;
				case "Locations":
					GoToPage("Locations");

					break;
				case "CheckIns":
					GoToPage("CheckIns");

					break;
				case "Events":
					GoToPage("Events");

					break;
			}
		}

		private async void SharedAutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args) {
			if (args.Reason.ToString() == "UserInput") {
				_cts = new CancellationTokenSource();
				CancellationToken token = _cts.Token;

				try {
					var searchList = new List<string>();

					switch (CurrentPage) {
						case "Home":
							break;
						case "Locations":
							var locations = new ObservableCollection<Location>();

							await Location.GetWorkspaceLocationsAsync(token, locations);
							searchList = locations.Where(l => l.Name.ToLower().StartsWith(sender.Text)).Select(l => l.Name).ToList();

							break;
						case "CheckIns":
							var checkins = new ObservableCollection<CheckIn>();
							searchList = checkins.Where(l => l.ProfileName.StartsWith(sender.Text)).Select(l => l.ProfileName).ToList();

							await CheckIn.GetCheckInsAsync(token, checkins);

							break;
						case "Events":
							break;
					}

					SharedAutoSuggestBox.ItemsSource = searchList;
				}
				catch (Exception) { }
				finally {
					_cts = null;
				}
			}
		}

		private void SharedAutoSuggestBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args) {
			sender.Text = args.SelectedItem.ToString();
		}
	}
}
