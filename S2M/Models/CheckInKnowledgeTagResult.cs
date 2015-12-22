using System.Collections.ObjectModel;

namespace S2M.Models {
	public class CheckInKnowledgeTagResult : ApiResult {
		public ObservableCollection<CheckInKnowledgeTag> Results { get; set; }
	}
}
