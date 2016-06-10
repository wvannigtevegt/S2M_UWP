using S2M.Models;
using S2M.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
	public sealed partial class Register : Page
	{
		public RegisterViewModel ViewModel { get; set; }

		public Register()
		{
			this.InitializeComponent();

			ViewModel = new RegisterViewModel();
			ViewModel.EnableButton = true;
			ViewModel.Profile = null;
			DataContext = ViewModel;
		}

		private async void AddTagButton_Click(object sender, RoutedEventArgs e)
		{
			var newTag = AddTagTextBox.Text.ToLower().Trim();

			if (!string.IsNullOrEmpty(newTag))
			{
				if (!ViewModel.Tags.Contains(newTag))
				{
					ViewModel.Tags.Add(newTag);
				}

				AddTagTextBox.Text = "";
			}
		}

		private async void DeleteTagButton_Click(object sender, RoutedEventArgs e)
		{
			var deleteTag = ((string)(sender as FrameworkElement).Tag);

			List<string> newTagList = ViewModel.Tags.Where(str => str != deleteTag).ToList();
			ViewModel.Tags.Clear();
			foreach (var tag in newTagList)
			{
				ViewModel.Tags.Add(tag);
			}
		}

		private async void buttonCreateProfile_Click(object sender, RoutedEventArgs e)
		{
			await ViewModel.RegisterNewProfile();
		}

		private async void ProfileRegisterButton_Click(object sender, RoutedEventArgs e)
		{
			Frame.Navigate(typeof(MainPage));
		}
	}
}
