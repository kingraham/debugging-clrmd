using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLRProfiler.Messages
{
	public class OpenArrayMessage
	{
		public object[] Object { get; set; }
		public string Title { get; set; }

		public OpenArrayMessage(object[] theObject, string title)
		{
			this.Object = theObject;
			this.Title = title;
		}
	}
}
