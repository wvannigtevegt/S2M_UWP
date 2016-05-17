using S2M.Models;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace S2M.Controls {
	public sealed partial class LocationTemplate : UserControl {
		//public static readonly DependencyProperty ShowLocationDistanceProperty = DependencyProperty.Register("ShowLocationDistance", typeof(object), typeof(bool), new PropertyMetadata(null));

		public Location Location { get { return this.DataContext as Location; } }
		public bool ShowLocationDistance { get; set; }
		//public bool ShowLocationDistance
		//{
		//	get { return (bool)GetValue(ShowLocationDistanceProperty); }
		//	set { SetValue(ShowLocationDistanceProperty, value); }
		//}


		public bool ShowSerendipityIndex { get; set; }

		public LocationTemplate() {
			this.InitializeComponent();
			//layoutRoot.DataContext = this;

			ShowSerendipityIndex = false;

			this.DataContextChanged += (s, e) => Bindings.Update();
		}
	}
}
