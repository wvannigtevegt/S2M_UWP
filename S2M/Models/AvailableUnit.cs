using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S2M.Models {
	public class AvailableUnit {
		public int Id { get; set; }
		public int SearchDateId { get; set; }
		public int LocationId { get; set; }
		public int UnitId { get; set; }
		public int SeatTypeId { get; set; }
		public string Name { get; set; }
		public string Image { get; set; }
		public int M2 { get; set; }
		public int Seats { get; set; }
		public DateTime StartTime { get; set; }
		public DateTime EndTime { get; set; }
		public int SettingId { get; set; }
		public int CurrencyId { get; set; }
		public string CurrencySymbol { get; set; }
		public decimal Price { get; set; }
		public decimal PriceTotalExclTax { get; set; }
		public int TaxId { get; set; }
		public decimal TaxPercentage { get; set; }
		public long Crc { get; set; }
	}
}
