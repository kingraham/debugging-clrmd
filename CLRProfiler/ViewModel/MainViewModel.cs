using CLRProfiler.Attributes;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Practices.ServiceLocation;
using System.Windows.Input;

namespace CLRProfiler.ViewModel
{
	[WeaveRaisePropertyChanged]
	public class MainViewModel : ViewModelBase
	{
		private ViewServices.DialogService _DialogService = ServiceLocator.Current.GetInstance<ViewServices.DialogService>();
		
		public MainViewModel()
		{
			AttachToProcessCommand = new RelayCommand(() =>
			{
				Model.ProcessItem process = _DialogService.ShowProcessList();
				if (process != null)
				{
					IsAttachedToProcess = true;
					SelectedProcessView = new ViewModel.ProcessViewViewModel(process);
				}
			});

			DetachFromProcessCommand = new RelayCommand(() =>
			{
				IsAttachedToProcess = false;
				SelectedProcessView.Cleanup();
				SelectedProcessView = null;
			});
		}

		#region Commands

		public ICommand AttachToProcessCommand { get; private set; }
		public ICommand DetachFromProcessCommand { get; private set; }

		#endregion Commands

		public bool IsAttachedToProcess { get; private set; }

		[RaiseCanExecuteDependency(new string[] { "AttachToProcessCommand", "DetachFromProcessCommand" })]
		public ViewModelBase SelectedProcessView { get; set; }
	}
}