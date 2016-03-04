using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.Web.Http;

namespace S2M.Models
{
	public class CheckIn
	{
		public int Id { get; set; }
		public int ChannelId { get; set; }
		public int ReservationId { get; set; }
		public int EvenId { get; set; }
		public string EventName { get; set; }
		public int ProfileId { get; set; }
		public string ProfileKey { get; set; }
		public string ProfileName { get; set; }
		public string ProfileImage { get; set; }
		public string[] Tags { get; set; }
		public string WorkingOn { get; set; }
		public int LocationId { get; set; }
		public string LocationName { get; set; }
		public string LocationImage { get; set; }
		public double LocationLatitude { get; set; }
		public double LocationLongitude { get; set; }
		public int MatchPercentage { get; set; }
		public string TagMatches { get; set; }
		public double Distance { get; set; }
		public long StartTimeStamp { get; set; }
		public long EndTimeStamp { get; set; }
		public bool IsEmployee { get; set; }
		public bool HasLeft { get; set; }
		public bool IsConfirmed { get; set; }
		public string ProfileImage_84
		{
			get
			{
				if (!string.IsNullOrEmpty(ProfileImage))
				{
					var imageCdn = "https://d3817ykd1rv0p7.cloudfront.net";

					var filenameWithoutExtension = ProfileImage.Substring(0, ProfileImage.LastIndexOf("."));
					var imagePath = imageCdn + "/" + ProfileId.ToString() + "_" + filenameWithoutExtension + "_84x84.jpg";

					return imagePath;
				}
				return "ms-appx:///Assets/defaultProfileImage_84x84.png";
			}
		}
		public string ProfileImage_150
		{
			get
			{
				if (!string.IsNullOrEmpty(ProfileImage))
				{
					var imageCdn = "https://d3817ykd1rv0p7.cloudfront.net";

					var filenameWithoutExtension = ProfileImage.Substring(0, ProfileImage.LastIndexOf("."));
					var imagePath = imageCdn + "/" + ProfileId.ToString() + "_" + filenameWithoutExtension + "_150x150.jpg";
					return imagePath;
				}
				return "ms-appx:///Assets/defaultProfileImage_84x84.png";
			}
		}

		public static async Task<CheckIn> CheckInToEvent(CancellationToken token, int eventDateId, string workingOn = "")
		{
			var checkIn = new CheckIn();

			using (var httpClient = new HttpClient())
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
					var criteria = new SaveEventCheckInCriteria
					{
						WorkingOn = workingOn
					};

					var url = apiUrl + "/api/checkin/event/" + eventDateId;
					var queryString = new HttpStringContent(JsonConvert.SerializeObject(criteria), Windows.Storage.Streams.UnicodeEncoding.Utf8, "application/json");

					using (var httpResponse = await httpClient.PostAsync(new Uri(url), queryString).AsTask(token))
					{
						string json = await httpResponse.Content.ReadAsStringAsync().AsTask(token);
						json = json.Replace("<br>", Environment.NewLine);
						checkIn = JsonConvert.DeserializeObject<CheckIn>(json);
					}
				}
				catch (Exception) { }
			}

