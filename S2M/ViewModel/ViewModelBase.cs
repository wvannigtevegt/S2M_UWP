using S2M.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S2M.ViewModel
{
	/// <summary>
	/// A base class for ViewModels.
	/// </summary>
	public abstract class ViewModelBase : ObservableObjectBase
	{
		/// <summary>
		/// Loads the state.
		/// </summary>
		public virtual Task LoadState()
		{
			return Task.CompletedTask;
		}
	}
}
