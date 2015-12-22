using Newtonsoft.Json;
using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.Web.Http;

namespace S2M.Models {
	public class Cart {
		public int Id { get; set; }
		public string CartKey { get; set; }
		public string Name { get; set; }
		public int SearchId { get; set; }
		public int ChannelId { get; set; }
		public int LocationId { get; set; }
		public int ProfileId { get; set; }
		public int CompanyId { get; set; }
		public int MeetingTypeId { get; set; }
		public int StatusId { get; set; }
		public int LanguageId { get; set; }
		public int CurrencyId { get; set; }
		public int MinutesToExpire { get; set; }
		public DateTime CreatedOn { get; set; }
		public DateTime OfferExpiresOn { get; set; }
		public string Tags { get; set; }
		public string WorkingOn { get; set; }
		//public List<CartUnit> Units { get; set; }
		//public List<CartOption> Options { get; set; }

		public static async Task<Cart> SetCarUnit(CancellationToken token, string cartKey, int searchDateId, int unitId, int currencyId, double pricePerItem, int taxId, long crc) {
			var cart = new Cart();

			using (var httpClient = new HttpClient()) {
				var apiKey = Common.StorageService.LoadSetting("ApiKey");
				var apiUrl = Common.StorageService.LoadSetting("ApiUrl");
				var profileToken = Common.StorageService.LoadSetting("ProfileToken");

				httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip");
				httpClient.DefaultRequestHeaders.Add("token", apiKey);
				httpClient.DefaultRequestHeaders.Add("api-version", "2");
				httpClient.DefaultRequestHeaders.Add("profileToken", profileToken);

				var criteria = new CartUnitCriteria() {
					CartKey = cartKey,
					SearchDateId = searchDateId,
					UnitId = unitId,
					CurrencyId = currencyId,
					PricePerItem = pricePerItem,
					TaxId = taxId,
					Crc = crc
				};

				var url = apiUrl + "/api/cart/unit/";
				var queryString = new HttpStringContent(JsonConvert.SerializeObject(criteria), Windows.Storage.Streams.UnicodeEncoding.Utf8, "application/json");

				using (var httpResponse = await httpClient.PostAsync(new Uri(url), queryString).AsTask(token)) {
					string json = await httpResponse.Content.ReadAsStringAsync().AsTask(token);
					json = json.Replace("<br>", Environment.NewLine);
					cart = JsonConvert.DeserializeObject<Cart>(json);
				}
			}

			return cart;
		}

		public async Task<Reservation> FinalizeCart(CancellationToken token) {
			var reservation = new Reservation();

			using (var httpClient = new HttpClient()) {
				var apiKey = Common.StorageService.LoadSetting("ApiKey");
				var apiUrl = Common.StorageService.LoadSetting("ApiUrl");
				var profileToken = Common.StorageService.LoadSetting("ProfileToken");

				httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip");
				httpClient.DefaultRequestHeaders.Add("token", apiKey);
				httpClient.DefaultRequestHeaders.Add("api-version", "2");
				httpClient.DefaultRequestHeaders.Add("profileToken", profileToken);

				var url = apiUrl + "/api/cart/finalize/" + this.CartKey;

				using (var httpResponse = await httpClient.GetAsync(new Uri(url)).AsTask(token)) {
					string json = await httpResponse.Content.ReadAsStringAsync().AsTask(token);
					json = json.Replace("<br>", Environment.NewLine);
					reservation = JsonConvert.DeserializeObject<Reservation>(json);
				}
			}

			return reservation;
		}

		public static async void DeleteCart(string searchKey) {
			using (var httpClient = new HttpClient()) {
				var apiKey = Common.StorageService.LoadSetting("ApiKey");
				var apiUrl = Common.StorageService.LoadSetting("ApiUrl");

				httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

				var url = apiUrl + "/api/cart/clearsearch/" + searchKey;

				using (var httpResponse = await httpClient.GetAsync(new Uri(url))) {

				}
			}
		}
	}

	public class CartUnitCriteria {
		public string CartKey { get; set; }
		public int SearchDateId { get; set; }
		public int UnitId { get; set; }
		public int CurrencyId { get; set; }
		public double PricePerItem { get; set; }
		public int TaxId { get; set; }
		public long Crc { get; set; }
	}
}

