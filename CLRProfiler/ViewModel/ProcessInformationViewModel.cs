using CLRProfiler.Attributes;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Diagnostics.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CLRProfiler.ViewModel
{
	[WeaveRaisePropertyChanged]
	public class ProcessInformationViewModel : ViewModelBase
	{
		public ICommand OpenAppDomainsCommand { get; private set; }
		public ICommand OpenFirstAppDomainCommand { get; private set; }
		public ICommand OpenThreadsCommand { get; private set; }

		public string BaseAppDomain { get; set; }
		public string CLRVersion { get; set; }
		public int AppDomainCount { get; set; }
		public int ModulesCount { get; set; }
		public int ThreadCount { get; set; }

		private Model.CLRDataTarget _dataTarget;

		// do not hold reference to the data target
		public ProcessInformationViewModel(Model.CLRDataTarget dataTarget)
		{
			BaseAppDomain = dataTarget.ClrRuntime.AppDomains[0].Name;
			CLRVersion = dataTarget.BaseDataTarget.ClrVersions[0].Version.ToString();
			AppDomainCount = dataTarget.ClrRuntime.AppDomains.Count;			
			ModulesCount = dataTarget.ClrRuntime.Modules.Count;
			ThreadCount = dataTarget.ClrRuntime.Threads.Count;

			_dataTarget = dataTarget;

			OpenAppDomainsCommand = new RelayCommand(() =>
			{
				MessengerInstance.Send<Messages.OpenDetailMessage>(new Messages.OpenDetailMessage(this._dataTarget, typeof(ClrAppDomain)));
			});

			OpenFirstAppDomainCommand = new RelayCommand(() =>
			{
				MessengerInstance.Send<Messages.OpenDetailMessage>(new Messages.OpenDetailMessage(this._dataTarget, typeof(ClrAppDomain), true));
			});

			OpenThreadsCommand = new RelayCommand(() =>
			{
				MessengerInstance.Send<Messages.OpenDetailMessage>(new Messages.OpenDetailMessage(this._dataTarget, typeof(ClrThread)));
			});
		}
	}
}
