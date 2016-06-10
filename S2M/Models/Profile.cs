using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using Windows.Web.Http;

namespace S2M.Models {
	public class Profile {
		public int Id { get; set; }
		public string Key { get; set; }
		public string Image { get; set; }
		public string ProfileImage_84 {
			get {
				if (!string.IsNullOrEmpty(Image)) {
					var imageCdn = "https://d3817ykd1rv0p7.cloudfront.net";

					var filenameWithoutExtension = Image.Substring(0, Image.LastIndexOf("."));
					var imagePath = imageCdn + "/" + Id.ToString() + "_" + filenameWithoutExtension + "_84x84.jpg";

					return imagePath;
				}
				return "";
			}
		}
		public string ProfileImage_150 {
			get {
				if (!string.IsNullOrEmpty(Image)) {
					var imageCdn = "https://d3817ykd1rv0p7.cloudfront.net";

					var filenameWithoutExtension = Image.Substring(0, Image.LastIndexOf("."));
					var imagePath = imageCdn + "/" + Id.ToString() + "_" + filenameWithoutExtension + "_150x150.jpg";

					return imagePath;
				}
				return "";
			}
		}
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string FullName { get; set; }
		public string Address { get; set; }
		public string Postalcode { get; set; }
		public string City { get; set; }
		public string Email { get; set; }
		public string Tags { get; set; }
		public bool Locked { get; set; }
		public bool Expired { get; set; }
		public bool Enabled { get; set; }
		public DateTime CreatedOn { get; set; }
		public string LastToken { get; set; }

		public static async Task<Profile> RegisterNewProfile(string email, string password, string firstName, string lastName, string tags)
		{
			using (var httpClient = new HttpClient())
			{
				var apiKey = Common.StorageService.LoadSetting("ApiKey");
				var apiUrl = Common.StorageService.LoadSetting("ApiUrl");

				httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip");
				httpClient.DefaultRequestHeaders.Add("token", apiKey);
				httpClient.DefaultRequestHeaders.Add("api-version", "2");

				var criteria = new RegisterProfileCriteria
				{
					Email = email,
					Password = password,
					FirstName = firstName,
					LastName = lastName,
					Tags = tags
				};

				var uri = new Uri(apiUrl + "/api/profiles/register");
				var queryString = new HttpStringContent(JsonConvert.SerializeObject(criteria), Windows.Storage.Streams.UnicodeEncoding.Utf8, "application/json");

				HttpResponseMessage response = await httpClient.PostAsync(uri, queryString);
				string json = await response.Content.ReadAsStringAsync();
				json = json.Replace("<br>", Environment.NewLine);
				var profile = JsonConvert.DeserializeObject<Profile>(json);

				return profile;
			}
		}

		public static async Task<Profile> GetProfile() {
			var profile = new Profile();

			using (var httpClient = new Windows.Web.Http.HttpClient()) {
				var apiKey = Common.StorageService.LoadSetting("ApiKey");
				var apiUrl = Common.StorageService.LoadSetting("ApiUrl");
				var profileToken = Common.StorageService.LoadSetting("ProfileToken");

				httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip");
				httpClient.DefaultRequestHeaders.Add("token", apiKey);
				httpClient.DefaultRequestHeaders.Add("api-version", "2");
				httpClient.DefaultRequestHeaders.Add("profileToken", profileToken);

				try {
					var url = apiUrl + "/api/profiles";

					var httpResponse = await httpClient.GetAsync(new Uri(url));
					string json = await httpResponse.Content.ReadAsStringAsync();
					json = json.Replace("<br>", Environment.NewLine);
					profile = JsonConvert.DeserializeObject<Profile>(json);
				}
				catch (Exception e) { }
			}

			return profile;
		}

		public static async Task<Profile> UpdateProfileAsync(Profile profile) {
			var authenticatedProfile = await Common.StorageService.RetrieveObjectAsync<Profile>("Profile");

			using (var httpClient = new HttpClient()) {
				var apiKey = Common.StorageService.LoadSetting("ApiKey");
				var apiUrl = Common.StorageService.LoadSetting("ApiUrl");
				var profileToken = Common.StorageService.LoadSetting("ProfileToken");

				httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip");
				httpClient.DefaultRequestHeaders.Add("token", apiKey);
				httpClient.DefaultRequestHeaders.Add("api-version", "2");
				httpClient.DefaultRequestHeaders.Add("profileToken", profileToken);

				var criteria = new UpdateProfileCriteria {
					Profile = profile
				};

				var uri = new Uri(apiUrl + "/api/profiles");
				var queryString = new HttpStringContent(JsonConvert.SerializeObject(criteria), Windows.Storage.Streams.UnicodeEncoding.Utf8, "application/json");

				HttpResponseMessage response = await httpClient.PostAsync(uri, queryString);
				string json = await response.Content.ReadAsStringAsync();
				json = json.Replace("<br>", Environment.NewLine);
				profile = JsonConvert.DeserializeObject<Profile>(json);

				if (profile != null) {
					await Common.StorageService.PersistObjectAsync("Profile", profile);
				}
			}

			return profile;
		}
	}

	public class RegisterProfileCriteria
	{
		public string Email { get; set; }
		public string Password { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Tags { get; set; }
	}

	public class UpdateProfileCriteria {
		public Profile Profile { get; set; }
	}
}
