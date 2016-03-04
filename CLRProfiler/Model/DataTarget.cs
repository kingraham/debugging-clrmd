using CLRProfiler.Attributes;
using GalaSoft.MvvmLight;
using Microsoft.Diagnostics.Runtime;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLRProfiler.Model
{
	[WeaveRaisePropertyChanged]
	public class CLRDataTarget : ObservableObject, IDisposable
	{
		public DataTarget BaseDataTarget { get; set; }
		public ClrRuntime ClrRuntime { get; set; }


		public CLRDataTarget(Model.ProcessItem pi)
		{
			BaseDataTarget = Microsoft.Diagnostics.Runtime.DataTarget.AttachToProcess(pi.ID, 3000, Microsoft.Diagnostics.Runtime.AttachFlag.Passive);
			ClrRuntime = CreateRuntime(BaseDataTarget);		
		}

		public void Dispose()
		{
			BaseDataTarget.Dispose();
		}

		private ClrRuntime CreateRuntime(DataTarget dataTarget)
		{
			// Now check bitness of our program/target:
			bool isTarget64Bit = dataTarget.PointerSize == 8;
			if (Environment.Is64BitProcess != isTarget64Bit)
				throw new Exception(string.Format("Architecture mismatch:  Process is {0} but target is {1}", Environment.Is64BitProcess ? "64 bit" : "32 bit", isTarget64Bit ? "64 bit" : "32 bit"));

			if (dataTarget.ClrVersions.Count == 0)
				throw new InvalidOperationException("Target Data Target has no associated Clr Version.");

			// does not handle SxS scenarios.
			ClrInfo version = dataTarget.ClrVersions[0];
			return version.CreateRuntime();
		}
	}
}
