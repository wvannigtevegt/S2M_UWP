using Microsoft.QueryStringDotNET;
using System;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;


namespace S2M
{
	/// <summary>
	/// Provides application-specific behavior to supplement the default Application class.
	/// </summary>
	sealed partial class App : Application {
		/// <summary>
		/// Initializes the singleton application object.  This is the first line of authored code
		/// executed, and as such is the logical equivalent of main() or WinMain().
		/// </summary>
		public App() {
			this.InitializeComponent();
			this.Suspending += OnSuspending;

			SetAppVariables();
		}

		/// <summary>
		/// Invoked when the application is launched normally by the end user.  Other entry points
		/// will be used such as when the application is launched to open a specific file.
		/// </summary>
		/// <param name="e">Details about the launch request and process.</param>
		protected override void OnLaunched(LaunchActivatedEventArgs e) {
			

			Frame rootFrame = Window.Current.Content as Frame;

			// Do not repeat app initialization when the Window already has content,
			// just ensure that the window is active
			if (rootFrame == null) {
				// Create a Frame to act as the navigation context and navigate to the first page
				rootFrame = new Frame();

				rootFrame.NavigationFailed += OnNavigationFailed;

				if (e.PreviousExecutionState == ApplicationExecutionState.Terminated) {
					//TODO: Load state from previously suspended application
				}

				// Place the frame in the current Window
				Window.Current.Content = rootFrame;
			}

			if (rootFrame.Content == null) {
				// When the navigation stack isn't restored navigate to the first page,
				// configuring the new page by passing required information as a navigation
				// parameter
				//rootFrame.Navigate(typeof(MainPage), e.Arguments);

				rootFrame.Navigate(typeof(MainPage));
			}
			// Ensure the current window is active
			Window.Current.Activate();
		}

		protected override void OnActivated(IActivatedEventArgs e)
		{
			// Get the root frame
			Frame rootFrame = Window.Current.Content as Frame;

			// TODO: Initialize root frame just like in OnLaunched
			if (rootFrame == null)
			{
				// Create a Frame to act as the navigation context and navigate to the first page
				rootFrame = new Frame();

				rootFrame.NavigationFailed += OnNavigationFailed;

				if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
				{
					//TODO: Load state from previously suspended application
				}

				// Place the frame in the current Window
				Window.Current.Content = rootFrame;
			}

			// Handle toast activation
			if (e is ToastNotificationActivatedEventArgs)
			{
				var toastActivationArgs = e as ToastNotificationActivatedEventArgs;

				// Parse the query string
				QueryString args = QueryString.Parse(toastActivationArgs.Argument);

				// See what action is being requested 
				switch (args["action"])
				{
					// Open the image
					case "chat":

						int chatId = int.Parse(args["chatId"]);

						if (rootFrame.Content is Pages.ChatDetail && (rootFrame.Content as Pages.ChatDetail).ChatObject.Id.Equals(chatId))
							break;

						//// Otherwise navigate to view it
						var criteria = new NavigationPageCriteria
						{
							Action = "ChatDetail",
							Id = chatId
						};

						rootFrame.Navigate(typeof(MainPage), criteria);
						break;


					// Open the conversation
					case "checkin":

						//// The conversation ID retrieved from the toast args
						//int conversationId = int.Parse(args["conversationId"]);

						//// If we're already viewing that conversation, do nothing
						//if (rootFrame.Content is ConversationPage && (rootFrame.Content as ConversationPage).ConversationId == conversationId)
						//	break;

						//// Otherwise navigate to view it
						//rootFrame.Navigate(typeof(ConversationPage), conversationId);
						break;
				}

				// If we're loading the app for the first time, place the main page on
				// the back stack so that user can go back after they've been
				// navigated to the specific page
				if (rootFrame.BackStack.Count == 0)
					rootFrame.BackStack.Add(new PageStackEntry(typeof(MainPage), null, null));
			}

			// TODO: Handle other types of activation

			// Ensure the current window is active
			Window.Current.Activate();
		}


		/// <summary>
		/// Invoked when Navigation to a certain page fails
		/// </summary>
		/// <param name="sender">The Frame which failed navigation</param>
		/// <param name="e">Details about the navigation failure</param>
		void OnNavigationFailed(object sender, NavigationFailedEventArgs e) {
			throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
		}

		/// <summary>
		/// Invoked when application execution is being suspended.  Application state is saved
		/// without knowing whether the application will be terminated or resumed with the contents
		/// of memory still intact.
		/// </summary>
		/// <param name="sender">The source of the suspend request.</param>
		/// <param name="e">Details about the suspend request.</param>
		private void OnSuspending(object sender, SuspendingEventArgs e) {
			var deferral = e.SuspendingOperation.GetDeferral();
			//TODO: Save application state and stop any background activity
			deferral.Complete();
		}

		private static void SetAppVariables() {
			Common.StorageService.SaveSetting("ApiKey", "14257895");
			Common.StorageService.SaveSetting("ApiUrl", "https://www.seats2meet.com");
			Common.StorageService.SaveSetting("ChannelId", "1");
			Common.StorageService.SaveSetting("CountryId", "152");
		}

		
	}
}
