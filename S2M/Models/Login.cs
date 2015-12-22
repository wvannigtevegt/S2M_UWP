using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Security.Credentials;
using Windows.Web.Http;

namespace S2M.Models {
	public class Login {
		public static async Task<LoginResult> LoginUser(string username, string password) {
			var loginResult = new LoginResult();

			using (var httpClient = new HttpClient()) {
				var apiKey = Common.StorageService.LoadSetting("ApiKey");
				var apiUrl = Common.StorageService.LoadSetting("ApiUrl");

				try {
					httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip");
					httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
					httpClient.DefaultRequestHeaders.Add("api-version", "2");
					httpClient.DefaultRequestHeaders.Add("token", apiKey);

					Dictionary<string, string> criteria = new Dictionary<string, string>();

					var hardwareId = "1234567890";// GetDeviceID();

					criteria.Add("MachineToken", hardwareId);
					criteria.Add("UserName", username);
					criteria.Add("Password", password);

					var uri = new Uri(apiUrl + "/api/login");
					var queryString = new HttpStringContent(JsonConvert.SerializeObject(criteria), Windows.Storage.Streams.UnicodeEncoding.Utf8, "application/json");

					HttpResponseMessage response = await httpClient.PostAsync(uri, queryString);
					string json = await response.Content.ReadAsStringAsync();
					json = json.Replace("<br>", Environment.NewLine);
					loginResult = JsonConvert.DeserializeObject<LoginResult>(json);
				}
				catch (Exception e) {

				}
			}

			return loginResult;
		}

		public static async Task LogOff() {
			var vault = new PasswordVault();
			const string vaultResource = "S2M";

			var credentialList = vault.FindAllByResource(vaultResource);

			if (credentialList.Any()) {
				var credentials = credentialList.First();
				var username = credentials.UserName;
				var password = vault.Retrieve(vaultResource, username).Password;

				vault.Remove(new PasswordCredential(vaultResource, username, password));
			}

			await Common.StorageService.DeleteObjectAsync("Profile");
		}

		//private static string GetDeviceID() {

		//var token = Windows.System.Profile.HardwareIdentification.GetPackageSpecificToken(null);
		//var hardwareId = token.Id;
		//var dataReader = Windows.Storage.Streams.DataReader.FromBuffer(hardwareId);

		//byte[] bytes = new byte[hardwareId.Length];
		//dataReader.ReadBytes(bytes);

		//return BitConverter.ToString(bytes).Replace("-", "");
		//}
	}
}
