using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLRProfiler.Messages
{
	public class OpenObjectMessage
	{
		public object Object { get; set; }
		public string Title { get; set; }

		public OpenObjectMessage(object theObject, string title)
		{
			this.Object = theObject;
			this.Title = title;
		}
	}
}
