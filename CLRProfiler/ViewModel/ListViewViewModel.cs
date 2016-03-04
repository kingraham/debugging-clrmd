using System.Collections;
using System.Linq;

namespace CLRProfiler.ViewModel
{
	public class ListViewViewModel : HandleLinkFromViewModel
	{
		public IList Items { get; set; }

		public ListViewViewModel(IList obj)
		{
			Items = obj;
		}

		public ListViewViewModel(object[] obj)
		{
			Items = obj.ToList();
		}
	}
}