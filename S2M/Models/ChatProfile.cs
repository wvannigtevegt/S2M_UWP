namespace S2M.Models {
	public class ChatProfile {
		public int Id { get; set; }
		public int ChatId { get; set; }
		public int ProfileId { get; set; }
		public string ProfileName { get; set; }
		public string ProfileImage { get; set; }
		public string ProfileImage_84 {
			get {
				if (!string.IsNullOrEmpty(ProfileImage)) {
					var imageCdn = "https://d3817ykd1rv0p7.cloudfront.net";

					var filenameWithoutExtension = ProfileImage.Substring(0, ProfileImage.LastIndexOf("."));
					var imagePath = imageCdn + "/" + ProfileId.ToString() + "_" + filenameWithoutExtension + "_84x84.jpg";

					return imagePath;
				}
				return "";
			}
		}
	}
}
