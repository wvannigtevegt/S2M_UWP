using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S2M.Models {
	public class Event {
		public int Id { get; set; }
		public int LocationId { get; set; }
		public DateTime Date { get; set; }
		public long StartTimeStamp { get; set; }
		public long EndTimeStamp { get; set; }
		public bool ShowTime { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public string Url { get; set; }
		public string Tags { get; set; }
		public decimal MatchPercentage { get; set; }
		public int NrOfCheckIns { get; set; }
		public string LocationImage { get; set; }
		public string LocationName { get; set; }
		public string LocationAddress { get; set; }
		public string LocationPostalcode { get; set; }
		public string LocationCity { get; set; }
		public string LocationPhone { get; set; }
		public string LocationEmail { get; set; }
		public double LocationLatitude { get; set; }
		public double LocationLongitude { get; set; }
		public double LocationDistance { get; set; }
		public string LocationImage_160 {
			get {
				if (!string.IsNullOrEmpty(LocationImage)) {
					var azureCdn = "https://az691754.vo.msecnd.net";
					var azureContainer = "website";

					var filenameWithoutExtension = LocationImage.Substring(0, LocationImage.LastIndexOf("."));
					var imagePath = azureCdn + "/" + azureContainer + "/" + LocationId.ToString() + "/160x120_" + filenameWithoutExtension + ".jpg";

					return imagePath;
				}
				return "";
			}
		}
		public string LocationImage_320 {
			get {
				if (!string.IsNullOrEmpty(LocationImage)) {
					var azureCdn = "https://az691754.vo.msecnd.net";
					var azureContainer = "website";

					var filenameWithoutExtension = LocationImage.Substring(0, LocationImage.LastIndexOf("."));
					var imagePath = azureCdn + "/" + azureContainer + "/" + LocationId.ToString() + "/320x240_" + filenameWithoutExtension + ".jpg";

					return imagePath;
				}
				return "";
			}
		}

		public static async Task GetEventsAsync(ObservableCollection<Event> eventList) {
			var eventObjs = await GetEventsDataAsync(DateTime.Now);

			foreach (var eventObj in eventObjs) {
				eventList.Add(eventObj);
			}
		}

		private static async Task<ObservableCollection<Event>> GetEventsDataAsync(DateTime date, int locationId = 0, string searchTerm = "", double latitude = 0, double longitude = 0, int radius = 0, string workingOn = "", int page = 0, int itemsPerPage = 0) {
			var events = new ObservableCollection<Event>();

			using (var httpClient = new Windows.Web.Http.HttpClient()) {
				var apiKey = Common.StorageService.LoadSetting("ApiKey");
				var apiUrl = Common.StorageService.LoadSetting("ApiUrl");

				httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip");
				httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
				httpClient.DefaultRequestHeaders.Add("token", apiKey);

				try {
					var criteria = new EventListCriteria {
						SearchTerm = searchTerm,
						Latitude = latitude,
						Longitude = longitude,
						Radius = radius,
						Page = page,
						ItemsPerPage = itemsPerPage
					};

					var url = apiUrl + "/api/event/calendar";
					if (locationId > 0) {
						url = url + "/location/" + locationId;
					}
					url = url + "/" + date.Year + "/" + date.Month + "/" + date.Day + "?" + JsonConvert.SerializeObject(criteria);

					var httpResponse = await httpClient.GetAsync(new Uri(url));
					string json = await httpResponse.Content.ReadAsStringAsync();
					json = json.Replace("<br>", Environment.NewLine);
					events = JsonConvert.DeserializeObject<ObservableCollection<Event>>(json);
				}
				catch (Exception e) { }
			}
			return events;
		}
	}

	public class EventListCriteria {
		public int ItemsPerPage { get; set; }
		public double Latitude { get; set; }
		public double Longitude { get; set; }
		public int Page { get; set; }
		public int Radius { get; set; }
		public string SearchTerm { get; set; }

	}
}
