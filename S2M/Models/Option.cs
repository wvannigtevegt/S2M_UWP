﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using Windows.Web.Http;

namespace S2M.Models
{
	public class Option
	{
		public int OptionId { get; set; }
		public string OptionImage { get; set; }
		public string OptionName { get; set; }
		public string OptionDescription { get; set; }
		public int LocationId { get; set; }
		public int CategoryId { get; set; }
		public bool IsPP { get; set; }
		public bool IsPublic { get; set; }
		public bool IsSystem { get; set; }
		public int RequiredItem { get; set; }
		public bool PricePerHour { get; set; }
		public decimal MaxPP { get; set; }
		public decimal MaxTotal { get; set; }
		public int MaxAmount { get; set; }
		public bool IsSelected { get; set; }
		public int Amount { get; set; }
		public int TaxId { get; set; }
		public int CurrencyId { get; set; }
		public string CurrencySymbol { get; set; }
		public decimal TaxPercentage { get; set; }
		public decimal BasePrice { get; set; }
		public decimal PricePerItem { get; set; }
		public long Crc { get; set; }
		public bool IsEnabled
		{
			get
			{
				if (RequiredItem == 2)
				{
					return false;
				}
				return true;
			}
		}

		public static async Task GetLocationOptionsAsync(CancellationToken token, int reservationId, ObservableCollection<Option> optionList)
		{
			using (var httpClient = new Windows.Web.Http.HttpClient())
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
					var url = apiUrl + "/api/reservation/wizard/options/" + reservationId;

					using (var httpResponse = await httpClient.GetAsync(new Uri(url)))
					{
						string json = await httpResponse.Content.ReadAsStringAsync();
						json = json.Replace("<br>", Environment.NewLine);
						var results = JsonConvert.DeserializeObject<List<Option>>(json);

						foreach (var result in results)
						{
							optionList.Add(result);
						}
					}
				}
				catch (Exception) { }
			}
		}

		public static async Task<Reservation> SaveOptionToReservation(CancellationToken token, int reservationId, Option option)
		{
			var reservation = new Reservation();

			using (var httpClient = new HttpClient())
			{
				var apiKey = Common.StorageService.LoadSetting("ApiKey");
				var apiUrl = Common.StorageService.LoadSetting("ApiUrl");
				var profileToken = Common.StorageService.LoadSetting("ProfileToken");

				httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip");
				httpClient.DefaultRequestHeaders.Add("token", apiKey);
				httpClient.DefaultRequestHeaders.Add("api-version", "2");
				httpClient.DefaultRequestHeaders.Add("profileToken", profileToken);

				var criteria = new SaveOptionCriteria
				{
					ReservationId = reservationId,
					OptionId = option.OptionId,
					Amount = option.Amount,
					CurrencyId = option.CurrencyId,
					PricePerItem = option.PricePerItem,
					PriceTotal = option.PricePerItem,
					TaxId = option.TaxId,
					Crc = option.Crc
				};

				var uri = new Uri(apiUrl + "/api/reservation/option");
				var queryString = new HttpStringContent(JsonConvert.SerializeObject(criteria), Windows.Storage.Streams.UnicodeEncoding.Utf8, "application/json");

				using (var httpResponse = await httpClient.PostAsync(uri, queryString))
				{
					string json = await httpResponse.Content.ReadAsStringAsync().AsTask(token);
					json = json.Replace("<br>", Environment.NewLine);
					reservation = JsonConvert.DeserializeObject<Reservation>(json);
				}
			}
			return reservation;
		}

		public static async Task<int> DeleteOptionFromReservation(CancellationToken token, int reservationId, int optionId)
		{
			var result = 0;

			using (var httpClient = new HttpClient())
			{
				var apiKey = Common.StorageService.LoadSetting("ApiKey");
				var apiUrl = Common.StorageService.LoadSetting("ApiUrl");
				var profileToken = Common.StorageService.LoadSetting("ProfileToken");

				httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip");
				httpClient.DefaultRequestHeaders.Add("token", apiKey);
				httpClient.DefaultRequestHeaders.Add("api-version", "2");
				httpClient.DefaultRequestHeaders.Add("profileToken", profileToken);

				var uri = new Uri(apiUrl + "/api/reservation/" + reservationId + "/option/" + optionId);

				using (var httpResponse = await httpClient.DeleteAsync(uri))
				{
					string json = await httpResponse.Content.ReadAsStringAsync().AsTask(token);
					json = json.Replace("<br>", Environment.NewLine);
					result = JsonConvert.DeserializeObject<int>(json);
				}
			}
			return result;
		}
	}

	public class SaveOptionCriteria
	{
		public int ReservationId { get; set; }
		public int OptionId { get; set; }
		public int Amount { get; set; }
		public int CurrencyId { get; set; }
		public decimal PricePerItem { get; set; }
		public decimal PriceTotal { get; set; }
		public int TaxId { get; set; }
		public long Crc { get; set; }
	}
}