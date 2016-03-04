using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLRProfiler.Messages
{
	public class OpenListMessage
	{
		public IList Object { get; set; }
		public string Title { get; set; }

		public OpenListMessage(IList theObject, string title)
		{
			this.Object = theObject;
			this.Title = title;
		}
	}
}
