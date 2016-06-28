using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace S2M.Models
{
	public class EventCalendar
	{
		public int Id { get; set; }
		public DateTime Date { get; set; }
		public string Description { get; set; }
		public int EventId { get; set; }	
		public long EndTimeStamp { get; set; }
		public int LocationId { get; set; }
		public string LocationImage { get; set; }
		public string LocationName { get; set; }
		public string LocationAddress { get; set; }
		public string LocationPostalcode { get; set; }
		public string LocationCity { get; set; }
		public string LocationState { get; set; }
		public string LocationPhone { get; set; }
		public string LocationEmail { get; set; }
		public double LocationLatitude { get; set; }
		public double LocationLongitude { get; set; }
		public double LocationDistance { get; set; }
		public string LocationImage_160
		{
			get
			{
				if (!string.IsNullOrEmpty(LocationImage))
				{
					var azureCdn = "https://az691754.vo.msecnd.net";
					var azureContainer = "website";

					var filenameWithoutExtension = LocationImage.Substring(0, LocationImage.LastIndexOf("."));
					var imagePath = azureCdn + "/" + azureContainer + "/" + LocationId.ToString() + "/160x120_" + filenameWithoutExtension + ".jpg";

					return imagePath;
				}
				return "";
			}
		}
		public string LocationImage_320
		{
			get
			{
				if (!string.IsNullOrEmpty(LocationImage))
				{
					var azureCdn = "https://az691754.vo.msecnd.net";
					var azureContainer = "website";

					var filenameWithoutExtension = LocationImage.Substring(0, LocationImage.LastIndexOf("."));
					var imagePath = azureCdn + "/" + azureContainer + "/" + LocationId.ToString() + "/320x240_" + filenameWithoutExtension + ".jpg";

					return imagePath;
				}
				return "";
			}
		}
		public decimal MatchPercentage { get; set; }
		public string Name { get; set; }
		public int NrOfCheckIns { get; set; }
		public long StartTimeStamp { get; set; }
		public string Tags { get; set; }
		public string Url { get; set; }

		public static async Task<EventCalendar> GetEventCalendarById(CancellationToken token, int eventDateId)
		{
			var eventObject = new EventCalendar();

			using (var httpClient = new Windows.Web.Http.HttpClient())
			{
				var apiKey = Common.StorageService.LoadSetting("ApiKey");
				var apiUrl = Common.StorageService.LoadSetting("ApiUrl");

				httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip");
				httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
				httpClient.DefaultRequestHeaders.Add("token", apiKey);
				httpClient.DefaultRequestHeaders.Add("api-version", "2");

				try
				{
					var url = apiUrl + "/api/event/calendar/" + eventDateId;

					using (var httpResponse = await httpClient.GetAsync(new Uri(url)).AsTask(token))
					{
						string json = await httpResponse.Content.ReadAsStringAsync().AsTask(token);
						json = json.Replace("<br>", Environment.NewLine);
						eventObject = JsonConvert.DeserializeObject<EventCalendar>(json);
					}

				}
				catch (Exception e) { }
			}

			return eventObject;
		}


		public static async Task GetEventsAsync(CancellationToken token, ObservableCollection<EventCalendar> eventList, DateTime date, int locationId = 0, string searchTerm = "")
		{
			var eventObjs = await GetEventsDataAsync(date);

			foreach (var eventObj in eventObjs)
			{
				eventList.Add(eventObj);
			}
		}

		private static async Task<ObservableCollection<EventCalendar>> GetEventsDataAsync(DateTime date, int locationId = 0, string searchTerm = "", double latitude = 0, double longitude = 0, int radius = 0, string workingOn = "", int page = 0, int itemsPerPage = 0)
		{
			var events = new ObservableCollection<EventCalendar>();

			using (var httpClient = new Windows.Web.Http.HttpClient())
			{
				var apiKey = Common.StorageService.LoadSetting("ApiKey");
				var apiUrl = Common.StorageService.LoadSetting("ApiUrl");

				httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip");
				httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
				httpClient.DefaultRequestHeaders.Add("token", apiKey);

				try
				{
					var criteria = new EventListCriteria
					{
						SearchTerm = searchTerm,
						Latitude = latitude,
						Longitude = longitude,
						Radius = radius,
						Page = page,
						ItemsPerPage = itemsPerPage
					};

					var url = apiUrl + "/api/event/calendar";
					if (locationId > 0)
					{
						url = url + "/location/" + locationId;
					}
					if (date.Year > 1900)
					{
						url = url + "/" + date.Year + "/" + date.Month + "/" + date.Day;
					}
					url = url + "?" + JsonConvert.SerializeObject(criteria);

					var httpResponse = await httpClient.GetAsync(new Uri(url));
					string json = await httpResponse.Content.ReadAsStringAsync();
					json = json.Replace("<br>", Environment.NewLine);
					events = JsonConvert.DeserializeObject<ObservableCollection<EventCalendar>>(json);
				}
				catch (Exception e) { }
			}
			return events;
		}

	}

	public class EventListCriteria
	{
		public int ItemsPerPage { get; set; }
		public double Latitude { get; set; }
		public double Longitude { get; set; }
		public int Page { get; set; }
		public int Radius { get; set; }
		public string SearchTerm { get; set; }
	}
}
