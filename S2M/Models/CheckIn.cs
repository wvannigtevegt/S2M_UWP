using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using Windows.Web.Http;

namespace S2M.Models {
	public class CheckIn {
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
		public string ProfileImage_84 {
			get {
				if (!string.IsNullOrEmpty(ProfileImage)) {
					var imageCdn = "https://d3817ykd1rv0p7.cloudfront.net";

					var filenameWithoutExtension = ProfileImage.Substring(0, ProfileImage.LastIndexOf("."));
					var imagePath = imageCdn + "/" + ProfileId.ToString() + "_" + filenameWithoutExtension + "_84x84.jpg";

					return imagePath;
				}
				return "ms-appx:///Assets/defaultProfileImage_84x84.png";
			}
		}
		public string ProfileImage_150 {
			get {
				if (!string.IsNullOrEmpty(ProfileImage)) {
					var imageCdn = "https://d3817ykd1rv0p7.cloudfront.net";

					var filenameWithoutExtension = ProfileImage.Substring(0, ProfileImage.LastIndexOf("."));
					var imagePath = imageCdn + "/" + ProfileId.ToString() + "_" + filenameWithoutExtension + "_150x150.jpg";
					return imagePath;
				}
				return "ms-appx:///Assets/defaultProfileImage_84x84.png";
			}
		}

		public static async Task GetCheckInsAsync(CancellationToken token, ObservableCollection<CheckIn> checkinList, int locationId = 0, int eventId = 0, string searchTerm = "", double latitude = 0, double longitude = 0, int radius = 0, string workingOn = "", int page = 0, int itemsPerPage = 0, bool allDay = false) {
			var checkInResult = await GetCheckInsDataAsync(token, locationId, eventId, searchTerm, latitude, longitude, radius, workingOn, page, itemsPerPage, allDay);
			var checkins = checkInResult.Results;

			foreach (var checkin in checkins) {
				checkinList.Add(checkin);
			}
		}

		public static async Task<string> GetNrOfCheckInsLiveTileAsync(int locationId = 0) {
			var tileContent = "";

			using (var httpClient = new HttpClient()) {
				var apiKey = Common.StorageService.LoadSetting("ApiKey");
				var apiUrl = Common.StorageService.LoadSetting("ApiUrl");

				httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip");
				//httpClient.DefaultRequestHeaders.Add("token", apiKey);
				//httpClient.DefaultRequestHeaders.Add("api-version", "2");

				try {
					var url = apiUrl + "/api/livetile/nrofcheckins";

					var httpResponse = await httpClient.GetAsync(new Uri(url));
					string json = await httpResponse.Content.ReadAsStringAsync();
					json = json.Replace("<br>", Environment.NewLine);
					tileContent = json;
					//checkInResult = JsonConvert.DeserializeObject<string>(json);
				}
				catch (Exception e) { }
			}

			return tileContent;
		}

		private static async Task<CheckInResult> GetCheckInsDataAsync(CancellationToken token, int locationId = 0, int eventId = 0, string searchTerm = "", double latitude = 0, double longitude = 0, int radius = 0, string workingOn = "", int page = 0, int itemsPerPage = 0, bool allDay = false) {
			var checkInResult = new CheckInResult();
			var checkins = new ObservableCollection<CheckIn>();

			var authenticatedProfile = await Common.StorageService.RetrieveObjectAsync<Profile>("Profile");

			using (var httpClient = new HttpClient()) {
				var apiKey = Common.StorageService.LoadSetting("ApiKey");
				var apiUrl = Common.StorageService.LoadSetting("ApiUrl");

				httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip");
				httpClient.DefaultRequestHeaders.Add("token", apiKey);
				httpClient.DefaultRequestHeaders.Add("api-version", "2");

				try {
					var criteria = new CheckInListCriteria {
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
					if (allDay) {
						url = url + "/allday";
					}
					if (locationId > 0) {
						url = url + "/location/" + locationId;
					}
					if (eventId > 0) {
						url = url + "/event/" + eventId;
					}
					//else {
					url = url + "?" + JsonConvert.SerializeObject(criteria);
					//}


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

	public class CheckInListCriteria {
		public long DateTimeStamp { get; set; }
		public int ItemsPerPage { get; set; }
		public double Latitude { get; set; }
		public double Longitude { get; set; }
		public int Page { get; set; }
		public int Radius { get; set; }
		public string SearchTerm { get; set; }
		public string WorkingOn { get; set; }
		public int FilterProfileId { get; set; }
	}
}
