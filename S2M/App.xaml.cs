using Microsoft.QueryStringDotNET;
using System;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;


namespace S2M
{
	sealed partial class App : Application {
		public App() {
			this.InitializeComponent();
			this.Suspending += OnSuspending;

			SetAppVariables();
		}

		protected override void OnLaunched(LaunchActivatedEventArgs e) {
			Frame rootFrame = Window.Current.Content as Frame;

			if (rootFrame == null) {
				rootFrame = new Frame();

				rootFrame.NavigationFailed += OnNavigationFailed;

				if (e.PreviousExecutionState == ApplicationExecutionState.Terminated) {
					//TODO: Load state from previously suspended application
				}

				Window.Current.Content = rootFrame;
			}

			if (rootFrame.Content == null) {
				rootFrame.Navigate(typeof(MainPage));
			}
			Window.Current.Activate();
		}

		protected override void OnActivated(IActivatedEventArgs e)
		{
			if (e.Kind == ActivationKind.Protocol)
			{
				var frame = Window.Current.Content as Frame;
				if (frame == null)
				{
					frame = new Frame();
				}

				var protocolArgs = (ProtocolActivatedEventArgs)e;
				var uri = protocolArgs.Uri;
				var queryString = uri.Query;
				queryString = queryString.Replace("?", "").Replace("/", "");

				QueryString args = QueryString.Parse(queryString);

				switch (args["action"])
				{
					case "eventcheckin":
						var eventId = int.Parse(args["id"]);

						var ec_criteria = new NavigationPageCriteria
						{
							Action = "EventCheckInNFC",
							Id = eventId
						};

						frame.Navigate(typeof(MainPage), ec_criteria);

						break;
					case "locationcheckin":
						var locationId = int.Parse(args["id"]);

						var lc_criteria = new NavigationPageCriteria
						{
							Action = "LocationCheckInNFC",
							Id = locationId
						};

						frame.Navigate(typeof(MainPage), lc_criteria);

						break;
				}

				Window.Current.Content = frame;

				Window.Current.Activate();
			}
			else
			{
				Frame rootFrame = Window.Current.Content as Frame;

				if (rootFrame == null)
				{
					rootFrame = new Frame();

					rootFrame.NavigationFailed += OnNavigationFailed;

					if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
					{
						//TODO: Load state from previously suspended application
					}

					Window.Current.Content = rootFrame;
				}

				// Handle toast activation
				if (e is ToastNotificationActivatedEventArgs)
				{
					var toastActivationArgs = e as ToastNotificationActivatedEventArgs;

					QueryString args = QueryString.Parse(toastActivationArgs.Argument);

					switch (args["action"])
					{
						case "chat":
							int chatId = int.Parse(args["chatId"]);

							if (rootFrame.Content is Pages.ChatDetail && (rootFrame.Content as Pages.ChatDetail).ChatObject.Id.Equals(chatId))
							{
								break;
							}
								
							var criteria = new NavigationPageCriteria
							{
								Action = "ChatDetail",
								Id = chatId
							};

							rootFrame.Navigate(typeof(MainPage), criteria);
							break;
						case "checkin":

							break;
					}

					if (rootFrame.BackStack.Count == 0)
					{
						rootFrame.BackStack.Add(new PageStackEntry(typeof(MainPage), null, null));
					}
				}
				Window.Current.Activate();
			}
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
			Common.StorageService.SaveSetting("ApiUrl", "https://staging.seats2meet.com");
			Common.StorageService.SaveSetting("ChannelId", "1");
			Common.StorageService.SaveSetting("CountryId", "152");
		}

		
	}
}
