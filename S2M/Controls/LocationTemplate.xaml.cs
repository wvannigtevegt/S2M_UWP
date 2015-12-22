using S2M.Models;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace S2M.Controls {
	public sealed partial class LocationTemplate : UserControl {
		public Location Location { get { return this.DataContext as Location; } }
		public bool ShowLocationDistance { get; set; }

		public LocationTemplate() {
			this.InitializeComponent();

			ShowLocationDistance = false;

			this.DataContextChanged += (s, e) => Bindings.Update();
		}
	}
}
