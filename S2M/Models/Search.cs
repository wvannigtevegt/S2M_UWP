using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S2M.Models {
	public class Search {
		public int Id { get; set; }
		public string SearchKey { get; set; }
		public int ChannelId { get; set; }
		public int CountryId { get; set; }
		public int LanguageId { get; set; }
		public int VoucherId { get; set; }
		public int DealId { get; set; }
		public int TagId { get; set; }
		public int WidgetType { get; set; }
		public string SearchTerm { get; set; }
		public int SearchType { get; set; }
		public double SearchLatitude { get; set; }
		public double SearchLongitude { get; set; }
		public int SearchRadius { get; set; }
		public string SearchLocations { get; set; }
		public string ServerSession { get; set; }
		public int SortSearchOn { get; set; }
		public DateTime CreatedOn { get; set; }
		public int LastStep { get; set; }
		public List<SearchDate> SearchDates { get; set; }
	}
}
