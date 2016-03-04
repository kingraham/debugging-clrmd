using CLRProfiler.Attributes;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Diagnostics.Runtime;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace CLRProfiler.ViewModel
{
	[WeaveRaisePropertyChanged]
	public class AppDomainListViewModel : ViewModelBase
	{
		public AppDomainListViewModel(Model.CLRDataTarget dataTarget, bool autoSelectFirst = false)
		{
			AppDomains = new ObservableCollection<ClrAppDomain>();
			foreach (var appdomain in dataTarget.ClrRuntime.AppDomains)
				AppDomains.Add(appdomain);

			SelectedAppDomain = new PropertyViewViewModel();

			if (autoSelectFirst)
				SelectedItem = AppDomains[0];

			OpenModuleCommand = new RelayCommand<ClrModule>(module =>
			{
				MessengerInstance.Send<Messages.OpenObjectMessage>(new Messages.OpenObjectMessage(module, "Module: " + module.AssemblyId));
			});
		}

		public ObservableCollection<ClrAppDomain> AppDomains { get; set; }

		public ICommand OpenModuleCommand { get; private set; }

		public PropertyViewViewModel SelectedAppDomain { get; set; }

		[AttachToPropertyView]
		public ClrAppDomain SelectedItem { get; set; }
	}
}