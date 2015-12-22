using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.Web.Http;

namespace S2M.Models {
	public enum ActivityType {
		None,
		Chat,
		CheckedIn,
		MeetRequest,
		QuestionAnswered,
		QuestionAsked,
		QuestionBestAnswer
	}

	public class Activity {
		public string Id { get; set; }
		public ActivityType Type { get; set; }
		public int TypeId { get; set; }
		public int LocationId { get; set; }
		public long TimeStamp { get; set; }
		public string Message { get; set; }
		public int FromProfileId { get; set; }
		public string FromProfileName { get; set; }
		public string FromProfileImage { get; set; }
		public string FromProfileImage_84 {
			get {
				if (!string.IsNullOrEmpty(FromProfileImage)) {
					var imageCdn = "https://d3817ykd1rv0p7.cloudfront.net";

					var filenameWithoutExtension = FromProfileImage.Substring(0, FromProfileImage.LastIndexOf("."));
					var imagePath = imageCdn + "/" + FromProfileId.ToString() + "_" + filenameWithoutExtension + "_84x84.jpg";

					return imagePath;
				}
				return "";
			}
		}
		public int FromProfileScore { get; set; }
		public int ToProfileId { get; set; }
		public string ToProfileName { get; set; }
		public string ToProfileImage { get; set; }
		public int ToProfileScore { get; set; }

		public static async Task GetLocationActivitiesAsync(ObservableCollection<Activity> activitylist, int locationId) {
			var activityObjs = await GetActvityDataAsync(locationId, 0);

			foreach (var activityObj in activityObjs) {
				activitylist.Add(activityObj);
			}
		}

		public static async Task GetProfileActivitiesAsync(ObservableCollection<Activity> activitylist, int profileId) {
			var activityObjs = await GetActvityDataAsync(0, profileId);

			foreach (var activityObj in activityObjs) {
				if (activityObj != null) {
					activitylist.Add(activityObj);
				}
			}
		}

		private static async Task<ObservableCollection<Activity>> GetActvityDataAsync(int locationId = 0, int profileId = 0) {
			var activities = new ObservableCollection<Activity>();

			using (var httpClient = new HttpClient()) {
				var apiKey = Common.StorageService.LoadSetting("ApiKey");
				var apiUrl = Common.StorageService.LoadSetting("ApiUrl");

				httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip");
				httpClient.DefaultRequestHeaders.Add("token", apiKey);
				httpClient.DefaultRequestHeaders.Add("api-version", "2");

				try {
					var url = apiUrl + "/api/activity";
					if (locationId > 0) {
						url = url + "/location/" + locationId.ToString();
					}
					if (profileId > 0) {
						url = url + "/profile/" + profileId.ToString();
					}

					var httpResponse = await httpClient.GetAsync(new Uri(url));
					string json = await httpResponse.Content.ReadAsStringAsync();
					json = json.Replace("<br>", Environment.NewLine);
					activities = JsonConvert.DeserializeObject<ObservableCollection<Activity>>(json);
				}
				catch (Exception e) { }
			}

			return activities;
		}
	}
}
