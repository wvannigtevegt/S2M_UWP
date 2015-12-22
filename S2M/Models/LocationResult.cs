using System.Collections.ObjectModel;

namespace S2M.Models {
	public class LocationResult : ApiResult {
		public ObservableCollection<Location> Results { get; set; }
	}
}
