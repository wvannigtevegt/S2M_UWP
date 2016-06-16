using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace S2M.Models {
	public class LocationText {
		public int Id { get; set; } = 0;
		public int LocationId { get; set; } = 0;
		public int LanguageId { get; set; } = 0;
		public string Description { get; set; } = "";
		public string UspTitle1 { get; set; } = "";
		public string UspDescription1 { get; set; } = "";
		public string UspTitle2 { get; set; } = "";
		public string UspDescription2 { get; set; } = "";
		public string UspTitle3 { get; set; } = "";
		public string UspDescription3 { get; set; } = "";
		public string UspTitle4 { get; set; } = "";
		public string UspDescription4 { get; set; } = "";

		public static async Task<LocationText> GetLocationDescriptionAsync(CancellationToken token, int locationId) {
			var locationTexts = await GetLocationTextDataAsync(token, locationId);
			if (locationTexts.Any()) {
				return locationTexts.Where(lt => lt.LanguageId == 65).First();
			}
			return null;
		}

		private static async Task<List<LocationText>> GetLocationTextDataAsync(CancellationToken token, int locationId, string languageCode = "en") {
			var locationTexts = new List<LocationText>();

			using (var httpClient = new Windows.Web.Http.HttpClient()) {
				var apiKey = Common.StorageService.LoadSetting("ApiKey");
				var apiUrl = Common.StorageService.LoadSetting("ApiUrl");

				httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip");
				httpClient.DefaultRequestHeaders.Add("token", apiKey);
				httpClient.DefaultRequestHeaders.Add("api-version", "2");

				try {
					var url = apiUrl + "/api/locations/text/" + locationId;

					var httpResponse = await httpClient.GetAsync(new Uri(url)).AsTask(token);
					string json = await httpResponse.Content.ReadAsStringAsync().AsTask(token);
					json = json.Replace("<br>", Environment.NewLine);
					locationTexts = JsonConvert.DeserializeObject<List<LocationText>>(json);
				}
				catch (Exception e) { }
			}

			return locationTexts;
		}
	}
}
