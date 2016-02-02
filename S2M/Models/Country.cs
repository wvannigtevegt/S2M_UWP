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
	public class Country
	{
		public int Id { get; set; }
		public string CountryCode { get; set; }
		public string Name { get; set; }
		public int LanguageId { get; set; }
		public string Language { get; set; }
		public string Culture { get; set; }
		public string LaguageCulture { get; set; }
		public int CurrencyId { get; set; }
		public string CurrencyCode { get; set; }
		public string CurrencyName { get; set; }
		public string PhoneCode { get; set; }
		public int ContinentId { get; set; }

		public static async Task GetActiveCountries(CancellationToken token, ObservableCollection<Country> countryList)
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
					var url = apiUrl + "/api/countries/active";

					using (var httpResponse = await httpClient.GetAsync(new Uri(url)).AsTask(token))
					{
						string json = await httpResponse.Content.ReadAsStringAsync().AsTask(token);
						json = json.Replace("<br>", Environment.NewLine);
						var results = JsonConvert.DeserializeObject<List<Country>>(json);

						foreach(var result in results)
						{
							countryList.Add(result);
						}
					}
				}
				catch (Exception) { }
			}
		}
	}
}
