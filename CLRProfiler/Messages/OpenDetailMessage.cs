
using System;
namespace CLRProfiler.Messages
{
	public class OpenDetailMessage
	{
		public OpenDetailMessage(Model.CLRDataTarget dataTarget, Type detailRequest, bool autoSelectFirst = false)
		{
			this.DataTarget = dataTarget;
			this.DetailsRequest = detailRequest;
			this.AutoSelectFirst = autoSelectFirst;
		}
		
		public Model.CLRDataTarget DataTarget { get; set; }
		public Type DetailsRequest { get; set; }
		public bool AutoSelectFirst { get; set; }
	}
}