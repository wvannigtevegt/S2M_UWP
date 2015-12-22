using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S2M.Models {
	public class AvailableLocation {
		public int SearchId { get; set; }
		public int MeetingTypeId { get; set; }
		public int LocationId { get; set; }
		public string OpenTime { get; set; }
		public string CloseTime { get; set; }
		public List<AvailableUnit> Units { get; set; }
	}
}
