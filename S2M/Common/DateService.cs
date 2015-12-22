using System;

namespace S2M.Common {
	public static class DateService {
		private static readonly long DatetimeMinTimeTicks = (new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).Ticks;

		public static long ToJavaScriptMilliseconds(this DateTime dt) {
			return (long)((dt.ToUniversalTime().Ticks - DatetimeMinTimeTicks) / 10000);
		}

		/// <summary>
		/// Converts a Unix timestamp into a System.DateTime
		/// </summary>
		/// <param name="timestamp">The Unix timestamp in milliseconds to convert, as a double</param>
		/// <returns>DateTime obtained through conversion</returns>
		public static DateTime ConvertFromUnixTimestamp(double timestamp) {
			DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
			return origin.AddMilliseconds(timestamp); // convert from milliseconds to seconds
		}
	}
}
