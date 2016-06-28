using S2M.Models;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace S2M.Controls
{
	public sealed partial class ContactControl : UserControl
	{
		public Contact Contact { get { return this.DataContext as Contact; } }

		public ContactControl()
		{
			this.InitializeComponent();

			//this.DataContextChanged += (s, e) => Bindings.Update();
		}
	}
}
