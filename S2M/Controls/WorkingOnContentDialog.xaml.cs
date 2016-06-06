using S2M.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Content Dialog item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace S2M.Controls
{
	public enum ChangeWorkingOnResult
	{
		ChangeWorkingOnOK,
		ChangeWorkingOnCancel,
		Nothing
	}

	public sealed partial class WorkingOnContentDialog : ContentDialog
	{
		public ChangeWorkingOnResult Result { get; private set; }
		public string WorkingOn { get; set; }

		public WorkingOnContentDialog()
		{
			this.InitializeComponent();

			this.Opened += WorkingOnContentDialog_Opened;
			this.Closing += WorkingOnContentDialog_Closing;
		}

		private void WorkingOnContentDialog_Opened(ContentDialog sender, ContentDialogOpenedEventArgs args)
		{
			WorkingOnTextBox.Text = WorkingOn;
		}

		private void WorkingOnContentDialog_Closing(ContentDialog sender, ContentDialogClosingEventArgs args)
		{
			WorkingOn = WorkingOnTextBox.Text;
		}

		private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
		{
			this.Result = ChangeWorkingOnResult.ChangeWorkingOnOK;
		}

		private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
		{
			this.Result = ChangeWorkingOnResult.ChangeWorkingOnCancel;
		}
	}
}
