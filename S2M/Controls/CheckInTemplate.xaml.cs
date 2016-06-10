using S2M.Models;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace S2M.Controls {
	public sealed partial class CheckInTemplate : UserControl {
		public static readonly DependencyProperty ShowMatchPercentageProperty = DependencyProperty.Register("ShowMatchPercentage", typeof(object), typeof(bool), new PropertyMetadata(null));

		public CheckIn CheckIn { get { return this.DataContext as CheckIn; } }
		public bool ShowMatchPercentage
		{
			get { return (bool)GetValue(ShowMatchPercentageProperty); }
			set { SetValue(ShowMatchPercentageProperty, value); }
		}

		public CheckInTemplate() {
			this.InitializeComponent();

			ShowMatchPercentage = false;

			this.DataContextChanged += (s, e) => Bindings.Update();
		}
	}
}
