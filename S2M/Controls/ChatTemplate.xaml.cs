using S2M.Models;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace S2M.Controls {
	public sealed partial class ChatTemplate : UserControl {
		public Chat Chat { get { return this.DataContext as Chat; } }

		public ChatTemplate() {
			this.InitializeComponent();

			this.DataContextChanged += (s, e) => Bindings.Update();
		}
	}
}
