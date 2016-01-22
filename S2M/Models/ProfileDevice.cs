using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Web.Http;

namespace S2M.Models
{
	class ProfileDevice
	{
		public int Id { get; set; }
		public int ProfileId { get; set; }
		public string DeviceType { get; set; }
		public string DeviceKey { get; set; }
		public long TimeStamp { get; set; }

		public static async Task<ProfileDevice> RegisterProfileDevice(CancellationToken token)
		{
			var profileDevice = new ProfileDevice();
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
					var criteria = new SaveProfileDeviceCriteria
					{
						DeviceKey = Common.DeviceHelper.GetDeviceId(),
						DeviceType = "Windows"
					};

					var url = apiUrl + "/api/profiles/device";
					var queryString = new HttpStringContent(JsonConvert.SerializeObject(criteria), Windows.Storage.Streams.UnicodeEncoding.Utf8, "application/json");

					using (var httpResponse = await httpClient.PostAsync(new Uri(url), queryString))
					{
						string json = await httpResponse.Content.ReadAsStringAsync().AsTask(token);
						json = json.Replace("<br>", Environment.NewLine);
						profileDevice = JsonConvert.DeserializeObject<ProfileDevice>(json);
					}
				}
				catch (Exception e) { }
			}

			return profileDevice;
		}
	}

	public class SaveProfileDeviceCriteria
	{
		public string DeviceKey { get; set; }
		public string DeviceType { get; set; }
	}
}
