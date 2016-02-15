using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace S2M.Models
{
	public class OpeningHour
	{
		public int NrOfLocations { get; set; }
		public DateTime MinTimeOpen { get; set; }
		public long MinTimeOpenTimeStamp { get; set; }
		public DateTime MaxTimeClose { get; set; }
		public long MaxTimeCloseTimeStamp { get; set; }

		public static async Task<OpeningHour> GetLocationOpeningHourssAsync(CancellationToken token, int locationId, DateTime date)
		{
			var openingHour = new OpeningHour();

			using (var httpClient = new Windows.Web.Http.HttpClient())
			{
				var apiKey = Common.StorageService.LoadSetting("ApiKey");
				var apiUrl = Common.StorageService.LoadSetting("ApiUrl");

				httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip");
				httpClient.DefaultRequestHeaders.Add("token", apiKey);
				httpClient.DefaultRequestHeaders.Add("api-version", "2");

				try
				{
					var url = apiUrl + "/api/openinghours/workspace/location/" + locationId + "/" + date.Year + "/" + date.Month + "/" + date.Day;

					using (var httpResponse = await httpClient.GetAsync(new Uri(url)))
					{
						string json = await httpResponse.Content.ReadAsStringAsync();
						json = json.Replace("<br>", Environment.NewLine);
						openingHour = JsonConvert.DeserializeObject<OpeningHour>(json);
					}
				}
				catch (Exception) { }

				return openingHour;
			}
		}
	}
}
