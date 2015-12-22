using System.Collections.ObjectModel;

namespace S2M.Models {
	public class ChatResult : ApiResult {
		public ObservableCollection<Chat> Results { get; set; }
	}
}
