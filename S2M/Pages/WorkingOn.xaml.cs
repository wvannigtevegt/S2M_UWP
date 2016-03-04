using S2M.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace S2M.Pages {
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class WorkingOn : Page {
		protected CurrentCheckIn CurrentCheckIn { get; set; }
		public ObservableCollection<CheckIn> CheckInList { get; set; }
		protected string WorkingOnString { get; set; }

		private CancellationTokenSource _cts = null;
		private DispatcherTimer _timer;

		public WorkingOn() {
			this.InitializeComponent();

			CheckInList = new ObservableCollection<CheckIn>();
			WorkingOnString = "";
		}

		protected async override void OnNavigatedTo(NavigationEventArgs e)
		{
			await Common.StorageService.DeleteObjectAsync("CurrentCheckIn");
			await GetCurrentCheckInData();
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

		private async void Page_Loaded(object sender, RoutedEventArgs e) {
			await LoadCheckInsAsync();
		}

		private async Task GetCurrentCheckInData()
		{
			_cts = new CancellationTokenSource();
			CancellationToken token = _cts.Token;

			try
			{
				var checkIn = await CheckIn.GetCurrentCheckIn(token);
				var currentCheckIn = new CurrentCheckIn();
				currentCheckIn.Date = DateTime.Now;
				if (checkIn != null)
				{
					currentCheckIn.CheckIn = checkIn;
				}
				await Common.StorageService.PersistObjectAsync("CurrentCheckIn", currentCheckIn);
			}
			catch (Exception) { }
			finally
			{
				_cts = null;
			}
		}

		private async Task LoadCheckInsAsync()
		{
			_cts = new CancellationTokenSource();
			CancellationToken token = _cts.Token;

			try
			{
				var _checkinList = new ObservableCollection<CheckIn>();
				await CheckIn.GetCheckInsAsync(token, _checkinList, 0, 0, "", 0, 0, 0, "", 1, 25, true);
				foreach(var checkin in _checkinList)
				{
					if (checkin.WorkingOn.Length > 2)
					{
						CheckInList.Add(checkin);
					}
				}

				if (CheckInList.Any())
				{
					_timer = new DispatcherTimer()
					{
						Interval = TimeSpan.FromSeconds(3)
					};
					_timer.Tick += ChangeCheckInFlipView;
					_timer.Start();
				}
			}
			catch (Exception) { }
			finally
			{
				_cts = null;
			}
		}

		private async void SaveWorkingOnButton_Click(object sender, RoutedEventArgs e) {
			await SetWorkingOn();
		}

		private async void WorkingOnTextBox_KeyUp(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
		{
			if (e.Key == Windows.System.VirtualKey.Enter)
			{
				e.Handled = true;
				await SetWorkingOn();
			}
		}

		private async Task SetWorkingOn()
		{
			WorkingOnString = WorkingOnTextBox.Text.ToLower().Trim();
			if (!string.IsNullOrEmpty(WorkingOnString))
			{
				var workingOnObj = new Models.WorkingOn()
				{
					Text = WorkingOnString,
					EnteredOn = DateTime.Now
				};

				await Common.StorageService.PersistObjectAsync("WorkingOn", workingOnObj);

				Frame.Navigate(typeof(Home));
			}
		}

		private void ChangeCheckInFlipView(object sender, object o)
		{
			if (CheckInList.Any())
			{
				var totalItems = (CheckInList.Count - 1);
				var currentIndex = WorkingOnFlipView.SelectedIndex;
				var newItemIndex = currentIndex + 1 > totalItems ? 0 : currentIndex + 1;
				WorkingOnFlipView.SelectedIndex = newItemIndex;
			}
			
		}
	}
}
