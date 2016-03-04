using CLRProfiler.Attributes;
using GalaSoft.MvvmLight;
using System;

namespace CLRProfiler.Model
{
	[WeaveRaisePropertyChanged]
	public class ProcessInfoTab : ObservableObject
	{
		public Guid ID { get; private set; }
		public bool Closeable { get; set; }
		public string Name { get; set; }
		public ViewModelBase ViewModel { get; set; }

		public ProcessInfoTab() { ID = Guid.NewGuid(); }
	}
}