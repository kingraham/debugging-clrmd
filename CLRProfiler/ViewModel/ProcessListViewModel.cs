using CLRProfiler.Attributes;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace CLRProfiler.ViewModel
{
	[WeaveRaisePropertyChanged]
	public class ProcessListViewModel : ViewModelBase
	{
		public ObservableCollection<Model.ProcessItem> ProcessItemList { get; private set; }

		public ProcessListViewModel()
		{			
			OKCommand = new RelayCommand(() =>
			{
				ProcessSelected = true;
			}, () => SelectedProcess != null);

			RefreshCommand = new RelayCommand(Refresh);
			Refresh();
		}

		public ICommand CancelCommand { get; private set; }
		public ICommand OKCommand { get; private set; }
		public ICommand RefreshCommand { get; private set; }
		public bool? ProcessSelected { get; set; }

		[RaiseCanExecuteDependency(new string[] { "OKCommand" })]
		public Model.ProcessItem SelectedProcess { get; set; }

		private void Refresh()
		{
			ProcessItemList = new ObservableCollection<Model.ProcessItem>();

			foreach (var process in System.Diagnostics.Process.GetProcesses().OrderBy(p => p.ProcessName))
			{
				ProcessItemList.Add(new Model.ProcessItem(process));
			}
		}
	}
}