using System;
using System.Collections.Generic;
using System.ComponentModel;
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

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace S2M.Controls {
	public sealed partial class DateTimeControl : UserControl {


		public DateTime Date
		{
			get { return (DateTime)GetValue(DateProperty); }
			set { SetValue(DateProperty, value); }
		}

		// Using a DependencyProperty as the backing store for Date.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty DateProperty =
			DependencyProperty.Register("Date", typeof(DateTime), typeof(DateTimeControl), new PropertyMetadata(0));

		public TimeSpan StartTime
		{
			get { return (TimeSpan)GetValue(StartTimeProperty); }
			set { SetValue(StartTimeProperty, value); }
		}

		// Using a DependencyProperty as the backing store for StartTime.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty StartTimeProperty =
			DependencyProperty.Register("StartTime", typeof(TimeSpan), typeof(DateTimeControl), new PropertyMetadata(0));

		public TimeSpan EndTime
		{
			get { return (TimeSpan)GetValue(EndTimeProperty); }
			set { SetValue(EndTimeProperty, value); }
		}

		// Using a DependencyProperty as the backing store for EndTime.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty EndTimeProperty =
			DependencyProperty.Register("EndTime", typeof(TimeSpan), typeof(DateTimeControl), new PropertyMetadata(0));


		//public event PropertyChangedEventHandler PropertyChanged;
		//void SetValue(DependencyProperty property, object value, [System.Runtime.CompilerServices.CallerMemberName] String p = null) 
		//{
		//	SetValue(property, value);
		//	if (PropertyChanged != null)
		//		PropertyChanged(this, new PropertyChangedEventArgs(p));
		//}

		public DateTimeControl() {
			this.InitializeComponent();

		}
	}
}
