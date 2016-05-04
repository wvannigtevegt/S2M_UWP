using System;

namespace S2M.Common
{
	public static class DateService
	{
		private static readonly long DatetimeMinTimeTicks = (new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).Ticks;

		public static long ToJavaScriptMilliseconds(this DateTime dt)
		{
			return (long)((dt.ToUniversalTime().Ticks - DatetimeMinTimeTicks) / 10000);
		}

		/// <summary>
		/// Converts a Unix timestamp into a System.DateTime
		/// </summary>
		/// <param name="timestamp">The Unix timestamp in milliseconds to convert, as a double</param>
		/// <returns>DateTime obtained through conversion</returns>
		public static DateTime ConvertFromUnixTimestamp(double timestamp)
		{
			DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
			return origin.AddMilliseconds(timestamp); // convert from milliseconds to seconds
		}

		/// <summary>
		/// Gets the relative time.
		/// </summary>
		/// <param name="dateTime">The date time offset.</param>
		/// <returns>The relative time.</returns>
		public static string ToRelativeTime(DateTime dateTime)
		{
			var s = DateTime.Now.Subtract(dateTime);

			var dayDifference = (int)s.TotalDays;

			var secondDifference = (int)s.TotalSeconds;

			if (dayDifference < 0)
			{
				return null;
			}

			if (dayDifference == 0)
			{
				if (secondDifference < 60)
				{
					return "just now";
				}

				if (secondDifference < 120)
				{
					return "1 minute ago";
				}

				if (secondDifference < 3600)
				{
					return $"{secondDifference / 60} minutes ago";
				}

				if (secondDifference < 7200)
				{
					return "1 hour ago";
				}

				if (secondDifference < 86400)
				{
					return $"{secondDifference / 3600} hours ago";
				}
			}

			if (dayDifference == 1)
			{
				return "yesterday";
			}

			if (dayDifference < 7)
			{
				return $"{dayDifference} days ago";
			}

			if (dayDifference < 14)
			{
				return "1 week ago";
			}

			if (dayDifference < 31)
			{
				return $"{dayDifference / 7} weeks ago";
			}

			if (dayDifference < 62)
			{
				return "1 month ago";
			}

			return $"{dayDifference / 31} months ago";
		}
	}
}
