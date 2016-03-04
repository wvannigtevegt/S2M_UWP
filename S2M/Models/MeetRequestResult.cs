using System.Collections.ObjectModel;

namespace S2M.Models
{
	class MeetRequestResult : ApiResult
	{
		public ObservableCollection<MeetRequest> Results { get; set; }
	}
}
