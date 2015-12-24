using Newtonsoft.Json;
using S2M.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace S2M.Models {
	public class Reservation {
		public int Id { get; set; }
		public string ReservationName { get; set; }
		public int ChannelId { get; set; }
		public int LocationId { get; set; }
		public string LocationName { get; set; }
		//public int LanguageId { get; set; }
		//public int ProfileId { get; set; }
		//public int CompanyId { get; set; }
		public int StatusId { get; set; }
		public int MeetingTypeId { get; set; }
		//public int TenderId { get; set; }
		public int VoucherId { get; set; }
		public string VoucherName { get; set; }
		public int TotalSeats { get; set; }
		public DateTime StartTime { get; set; }
		public DateTime EndTime { get; set; }
		public int CurrencyId { get; set; }
		public decimal TotalExcl { get; set; }
		public decimal TotalIncl { get; set; }
		public decimal TotalPaid { get; set; }
		public decimal TotalOpen { get; set; }
		//public int HasDepositInvoice { get; set; }
		//public int HasInvoice { get; set; }
		public DateTime CreatedOn { get; set; }
		public DateTime EditedOn { get; set; }

		public static async Task GetReservationsAsync(CancellationToken token, ObservableCollection<Reservation> reservationList, string searchTerm = "", int locationId = 0, int profileId = 0, int page = 0, int itemsPerPage = 0) {
			var reservationResult = new ReservationResult();

			using (var httpClient = new Windows.Web.Http.HttpClient()) {
				var apiKey = StorageService.LoadSetting("ApiKey");
				var apiUrl = StorageService.LoadSetting("ApiUrl");
				var profileToken = StorageService.LoadSetting("ProfileToken");

				httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip");
				httpClient.DefaultRequestHeaders.Add("token", apiKey);
				httpClient.DefaultRequestHeaders.Add("api-version", "2");
				httpClient.DefaultRequestHeaders.Add("profileToken", profileToken);

				try {
					//var criteria = new ReservationListCriteria {
					//	ItemsPerPage = itemsPerPage,
					//	LocationId = locationId,
					//	MeetingTypeIds = new List<int>() { 2 },
					//	Page = page,
					//	ProfileId = profileId,
					//	SearchTerm = searchTerm,
					//	StartDate = DateTime.Now,
					//	StatusIds = new List<int>() { 2 }
					//};

					var url = apiUrl + "/api/reservation/profile";
					//if (locationId > 0) {

					//}
					//if (profileId > 0) {
					//	url = url + "/profile";
					//}
					//url = url + "?" + JsonConvert.SerializeObject(criteria);

					using (var httpResponse = await httpClient.GetAsync(new Uri(url)).AsTask(token)) {
						string json = await httpResponse.Content.ReadAsStringAsync().AsTask(token);
						json = json.Replace("<br>", Environment.NewLine);
						reservationResult = JsonConvert.DeserializeObject<ReservationResult>(json);
					}
				}
				catch (Exception) { }
			}

			foreach (var reservation in reservationResult.Results) {
				reservationList.Add(reservation);
			}
		}
	}

	public class ReservationListCriteria {
		public int ItemsPerPage { get; set; }
		public int LocationId { get; set; }
		public List<int> MeetingTypeIds { get; set; }
		public int Page { get; set; }
		public int ProfileId { get; set; }
		public string SearchTerm { get; set; }
		public DateTime StartDate { get; set; }
		public List<int> StatusIds { get; set; }
	}
}
