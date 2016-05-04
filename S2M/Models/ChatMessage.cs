using Newtonsoft.Json;
using S2M.Common;
using System;
using System.Threading.Tasks;
using Windows.Web.Http;

namespace S2M.Models {
	public class ChatMessage {
		public int Id { get; set; }
		public int ChatId { get; set; }
		public string Message { get; set; }
		public int ProfileId { get; set; }
		public string ProfileName { get; set; }
		public long CreatedOn { get; set; }
		public DateTime CreatedOnDate
		{
			get
			{
				return DateService.ConvertFromUnixTimestamp(this.CreatedOn).ToLocalTime();
			}
		}
		public bool IsSystem { get; set; }
		public string Type { get; set; }
		public string ProfileImage { get; set; }
		public string ProfileImage_84 {
			get {
				if (!string.IsNullOrEmpty(ProfileImage)) {
					var imageCdn = "https://d3817ykd1rv0p7.cloudfront.net";

					var filenameWithoutExtension = ProfileImage.Substring(0, ProfileImage.LastIndexOf("."));
					var imagePath = imageCdn + "/" + ProfileId.ToString() + "_" + filenameWithoutExtension + "_84x84.jpg";

					return imagePath;
				}
				return "";
			}
		}

		public static async Task<Chat> PostChatMessage(int chatId, string message) {
			var chat = new Chat();

			//var authenticatedProfile = await Common.StorageService.RetrieveObjectAsync<Profile>("Profile");

			using (var httpClient = new HttpClient()) {
				var apiKey = Common.StorageService.LoadSetting("ApiKey");
				var apiUrl = Common.StorageService.LoadSetting("ApiUrl");
				var profileToken = Common.StorageService.LoadSetting("ProfileToken");

				httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip");
				httpClient.DefaultRequestHeaders.Add("token", apiKey);
				httpClient.DefaultRequestHeaders.Add("api-version", "2");
				httpClient.DefaultRequestHeaders.Add("profileToken", profileToken);

				var criteria = new NewChatMessageCriteria {
					ChatId = chatId,
					Message = message
				};

				var uri = new Uri(apiUrl + "/api/chat/message");
				var queryString = new HttpStringContent(JsonConvert.SerializeObject(criteria), Windows.Storage.Streams.UnicodeEncoding.Utf8, "application/json");

				HttpResponseMessage response = await httpClient.PostAsync(uri, queryString);
				string json = await response.Content.ReadAsStringAsync();
				json = json.Replace("<br>", Environment.NewLine);
				chat = JsonConvert.DeserializeObject<Chat>(json);
			}

			return chat;
		}
	}

	public class NewChatMessageCriteria {
		public int ChatId { get; set; } = 0;
		public string Message { get; set; } = "";
	}
}
