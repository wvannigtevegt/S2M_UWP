namespace S2M.Models {
	public class ApiResult {
		public int StatusCode { get; set; }
		public string Message { get; set; }
		public int MorePages { get; set; }
		public long TimeStamp { get; set; }
	}
}
