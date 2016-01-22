using Newtonsoft.Json;
using S2M.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace S2M.Models {
	public class Location {
		public int Id { get; set; }
		public string Name { get; set; }
		public string Address { get; set; }
		public string Zipcode { get; set; }
		public string City { get; set; }
		public string Addition { get; set; }
		public string Country { get; set; }
		public string State { get; set; }
		public string Email { get; set; }
		public string Phone { get; set; }
		public string Url { get; set; }
		public double Longitude { get; set; }
		public double Latitude { get; set; }
		public double Distance { get; set; }
		public string MatchedTags { get; set; }
		public decimal ReviewScore { get; set; }
		public decimal ReviewCount { get; set; }
		public decimal MatchPercentage { get; set; }
		public int NrOfCheckIns { get; set; }
		public string StarImage {
			get {
				if (ReviewScore > 0) {
					decimal roundedReviewScore = 0;

					roundedReviewScore = (Math.Round((ReviewScore / 2) / 5) * 5);
					return "/Images/rating-" + roundedReviewScore + ".png";
				}
				return "/Images/rating-0.png";
			}
		}
		public string StarImageSmall {
			get {
				if (ReviewScore > 0) {
					decimal roundedReviewScore = 0;

					roundedReviewScore = (Math.Round((ReviewScore / 2) / 5) * 5);
					return "/Images/rating-" + roundedReviewScore + "-small.png";
				}
				return "/Images/rating-0.png";
			}
		}
		public string Image { get; set; }
		public string Image_160 {
			get {
				if (!string.IsNullOrEmpty(Image)) {
					var azureCdn = "https://az691754.vo.msecnd.net";
					var azureContainer = "website";

					var filenameWithoutExtension = Image.Substring(0, Image.LastIndexOf("."));
					var imagePath = azureCdn + "/" + azureContainer + "/" + Id.ToString() + "/160x120_" + filenameWithoutExtension + ".jpg";

					return imagePath;
				}
				return "Assets/StoreLogo.png";
			}
		}
		public string Image_320 {
			get {
				if (!string.IsNullOrEmpty(Image)) {
					var azureCdn = "https://az691754.vo.msecnd.net";
					var azureContainer = "website";

					var filenameWithoutExtension = Image.Substring(0, Image.LastIndexOf("."));
					var imagePath = azureCdn + "/" + azureContainer + "/" + Id.ToString() + "/320x240_" + filenameWithoutExtension + ".jpg";

					return imagePath;
				}
				return "";
			}
		}

		public static async Task<Location> GetLocationById(CancellationToken token, int locationId)
		{
			Location location = null;

			using (var httpClient = new Windows.Web.Http.HttpClient())
			{
				var apiKey = StorageService.LoadSetting("ApiKey");
				var apiUrl = StorageService.LoadSetting("ApiUrl");

				httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip");
				httpClient.DefaultRequestHeaders.Add("token", apiKey);
				httpClient.DefaultRequestHeaders.Add("api-version", "2");

				try
				{
					var url = apiUrl + "/api/locations/" + locationId;

					using (var httpResponse = await httpClient.GetAsync(new Uri(url)).AsTask(token))
					{
						string json = await httpResponse.Content.ReadAsStringAsync().AsTask(token);
						json = json.Replace("<br>", Environment.NewLine);

						location = JsonConvert.DeserializeObject<Location>(json);
					}
				}
				catch (Exception) { }
			}

			return location;
		}

		public static async Task GetLocationRecommendationsAsync(CancellationToken token, ObservableCollection<Location> locationList, double latitude = 0, double longitude = 0, int radius = 0, string workingOn = "", int page = 0, int itemsPerPage = 0) {
			var locationResult = new LocationResult();

			using (var httpClient = new Windows.Web.Http.HttpClient()) {
				var apiKey = StorageService.LoadSetting("ApiKey");
				var apiUrl = StorageService.LoadSetting("ApiUrl");
				var channelId = int.Parse(StorageService.LoadSetting("ChannelId"));
				var countryId = int.Parse(StorageService.LoadSetting("CountryId"));

				httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip");
				httpClient.DefaultRequestHeaders.Add("token", apiKey);
				httpClient.DefaultRequestHeaders.Add("api-version", "2");

				try {
					var criteria = new LocationListCriteria {
						ItemsPerPage = itemsPerPage,
						Latitude = latitude,
						Longitude = longitude,
						Page = page,
						Radius = radius,
						WorkingOn = workingOn
					};

					var url = apiUrl + "/api/locations/recommendation?" + JsonConvert.SerializeObject(criteria);

					using (var httpResponse = await httpClient.GetAsync(new Uri(url)).AsTask(token)) {
						string json = await httpResponse.Content.ReadAsStringAsync().AsTask(token);
						json = json.Replace("<br>", Environment.NewLine);
						locationResult = JsonConvert.DeserializeObject<LocationResult>(json);
					}
				}
				catch (Exception) { }
			}

			foreach (var location in locationResult.Results) {
				locationList.Add(location);
			}
		}

		public static async Task GetWorkspaceLocationsAsync(CancellationToken token, ObservableCollection<Location> locationList, string searchTerm = "", double latitude = 0, double longitude = 0, int radius = 0, string workingOn = "", int page = 0, int itemsPerPage = 0) {
			var locations = new List<Location>();

			if (ConnectionHelper.CheckForInternetAccess()) {
				var locationResultA = await GetLocationsDataAsync(token, searchTerm, latitude, longitude);
				locations = locationResultA.Results.ToList();
			}
			else {
				var locationResultB = await GetLocationsFromFileAsync();
				locations = locationResultB.Results.ToList();

				if (!string.IsNullOrEmpty(searchTerm)) {
					locations = locations.Where(s => s.Name.ToLower().StartsWith(searchTerm)).ToList();
				}
			}

			foreach (var location in locations) {
				locationList.Add(location);
			}
		}

		public static async Task<LocationResult> GetLocationsFromFileAsync() {
			try {
				var json = await FileHelper.ReadStringFromLocalFile("locations.json");
				if (!string.IsNullOrEmpty(json)) {
					var locationResult = JsonConvert.DeserializeObject<LocationResult>(json);
					return locationResult;
				}
			}
			catch (Exception) { }

			return null;
		}

		private static async Task<LocationResult> GetLocationsDataAsync(CancellationToken token, string searchTerm = "", double latitude = 0, double longitude = 0, int radius = 0, string workingOn = "", int page = 0, int itemsPerPage = 0) {
			var locationResult = new LocationResult();

			using (var httpClient = new Windows.Web.Http.HttpClient()) {
				var apiKey = StorageService.LoadSetting("ApiKey");
				var apiUrl = StorageService.LoadSetting("ApiUrl");
				var channelId = int.Parse(StorageService.LoadSetting("ChannelId"));
				var countryId = int.Parse(StorageService.LoadSetting("CountryId"));

				httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip");
				httpClient.DefaultRequestHeaders.Add("token", apiKey);
				httpClient.DefaultRequestHeaders.Add("api-version", "2");

				try {
					var criteria = new LocationListCriteria {
						ChannelId = channelId,
						CountryId = countryId,
                        ItemsPerPage = itemsPerPage,
						Latitude = latitude,
						Longitude = longitude,
						Page = page,
						Radius = radius,
						SearchTerm = searchTerm,
						WorkingOn = workingOn
					};

					var url = apiUrl + "/api/locations/workspace?" + JsonConvert.SerializeObject(criteria);

					using (var httpResponse = await httpClient.GetAsync(new Uri(url)).AsTask(token)) {
						string json = await httpResponse.Content.ReadAsStringAsync().AsTask(token);
						json = json.Replace("<br>", Environment.NewLine);
						if (string.IsNullOrEmpty(searchTerm)) {
							await FileHelper.SaveStringToLocalFile("locations.json", json);
						}
						locationResult = JsonConvert.DeserializeObject<LocationResult>(json);
					}
				}
				catch (Exception) { }
			}

			return locationResult;
		}
	}

	public class LocationListCriteria {
		public int ChannelId { get; set; }
		public int CountryId { get; set; }
		public int ItemsPerPage { get; set; }
		public double Latitude { get; set; }
		public double Longitude { get; set; }
		public int[] MeetingTypeIds { get; set; }
		public int Page { get; set; }
		public int Radius { get; set; }
		public string SearchTerm { get; set; }
		public string WorkingOn { get; set; }
	}
}
