using S2M.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S2M.Models
{
	public class LocationDay : NotificationBase
	{
		private CheckIn _activeCheckIn = new CheckIn();
		private DateTime _date { get; set; }

		public CheckIn ActiveCheckIn
		{
			get { return _activeCheckIn; }
			set { SetProperty(_activeCheckIn, value, () => _activeCheckIn = value); }
		}

		public DateTime Date
		{
			get { return _date; }
			set { SetProperty(_date, value, () => _date = value); }
		}
	}
}
