using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.Web.Http;

namespace S2M.Models
{
	public class Channel
	{
		public int Id { get; set; }
		public string Name { get; set; }
		//public string Template { get; set; }
		//public string Url { get; set; }
		//public string EmailTemplate { get; set; }

		public static async Task GetActiveChannels(CancellationToken token, ObservableCollection<Channel> channelList)
		{

			using (var httpClient = new HttpClient())
			{
				var apiKey = Common.StorageService.LoadSetting("ApiKey");
				var apiUrl = Common.StorageService.LoadSetting("ApiUrl");

				httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip");
				httpClient.DefaultRequestHeaders.Add("token", apiKey);
				httpClient.DefaultRequestHeaders.Add("api-version", "2");

				try
				{
					var url = apiUrl + "/api/channels/active";

					using (var httpResponse = await httpClient.GetAsync(new Uri(url)).AsTask(token))
					{
						string json = await httpResponse.Content.ReadAsStringAsync().AsTask(token);
						json = json.Replace("<br>", Environment.NewLine);
						var results = JsonConvert.DeserializeObject<List<Channel>>(json);

						foreach (var result in results)
						{
							channelList.Add(result);
						}
					}
				}
				catch (Exception) { }
			}
		}
	}
}
