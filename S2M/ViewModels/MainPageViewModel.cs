using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S2M.ViewModels {
	public class MainPageViewModel : INotifyPropertyChanged {
		private string _pageHeader { get; set; }

		public string PageHeader {
			get { return _pageHeader; }
			set {
				_pageHeader = value;
				NotifyPropertyChanged("PageHeader");
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		private void NotifyPropertyChanged(string propertyName) {
			var propertyChanged = PropertyChanged;
			if (propertyChanged != null) {
				propertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}
