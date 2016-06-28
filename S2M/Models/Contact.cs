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

		public static async Task<ObservableCollection<Contact>> GetProfileContacts(CancellationToken token, ObservableCollection<Contact> contactlist)
		{
			var contacts = new ObservableCollection<Contact>();
			var contactResult = await GetContactsDataAsync(token);
			if (contactResult != null)
			{
				contacts = contactResult.Results;

				foreach(var contact in contacts)
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
					var url = apiUrl + "/api/profiles/contacts";

					using (var httpResponse = await httpClient.GetAsync(new Uri(url)))
					{
						string json = await httpResponse.Content.ReadAsStringAsync();
						json = json.Replace("<br>", Environment.NewLine);
						contactResult = JsonConvert.DeserializeObject<ContactResult>(json);
					}
				}
				catch (Exception e) { }
			}

			return contactResult;
		}
	}
}
