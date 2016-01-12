using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S2M.Models {
	public class CheckInKnowledgeTag {
		public string Tag { get; set; }
		public ObservableCollection<CheckIn> CheckIns { get; set; }
		public int NrOfCheckIns {
			get {
				return CheckIns.Count;
			}
		}
		public ObservableCollection<CheckIn> CheckInsTopFive
		{
			get
			{
				var topFive = new ObservableCollection<CheckIn>();
				foreach(var checkIn in CheckIns.Take(5)) {
					topFive.Add(checkIn);
				}
				return topFive;
			}
		}

		public static async Task GetLocationCheckInKnowledgeTagsAsync(ObservableCollection<CheckInKnowledgeTag> tagList, int locationId = 0, string searchTerm = "") {
			const int channelId = 0;
			const int eventId = 0;

			var tagResults = await GetCheckInKnowledgeTagDataAsync(channelId, locationId, eventId, searchTerm);
			var tags = tagResults.Results;

			foreach (var tag in tags) {
				tagList.Add(tag);
			}
		}

		public static async Task GetEventCheckInKnowledgeTagsAsync(ObservableCollection<CheckInKnowledgeTag> tagList, int eventId = 0, string searchTerm = "") {
			const int channelId = 0;
			const int locationId = 0;

			var tagResults = await GetCheckInKnowledgeTagDataAsync(channelId, locationId, eventId, searchTerm);
			var tags = tagResults.Results;

			foreach (var tag in tags) {
				tagList.Add(tag);
			}
		}

		private static async Task<CheckInKnowledgeTagResult> GetCheckInKnowledgeTagDataAsync(int channelId = 0, int locationId = 0, int eventId = 0, string searchTerm = "", int page = 1, int itemsPerPage = 10) {
			var tagResults = new CheckInKnowledgeTagResult();

			using (var httpClient = new Windows.Web.Http.HttpClient()) {
				var apiKey = Common.StorageService.LoadSetting("ApiKey");
				var apiUrl = Common.StorageService.LoadSetting("ApiUrl");

				httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip");
				httpClient.DefaultRequestHeaders.Add("token", apiKey);
				httpClient.DefaultRequestHeaders.Add("api-version", "2");

				try {
					var criteria = new TagKnowledgeCriteria {
						ChannelId = channelId,
						EventId = eventId,
						ItemsPerPage = itemsPerPage,
						LocationId = locationId,
						Page = page,
						SearchTerm = searchTerm
					};

					var url = apiUrl + "/api/checkin/knowledge?" + JsonConvert.SerializeObject(criteria);

					var httpResponse = await httpClient.GetAsync(new Uri(url));
					string json = await httpResponse.Content.ReadAsStringAsync();
					json = json.Replace("<br>", Environment.NewLine);
					tagResults = JsonConvert.DeserializeObject<CheckInKnowledgeTagResult>(json);
				}
				catch (Exception e) { }
			}

			return tagResults;
		}

		public class TagKnowledgeCriteria {
			public int ChannelId { get; set; }
			//public DateTime Date { get; set; }
			public int EventId { get; set; }
			public int ItemsPerPage { get; set; }
			public int LocationId { get; set; }
			public int Page { get; set; }
			public string SearchTerm { get; set; }
		}
	}
}