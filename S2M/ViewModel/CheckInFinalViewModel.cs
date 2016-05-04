using S2M.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S2M.ViewModel
{
	public class CheckInFinalViewModel : NotificationBase
	{
		private CheckIn _checkIn = new CheckIn();
		private ObservableCollection<Option> _optionList = new ObservableCollection<Option>();
		private int _reservationId { get; set; }

		public CheckIn CheckIn
		{
			get { return _checkIn; }
			set { SetProperty(_checkIn, value, () => _checkIn = value); }
		}

		public ObservableCollection<Option> OptionList
		{
			get { return _optionList; }
			set { SetProperty(ref _optionList, value); }
		}

		public int ReservationId
		{
			get { return _reservationId; }
			set { SetProperty(_reservationId, value, () => _reservationId = value); }
		}
	}
}
