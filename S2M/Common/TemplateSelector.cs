using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace S2M.Common {
	public abstract class TemplateSelector : ContentControl {
		public abstract DataTemplate SelectTemplate(object item, DependencyObject container);

		protected override void OnContentChanged(object oldContent, object newContent) {
			base.OnContentChanged(oldContent, newContent);

			ContentTemplate = SelectTemplate(newContent, this);
		}
	}
}