			return checkIn;
		}

		public static async Task<CheckIn> GetCurrentCheckIn(CancellationToken token)
		{
			var checkInResult = new CheckInResult();
			var checkIn = new CheckIn();

			var authenticatedProfile = await Common.StorageService.RetrieveObjectAsync<Profile>("Profile");

			using (var httpClient = new HttpClient())
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
					var url = apiUrl + "/api/checkin/current/" + authenticatedProfile.Id;

					var httpResponse = await httpClient.GetAsync(new Uri(url)).AsTask(token);
					string json = await httpResponse.Content.ReadAsStringAsync().AsTask(token);
					json = json.Replace("<br>", Environment.NewLine);
					checkInResult = JsonConvert.DeserializeObject<CheckInResult>(json);

					if (checkInResult.Results.Any())
					{
						checkIn = checkInResult.Results.First();
					}
				}
				catch (Exception) { }
			}

			return checkIn;
		}

		public static async Task GetProfileCheckInsAsync(CancellationToken token, ObservableCollection<CheckIn> checkinList)
		{
			var checkInResult = new CheckInResult();
			var checkins = new ObservableCollection<CheckIn>();

			var authenticatedProfile = await Common.StorageService.RetrieveObjectAsync<Profile>("Profile");

			using (var httpClient = new HttpClient())
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
					var url = apiUrl + "/api/checkin/profile/" + authenticatedProfile.Id;

					var httpResponse = await httpClient.GetAsync(new Uri(url)).AsTask(token);
					string json = await httpResponse.Content.ReadAsStringAsync().AsTask(token);
					json = json.Replace("<br>", Environment.NewLine);
					checkInResult = JsonConvert.DeserializeObject<CheckInResult>(json);
				}
				catch (Exception) { }
			}

			foreach (var checkin in checkInResult.Results)
			{
				checkinList.Add(checkin);
			}
		}

		public static async Task GetCheckInsAsync(CancellationToken token, ObservableCollection<CheckIn> checkinList, int locationId = 0, int eventId = 0, string searchTerm = "", double latitude = 0, double longitude = 0, int radius = 0, string workingOn = "", int page = 0, int itemsPerPage = 0, bool allDay = false)
		{
			var checkInResult = await GetCheckInsDataAsync(token, locationId, eventId, searchTerm, latitude, longitude, radius, workingOn, page, itemsPerPage, allDay);
			var checkins = checkInResult.Results.ToList();

			if (!string.IsNullOrEmpty(workingOn))
			{
				checkins = checkins.OrderByDescending(c => c.MatchPercentage).ToList();
			}

			foreach (var checkin in checkins)
			{
				checkinList.Add(checkin);
			}
		}

		public static async Task GetCheckInsEventDateAsync(CancellationToken token, ObservableCollection<CheckIn> checkinList, EventCalendar eventObject)
		{
			var checkInResult = new CheckInResult();

			using (var httpClient = new HttpClient())
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
					var url = apiUrl + "/api/checkin/event/" + eventObject.EventId + "/" + eventObject.Date.Year + "/" + eventObject.Date.Month + "/" + eventObject.Date.Day;

					using (var httpResponse = await httpClient.GetAsync(new Uri(url)).AsTask(token))
					{
						string json = await httpResponse.Content.ReadAsStringAsync().AsTask(token);
						json = json.Replace("<br>", Environment.NewLine);
						checkInResult = JsonConvert.DeserializeObject<CheckInResult>(json);

						foreach(var checkin in checkInResult.Results)
						{
							checkinList.Add(checkin);
						}
					}
				}
				catch (Exception) { }
			}
		}

		public static async Task GetEventCheckinRecommendationsAsync(CancellationToken token, ObservableCollection<CheckIn> checkinList, int eventCalendarId, string workingOn = "", int page = 1, int itemsPerPage = 10)
		{
			var checkInResult = new CheckInResult();

			using (var httpClient = new HttpClient())
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
					var criteria = new CheckInListCriteria
					{
						ItemsPerPage = itemsPerPage,
						Page = page,
						WorkingOn = workingOn,
					};

					var url = apiUrl + "/api/checkin/recommendation/event/" + eventCalendarId;
					url = url + "?" + JsonConvert.SerializeObject(criteria);

					using (var httpResponse = await httpClient.GetAsync(new Uri(url)).AsTask(token))
					{
						string json = await httpResponse.Content.ReadAsStringAsync().AsTask(token);
						json = json.Replace("<br>", Environment.NewLine);
						checkInResult = JsonConvert.DeserializeObject<CheckInResult>(json);

						foreach (var checkin in checkInResult.Results)
						{
							checkinList.Add(checkin);
						}
					}
				}
				catch (Exception) { }
			}
		}

		//public static async Task GetCheckinRecommendationsByCheckInAsync(CancellationToken token, ObservableCollection<CheckIn> checkinList, int checkinId, )

		public static async Task<string> GetNrOfCheckInsLiveTileAsync(int locationId = 0)
		{
			var tileContent = "";

			using (var httpClient = new HttpClient())
			{
				var apiKey = Common.StorageService.LoadSetting("ApiKey");
				var apiUrl = Common.StorageService.LoadSetting("ApiUrl");

				httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip");
				//httpClient.DefaultRequestHeaders.Add("token", apiKey);
				//httpClient.DefaultRequestHeaders.Add("api-version", "2");

				try
				{
					var url = apiUrl + "/api/livetile/nrofcheckins";

					var httpResponse = await httpClient.GetAsync(new Uri(url));
					string json = await httpResponse.Content.ReadAsStringAsync();
					json = json.Replace("<br>", Environment.NewLine);
					tileContent = json;
					//checkInResult = JsonConvert.DeserializeObject<string>(json);
				}
				catch (Exception) { }
			}

			return tileContent;
		}

		public static async Task UpdateCheckIn(CancellationToken token, CheckIn checkin)
		{
			using (var httpClient = new HttpClient())
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
					var url = apiUrl + "/api/checkin";
					var queryString = new HttpStringContent(JsonConvert.SerializeObject(checkin), Windows.Storage.Streams.UnicodeEncoding.Utf8, "application/json");

					using (var httpResponse = await httpClient.PostAsync(new Uri(url), queryString).AsTask(token))
					{
						string json = await httpResponse.Content.ReadAsStringAsync().AsTask(token);
						json = json.Replace("<br>", Environment.NewLine);
						//result = JsonConvert.DeserializeObject<int>(json);
					}
				}
				catch (Exception) { }
			}
		}

		public static async Task<int> CancelCheckIn(CancellationToken token, int id)
		{
			var result = -1;

			using (var httpClient = new HttpClient())
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
					var url = apiUrl + "/api/checkin/" + id;

					using (var httpResponse = await httpClient.DeleteAsync(new Uri(url)).AsTask(token))
					{
						string json = await httpResponse.Content.ReadAsStringAsync().AsTask(token);
						json = json.Replace("<br>", Environment.NewLine);
						result = JsonConvert.DeserializeObject<int>(json);
					}
				}
				catch (Exception) { }
			}

			return result;
		}

		private static async Task<CheckInResult> GetCheckInsDataAsync(CancellationToken token, int locationId = 0, int eventId = 0, string searchTerm = "", double latitude = 0, double longitude = 0, int radius = 0, string workingOn = "", int page = 0, int itemsPerPage = 0, bool allDay = false)
		{
			var checkInResult = new CheckInResult();
			var checkins = new ObservableCollection<CheckIn>();

			var authenticatedProfile = await Common.StorageService.RetrieveObjectAsync<Profile>("Profile");

			using (var httpClient = new HttpClient())
			{
				var apiKey = Common.StorageService.LoadSetting("ApiKey");
				var apiUrl = Common.StorageService.LoadSetting("ApiUrl");

				httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip");
				httpClient.DefaultRequestHeaders.Add("token", apiKey);
				httpClient.DefaultRequestHeaders.Add("api-version", "2");

				try
				{
					var criteria = new CheckInListCriteria
					{
						DateTimeStamp = Common.DateService.ToJavaScriptMilliseconds(DateTime.Now),
						ItemsPerPage = itemsPerPage,
						Latitude = latitude,
						Longitude = longitude,
						Page = page,
						Radius = radius,
						SearchTerm = searchTerm,
						WorkingOn = workingOn,
						FilterProfileId = authenticatedProfile.Id
					};

					var url = apiUrl + "/api/checkin";
					if (allDay)
					{
						url = url + "/allday";
					}
					if (locationId > 0)
					{
						url = url + "/location/" + locationId;
					}
					if (eventId > 0)
					{
						url = url + "/event/" + eventId;
					}
					if (eventId == 0)
					{
						url = url + "?" + JsonConvert.SerializeObject(criteria);
					}


					var httpResponse = await httpClient.GetAsync(new Uri(url)).AsTask(token);
					string json = await httpResponse.Content.ReadAsStringAsync().AsTask(token);
					json = json.Replace("<br>", Environment.NewLine);
					checkInResult = JsonConvert.DeserializeObject<CheckInResult>(json);
				}
				catch (Exception e) { }
			}

			return checkInResult;
		}
	}

	public class CheckInListCriteria
	{
		public long DateTimeStamp { get; set; } = 0;
		public int ItemsPerPage { get; set; } = 0;
		public double Latitude { get; set; } = 0;
		public double Longitude { get; set; } = 0;
		public int Page { get; set; } = 0;
		public int Radius { get; set; } = 0;
		public string SearchTerm { get; set; } = "";
		public string WorkingOn { get; set; } = "";
		public int FilterProfileId { get; set; } = 0;
	}

	public class SaveEventCheckInCriteria
	{
		public string WorkingOn { get; set; } = "";
	}
}
