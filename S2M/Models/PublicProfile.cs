using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S2M.Models
{
	public class PublicProfile
	{
		public int Id { get; set; }
		public string ProfileKey { get; set; }
		public string ProfileName { get; set; }
		public string ProfileImage { get; set; }
		public string TwitterAccount { get; set; }
		public string FacebookAccount { get; set; }
		public string LinkedInAccount { get; set; }
		public string GoogleAccount { get; set; }
		public int Score { get; set; }
		public string[] Tags { get; set; }
		public CheckIn CurrentCheckin { get; set; }
		public DateTime CreatedOn { get; set; }

		public static async Task<PublicProfile> GetProfileByProfileId(int profileId)
		{
			var profile = new PublicProfile();

			using (var httpClient = new Windows.Web.Http.HttpClient())
			{
				var apiKey = Common.StorageService.LoadSetting("ApiKey");
				var apiUrl = Common.StorageService.LoadSetting("ApiUrl");

				httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip");
				httpClient.DefaultRequestHeaders.Add("token", apiKey);
				httpClient.DefaultRequestHeaders.Add("api-version", "2");

				try
				{
					var url = apiUrl + "/api/profiles/" + profileId;

					var httpResponse = await httpClient.GetAsync(new Uri(url));
					string json = await httpResponse.Content.ReadAsStringAsync();
					json = json.Replace("<br>", Environment.NewLine);
					profile = JsonConvert.DeserializeObject<PublicProfile>(json);
				}
				catch (Exception e) { }
			}

			return profile;
		}
	}
}
