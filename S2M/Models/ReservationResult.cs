using System.Collections.ObjectModel;

namespace S2M.Models {
	public class ReservationResult : ApiResult {
		public ObservableCollection<Reservation> Results { get; set; }
	}
}
