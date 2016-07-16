using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using Windows.Web.Http;

namespace S2M.Models
{
	public class Contact
	{
		public int Id { get; set; }
		public int ProfileId { get; set; }
		public string ProfileKey { get; set; }
		public string ProfileName { get; set; }
		public string ProfileImage { get; set; }
		public string TwitterAccount { get; set; }
		public string FacebookAccount { get; set; }
		public string LinkedInAccount { get; set; }
		public string GoogleAccount { get; set; }
		public string KnowledgeTags { get; set; }
		public string Tags { get; set; }
		public CheckIn CurrentCheckin { get; set; }
		public DateTime CreatedOn { get; set; }
		public string ProfileImage_84
		{
			get
			{
				if (!string.IsNullOrEmpty(ProfileImage))
				{
					var imageCdn = "https://d3817ykd1rv0p7.cloudfront.net";

					var filenameWithoutExtension = ProfileImage.Substring(0, ProfileImage.LastIndexOf("."));
					var imagePath = imageCdn + "/" + ProfileId.ToString() + "_" + filenameWithoutExtension + "_84x84.jpg";

					return imagePath;
				}
				return "ms-appx:///Assets/defaultProfileImage_84x84.png";
			}
		}
		public string ProfileImage_150
		{
			get
			{
				if (!string.IsNullOrEmpty(ProfileImage))
				{
					var imageCdn = "https://d3817ykd1rv0p7.cloudfront.net";

					var filenameWithoutExtension = ProfileImage.Substring(0, ProfileImage.LastIndexOf("."));
					var imagePath = imageCdn + "/" + ProfileId.ToString() + "_" + filenameWithoutExtension + "_150x150.jpg";
					return imagePath;
				}
				return "ms-appx:///Assets/defaultProfileImage_84x84.png";
			}
		}

		public static async Task<Contact> SaveContact(int profileId, string tags)
		{
			var contact = new Contact();

			using (var httpClient = new HttpClient())
			{
				var apiKey = Common.StorageService.LoadSetting("ApiKey");
				var apiUrl = Common.StorageService.LoadSetting("ApiUrl");
				var profileToken = Common.StorageService.LoadSetting("ProfileToken");

				httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip");
				httpClient.DefaultRequestHeaders.Add("token", apiKey);
				httpClient.DefaultRequestHeaders.Add("api-version", "2");
				httpClient.DefaultRequestHeaders.Add("profileToken", profileToken);

				var criteria = new SaveProfileContactCriteria
				{
					ProfileId = profileId,
					Tags = tags
				};

				var uri = new Uri(apiUrl + "/api/contact");
				var queryString = new HttpStringContent(JsonConvert.SerializeObject(criteria), Windows.Storage.Streams.UnicodeEncoding.Utf8, "application/json");

				HttpResponseMessage response = await httpClient.PostAsync(uri, queryString);
				string json = await response.Content.ReadAsStringAsync();
				json = json.Replace("<br>", Environment.NewLine);
				contact = JsonConvert.DeserializeObject<Contact>(json);
			}

			return contact;
		}

		public static async Task DeleteContact(int profileId)
		{
			using (var httpClient = new HttpClient())
			{
				var apiKey = Common.StorageService.LoadSetting("ApiKey");
				var apiUrl = Common.StorageService.LoadSetting("ApiUrl");
				var profileToken = Common.StorageService.LoadSetting("ProfileToken");

				httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip");
				httpClient.DefaultRequestHeaders.Add("token", apiKey);
				httpClient.DefaultRequestHeaders.Add("api-version", "2");
				httpClient.DefaultRequestHeaders.Add("profileToken", profileToken);

				var uri = new Uri(apiUrl + "/api/contact/" + profileId);

				HttpResponseMessage response = await httpClient.DeleteAsync(uri);
				string json = await response.Content.ReadAsStringAsync();
				//json = json.Replace("<br>", Environment.NewLine);
				//contact = JsonConvert.DeserializeObject<Contact>(json);
			}
		}

		public static async Task<Contact> GetContectByProfileId(CancellationToken token, int profileId)
		{
			var contact = new Contact();

			using (var httpClient = new HttpClient())
			{
				var apiKey = Common.StorageService.LoadSetting("ApiKey");
				var apiUrl = Common.StorageService.LoadSetting("ApiUrl");
				var profileToken = Common.StorageService.LoadSetting("ProfileToken");

				httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip");
				httpClient.DefaultRequestHeaders.Add("token", apiKey);
				httpClient.DefaultRequestHeaders.Add("profileToken", profileToken);

				try
				{
					var url = apiUrl + "/api/contact/" + profileId;

					using (var httpResponse = await httpClient.GetAsync(new Uri(url)))
					{
						string json = await httpResponse.Content.ReadAsStringAsync();
						json = json.Replace("<br>", Environment.NewLine);
						contact = JsonConvert.DeserializeObject<Contact>(json);
					}
				}
				catch (Exception) { }
			}

			return contact;
		}

		public static async Task<ObservableCollection<Contact>> GetProfileContacts(CancellationToken token, ObservableCollection<Contact> contactlist)
		{
			var contacts = new ObservableCollection<Contact>();
			var contactResult = await GetContactsDataAsync(token);
			if (contactResult != null)
			{
				contacts = contactResult.Results;

				foreach (var contact in contacts)
				{
					contactlist.Add(contact);
				}
			}

			return contacts;
		}

		private static async Task<ContactResult> GetContactsDataAsync(CancellationToken token, string searchTerm = "")
		{
			var contactResult = new ContactResult();

			using (var httpClient = new Windows.Web.Http.HttpClient())
			{
				var apiKey = Common.StorageService.LoadSetting("ApiKey");
				var apiUrl = Common.StorageService.LoadSetting("ApiUrl");
				var profileToken = Common.StorageService.LoadSetting("ProfileToken");

				httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip");
				httpClient.DefaultRequestHeaders.Add("token", apiKey);
				httpClient.DefaultRequestHeaders.Add("profileToken", profileToken);

				try
				{
					var url = apiUrl + "/api/contact";

					using (var httpResponse = await httpClient.GetAsync(new Uri(url)))
					{
						string json = await httpResponse.Content.ReadAsStringAsync();
						json = json.Replace("<br>", Environment.NewLine);
						contactResult = JsonConvert.DeserializeObject<ContactResult>(json);
					}
				}
				catch (Exception) { }
			}

			return contactResult;
		}
	}

	public class SaveProfileContactCriteria
	{
		public int ProfileId { get; set; } = 0;
		public string Tags { get; set; } = "";
	}
}
