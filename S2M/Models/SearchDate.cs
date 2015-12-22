using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S2M.Models {
	public class SearchDate {
		public int Id { get; set; }
		public int SearchId { get; set; }
		public DateTime Date { get; set; }
		public TimeSpan StartTimeSpan { get; set; }
		public TimeSpan EndTimeSpan { get; set; }
		public int Seats { get; set; }
		public int SettingId { get; set; }
		public int MeetingTypeId { get; set; }
		public long StartTimeStamp
		{
			get
			{
				return Common.DateService.ToJavaScriptMilliseconds(Date.Date + StartTimeSpan);
			}
		}
		public DateTime StartTime
		{
			get
			{
				var startDate = Date.Date + StartTimeSpan;
				return DateTime.SpecifyKind(startDate, DateTimeKind.Utc);
			}
		}
		public long EndTimeStamp
		{
			get
			{
				return Common.DateService.ToJavaScriptMilliseconds(Date.Date + EndTimeSpan);
			}
		}
		public DateTime EndTime
		{
			get
			{
				var endDate = Date.Date + EndTimeSpan;
				return DateTime.SpecifyKind(endDate, DateTimeKind.Utc);
			}
		}
	}
}
