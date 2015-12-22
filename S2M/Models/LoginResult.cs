namespace S2M.Models {
	public class LoginResult {
		public string ProfileKey { get; set; }
		public string ProfileToken { get; set; }
		public int StatusCode { get; set; }
		public string Message { get; set; }
		public int MorePages { get; set; }
		public long TimeStamp { get; set; }
	}
}
