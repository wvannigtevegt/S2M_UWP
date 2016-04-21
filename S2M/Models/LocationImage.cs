using Newtonsoft.Json;
using S2M.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace S2M.Models
{
	public class LocationImage
	{
		public int Id { get; set; }
		public int LocationId { get; set; }
		public string Image { get; set; }
		public bool IsDefault { get; set; }
		public DateTime CreatedOn { get; set; }
		public int CreatedBy { get; set; }
		public string Image_320 {
			get {
				if (!string.IsNullOrEmpty(Image)) {
					var azureCdn = "https://az691754.vo.msecnd.net";
					var azureContainer = "website";

					var filenameWithoutExtension = Image.Substring(0, Image.LastIndexOf("."));
					var imagePath = azureCdn + "/" + azureContainer + "/" + LocationId.ToString() + "/320x240_" + filenameWithoutExtension + ".jpg";

					return imagePath;
				}
				return "Assets/StoreLogo.png";
			}
		}

		public static async Task<ObservableCollection<LocationImage>> GetLocationImagesById(CancellationToken token, int locationId)
		{
			var locationImages = new ObservableCollection<LocationImage>();

			using (var httpClient = new Windows.Web.Http.HttpClient())
			{
				var apiKey = StorageService.LoadSetting("ApiKey");
				var apiUrl = StorageService.LoadSetting("ApiUrl");

				httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip");
				httpClient.DefaultRequestHeaders.Add("token", apiKey);
				httpClient.DefaultRequestHeaders.Add("api-version", "2");

				try
				{
					var url = apiUrl + "/api/locations/images/" + locationId;

					using (var httpResponse = await httpClient.GetAsync(new Uri(url)).AsTask(token))
					{
						string json = await httpResponse.Content.ReadAsStringAsync().AsTask(token);
						json = json.Replace("<br>", Environment.NewLine);

						var results = JsonConvert.DeserializeObject<List<LocationImage>>(json);

						foreach(var result in results)
						{
							locationImages.Add(result);
						}
					}
				}
				catch (Exception) { }
			}

			return locationImages;
		}
	}
}
