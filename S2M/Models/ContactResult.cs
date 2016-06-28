using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S2M.Models
{
	class ContactResult : ApiResult
	{
		public ObservableCollection<Contact> Results { get; set; }
	}
}
