using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Windows.Web.Http;

namespace S2M.Models {
	public class Availability {
		public string SearchKey { get; set; }
		public List<AvailableLocation> Locations { get; set; }

		public static async Task<Availability> GetAvailableLocations(CancellationToken token, int locationId, DateTime date, TimeSpan startTime, TimeSpan endTime) {
			var availability = new Availability();

			var authenticatedProfile = await Common.StorageService.RetrieveObjectAsync<Profile>("Profile");
			var searchObject = BuildSearchObject(locationId, date, startTime, endTime);

			using (var httpClient = new HttpClient()) {
				try {
					var apiKey = Common.StorageService.LoadSetting("ApiKey");
					var apiUrl = Common.StorageService.LoadSetting("ApiUrl");

					httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip");
					httpClient.DefaultRequestHeaders.Add("token", apiKey);
					httpClient.DefaultRequestHeaders.Add("api-version", "2");

					var criteria = new AvailabilityCriteria {
						ApiKey = int.Parse(apiKey),
						ProfileKey = authenticatedProfile.Key,
						SearchObject = searchObject
					};

					var url = apiUrl + "/api/availability/search";
					var queryString = new HttpStringContent(JsonConvert.SerializeObject(criteria), Windows.Storage.Streams.UnicodeEncoding.Utf8, "application/json");

					using (var httpResponse = await httpClient.PostAsync(new Uri(url), queryString)) {
						string json =  await httpResponse.Content.ReadAsStringAsync().AsTask(token);
						json = json.Replace("<br>", Environment.NewLine);
						availability = JsonConvert.DeserializeObject<Availability>(json);
					}
				}
				catch (Exception) {
				}
			}

			return availability;
		}

		private static Search BuildSearchObject(int locationId, DateTime date, TimeSpan startTime, TimeSpan endTime) {
			var channelId = int.Parse(Common.StorageService.LoadSetting("ChannelId"));
			var countryId = int.Parse(Common.StorageService.LoadSetting("CountryId"));

			var searchObject = new Search {
				SearchKey = "",
				ChannelId = channelId,
				CountryId = countryId,
				LanguageId = 52,
				VoucherId = 0,
				DealId = 0,
				TagId = 0,
				WidgetType = -1,
				SearchTerm = "",
				SearchType = 2,
				SearchLatitude = 0,
				SearchLongitude = 0,
				SearchRadius = 0,
				SearchLocations = locationId.ToString(),
				ServerSession = "",
				SortSearchOn = 0,
				CreatedOn = DateTime.Now,
				LastStep = 1,
				SearchDates = new List<SearchDate>()
			};

			var searchDateObject = new SearchDate {
				Id = 0,
				SearchId = 0,
				Date = date,
				StartTimeSpan = startTime,
				EndTimeSpan = endTime,
				Seats = 1,
				SettingId = 0,
				MeetingTypeId = 2
			};

			searchObject.SearchDates.Add(searchDateObject);

			return searchObject;
		}

		public static async Task<Reservation> SelectAvailableLocation(CancellationToken token, string searchKey, int locationId, int searchDateId, int unitId, int settingId) {
			var reservation = new Reservation();

			var chosenUnits = new List<ChosenUnit>();
			var chosenUnit = new ChosenUnit {
				SearchDateId = searchDateId,
				LocationId = locationId,
				UnitId = unitId,
				SettingId = settingId
			};
			chosenUnits.Add(chosenUnit);

			using (var httpClient = new HttpClient()) {
				var apiKey = Common.StorageService.LoadSetting("ApiKey");
				var apiUrl = Common.StorageService.LoadSetting("ApiUrl");
				var profileToken = Common.StorageService.LoadSetting("ProfileToken");

				httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip");
				httpClient.DefaultRequestHeaders.Add("token", apiKey);
				httpClient.DefaultRequestHeaders.Add("api-version", "2");
				httpClient.DefaultRequestHeaders.Add("profileToken", profileToken);

				var criteria = new ChosenLocationCriteria {
					SearchKey = searchKey,
					LocationId = locationId,
					SearchDateId = searchDateId,
					UnitId = unitId,
					SettingId = settingId
				};

				try {
					var url = apiUrl + "/api/availability/choselocation/finalize";
					var queryString = new HttpStringContent(JsonConvert.SerializeObject(criteria), Windows.Storage.Streams.UnicodeEncoding.Utf8, "application/json");

					using (var httpResponse = await httpClient.PostAsync(new Uri(url), queryString)) {
						string json = await httpResponse.Content.ReadAsStringAsync().AsTask(token);
						json = json.Replace("<br>", Environment.NewLine);
						reservation = JsonConvert.DeserializeObject<Reservation>(json);
					}
				}
				catch (Exception) { }			
			}

			return reservation;
		}

		public class AvailabilityCriteria {
			public int ApiKey { get; set; }
			public string ProfileKey { get; set; }
			public Search SearchObject { get; set; }
		}

		public class ChosenLocationCriteria {
			public int LocationId { get; set; } = 0;
			public string SearchKey { get; set; } = "";
			public int SearchDateId { get; set; } = 0;
			public int UnitId { get; set; } = 0;
			public int SettingId { get; set; } = 0;
		}
	}
}
