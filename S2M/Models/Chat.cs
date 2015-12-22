using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.Web.Http;

namespace S2M.Models {
	public class Chat {
		public int Id { get; set; }
		public string Name { get; set; }
		public int CreatedBy { get; set; }
		public long CreatedOn { get; set; }
		public ObservableCollection<ChatMessage> Messages { get; set; }
		public ObservableCollection<ChatProfile> Profiles { get; set; }
		public long LastActivity { get; set; }
		public ChatMessage LastMessage {
			get {
				if (Messages.Any()) {
					return Messages.OrderByDescending(m => m.Id).Take(1).First();
				}
				return new ChatMessage();
			}

		}
		public bool IsMeetRequest { get; set; }
		public bool IsGroup { get; set; }
		public string Image {
			get {
				if (!IsGroup) {
					if (Profiles.Any()) {
						return Profiles.OrderByDescending(m => m.Id).Take(1).First().ProfileImage_84;
					}
				}
				return "";
			}
		}

		public static async Task GetProfileChatsAsync(ObservableCollection<Chat> chatlist) {
			var chatResult = await GetChatsDataAsync();
			var chats = chatResult.Results;

			foreach (var chat in chats) {
				chatlist.Add(chat);
			}
		}

		public static async Task<Chat> GetChatByIdAsync(int chatId) {
			var chat = new Chat();

			using (var httpClient = new Windows.Web.Http.HttpClient()) {
				var apiKey = Common.StorageService.LoadSetting("ApiKey");
				var apiUrl = Common.StorageService.LoadSetting("ApiUrl");
				var profileToken = Common.StorageService.LoadSetting("ProfileToken");

				httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip");
				httpClient.DefaultRequestHeaders.Add("token", apiKey);
				httpClient.DefaultRequestHeaders.Add("api-version", "2");
				httpClient.DefaultRequestHeaders.Add("profileToken", profileToken);

				try {

					var url = apiUrl + "/api/chat/" + chatId;

					using (var httpResponse = await httpClient.GetAsync(new Uri(url))) {
						string json = await httpResponse.Content.ReadAsStringAsync();
						json = json.Replace("<br>", Environment.NewLine);
						chat = JsonConvert.DeserializeObject<Chat>(json);
					}
				}
				catch (Exception) { }
			}

			return chat;
		}

		private static async Task<ChatResult> GetChatsDataAsync(string searchTerm = "") {
			var chatResult = new ChatResult();
			var chat = new ObservableCollection<Chat>();

			using (var httpClient = new Windows.Web.Http.HttpClient()) {
				var apiKey = Common.StorageService.LoadSetting("ApiKey");
				var apiUrl = Common.StorageService.LoadSetting("ApiUrl");
				var profileToken = Common.StorageService.LoadSetting("ProfileToken");

				httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip");
				httpClient.DefaultRequestHeaders.Add("token", apiKey);
				httpClient.DefaultRequestHeaders.Add("api-version", "2");
				httpClient.DefaultRequestHeaders.Add("profileToken", profileToken);

				try {
					//var criteria = new LocationListCriteria {
					//	SearchTerm = searchTerm
					//};

					var url = apiUrl + "/api/chat"; //+ JsonConvert.SerializeObject(criteria);

					using (var httpResponse = await httpClient.GetAsync(new Uri(url))) {
						string json = await httpResponse.Content.ReadAsStringAsync();
						json = json.Replace("<br>", Environment.NewLine);
						chatResult = JsonConvert.DeserializeObject<ChatResult>(json);
					}
				}
				catch (Exception e) { }
			}

			return chatResult;
		}

		public static async Task<Chat> CreateChat(int profileId) {
			var chat = new Chat();

			using (var httpClient = new HttpClient()) {
				var apiKey = Common.StorageService.LoadSetting("ApiKey");
				var apiUrl = Common.StorageService.LoadSetting("ApiUrl");
				var profileToken = Common.StorageService.LoadSetting("ProfileToken");

				httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip");
				httpClient.DefaultRequestHeaders.Add("token", apiKey);
				httpClient.DefaultRequestHeaders.Add("api-version", "2");
				httpClient.DefaultRequestHeaders.Add("profileToken", profileToken);

				var criteria = new NewChatCriteria {
					ProfileId = profileId,
					Message = ""
				};

				var uri = new Uri(apiUrl + "/api/chat");
				var queryString = new HttpStringContent(JsonConvert.SerializeObject(criteria), Windows.Storage.Streams.UnicodeEncoding.Utf8, "application/json");

				HttpResponseMessage response = await httpClient.PostAsync(uri, queryString);
				string json = await response.Content.ReadAsStringAsync();
				json = json.Replace("<br>", Environment.NewLine);
				chat = JsonConvert.DeserializeObject<Chat>(json);
			}

			return chat;
		}
	}

	public class NewChatCriteria {
		public int ProfileId { get; set; }
		public string Message { get; set; }
	}
}
