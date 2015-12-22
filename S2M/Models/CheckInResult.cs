using System.Collections.ObjectModel;

namespace S2M.Models {
	public class CheckInResult : ApiResult {
		public ObservableCollection<CheckIn> Results { get; set; }
	}
}
