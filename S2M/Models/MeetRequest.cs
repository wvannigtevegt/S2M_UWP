using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.Web.Http;


namespace S2M.Models
{
	public class MeetRequest: Chat
	{
		public int MeetRequestAccepted { get; set; }

		public static async Task GetProfileMeetRequestsAsync(ObservableCollection<MeetRequest> meetRequestlist)
		{
			var meetRequestResult = await GetMeetRequestsDataAsync();
			var meetRequests = meetRequestResult.Results;

			foreach (var meetRequest in meetRequests)
			{
				meetRequestlist.Add(meetRequest);
			}
		}

		private static async Task<MeetRequestResult> GetMeetRequestsDataAsync(string searchTerm = "")
		{
			var meetRequestResult = new MeetRequestResult();
			var meetRequest = new ObservableCollection<MeetRequest>();

			using (var httpClient = new Windows.Web.Http.HttpClient())
			{
				var apiKey = Common.StorageService.LoadSetting("ApiKey");
				var apiUrl = Common.StorageService.LoadSetting("ApiUrl");
				var profileToken = Common.StorageService.LoadSetting("ProfileToken");

				httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip");
				httpClient.DefaultRequestHeaders.Add("token", apiKey);
				httpClient.DefaultRequestHeaders.Add("api-version", "2");
				httpClient.DefaultRequestHeaders.Add("profileToken", profileToken);

				try
				{
					//var criteria = new LocationListCriteria {
					//	SearchTerm = searchTerm
					//};

					var url = apiUrl + "/api/meetrequest"; //+ JsonConvert.SerializeObject(criteria);

					using (var httpResponse = await httpClient.GetAsync(new Uri(url)))
					{
						string json = await httpResponse.Content.ReadAsStringAsync();
						json = json.Replace("<br>", Environment.NewLine);
						meetRequestResult = JsonConvert.DeserializeObject<MeetRequestResult>(json);
					}
				}
				catch (Exception) { }
			}

			return meetRequestResult;
		}

		public static async Task<MeetRequest> CreateMeetRequest(int profileId)
		{
			var meetRequest = new MeetRequest();

			using (var httpClient = new HttpClient())
			{
				var apiKey = Common.StorageService.LoadSetting("ApiKey");
				var apiUrl = Common.StorageService.LoadSetting("ApiUrl");
				var profileToken = Common.StorageService.LoadSetting("ProfileToken");

				httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip");
				httpClient.DefaultRequestHeaders.Add("token", apiKey);
				httpClient.DefaultRequestHeaders.Add("api-version", "2");
				httpClient.DefaultRequestHeaders.Add("profileToken", profileToken);

				var criteria = new NewChatCriteria
				{
					ProfileId = profileId,
					Message = ""
				};

				var uri = new Uri(apiUrl + "/api/meetrequest");
				var queryString = new HttpStringContent(JsonConvert.SerializeObject(criteria), Windows.Storage.Streams.UnicodeEncoding.Utf8, "application/json");

				HttpResponseMessage response = await httpClient.PostAsync(uri, queryString);
				string json = await response.Content.ReadAsStringAsync();
				json = json.Replace("<br>", Environment.NewLine);
				meetRequest = JsonConvert.DeserializeObject<MeetRequest>(json);
			}

			return meetRequest;
		}

		public static async Task<MeetRequest> AcceptMeetRequest(int id)
		{
			var meetRequest = new MeetRequest();

			using (var httpClient = new HttpClient())
			{
				var apiKey = Common.StorageService.LoadSetting("ApiKey");
				var apiUrl = Common.StorageService.LoadSetting("ApiUrl");
				var profileToken = Common.StorageService.LoadSetting("ProfileToken");

				httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip");
				httpClient.DefaultRequestHeaders.Add("token", apiKey);
				httpClient.DefaultRequestHeaders.Add("api-version", "2");
				httpClient.DefaultRequestHeaders.Add("profileToken", profileToken);

				var criteria = new ChangeStatusMeetRequestCriteria
				{
					MeetRequestId = id,
					Message = ""
				};

				var uri = new Uri(apiUrl + "/api/meetrequest/accept");
				var queryString = new HttpStringContent(JsonConvert.SerializeObject(criteria), Windows.Storage.Streams.UnicodeEncoding.Utf8, "application/json");

				HttpResponseMessage response = await httpClient.PostAsync(uri, queryString);
				string json = await response.Content.ReadAsStringAsync();
				json = json.Replace("<br>", Environment.NewLine);
				meetRequest = JsonConvert.DeserializeObject<MeetRequest>(json);
			}

			return meetRequest;
		}

		public static async Task<MeetRequest> DeclineMeetRequest(int id)
		{
			var meetRequest = new MeetRequest();

			using (var httpClient = new HttpClient())
			{
				var apiKey = Common.StorageService.LoadSetting("ApiKey");
				var apiUrl = Common.StorageService.LoadSetting("ApiUrl");
				var profileToken = Common.StorageService.LoadSetting("ProfileToken");

				httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip");
				httpClient.DefaultRequestHeaders.Add("token", apiKey);
				httpClient.DefaultRequestHeaders.Add("api-version", "2");
				httpClient.DefaultRequestHeaders.Add("profileToken", profileToken);

				var criteria = new ChangeStatusMeetRequestCriteria
				{
					MeetRequestId = id,
					Message = ""
				};

				var uri = new Uri(apiUrl + "/api/meetrequest/decline");
				var queryString = new HttpStringContent(JsonConvert.SerializeObject(criteria), Windows.Storage.Streams.UnicodeEncoding.Utf8, "application/json");

				HttpResponseMessage response = await httpClient.PostAsync(uri, queryString);
				string json = await response.Content.ReadAsStringAsync();
				json = json.Replace("<br>", Environment.NewLine);
				meetRequest = JsonConvert.DeserializeObject<MeetRequest>(json);
			}

			return meetRequest;
		}
	}

	public class ChangeStatusMeetRequestCriteria
	{
		public int MeetRequestId { get; set; } = 0;
		public string Message { get; set; } = "";
	}
}
