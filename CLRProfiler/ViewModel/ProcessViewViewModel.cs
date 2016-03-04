using CLRProfiler.Attributes;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Diagnostics.Runtime;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace CLRProfiler.ViewModel
{
	[WeaveRaisePropertyChanged]
	public class ProcessViewViewModel : ViewModelBase
	{
		public ObservableCollection<Model.ProcessInfoTab> Tabs { get; set; }
		public Model.ProcessInfoTab SelectedTab { get; set; }

		private Model.CLRDataTarget _dataTarget;

		public ICommand CloseTabCommand { get; private set; }

		public ProcessViewViewModel(Model.ProcessItem pi)
		{
			_dataTarget = new Model.CLRDataTarget(pi);

			Tabs = new ObservableCollection<Model.ProcessInfoTab>();

			AddTab(new Model.ProcessInfoTab()
			{
				Name = "Information",
				Closeable = false,
				ViewModel = new ViewModel.ProcessInformationViewModel(_dataTarget)
			});

			SelectedTab = Tabs[0];

			MessengerInstance.Register<Messages.OpenDetailMessage>(this, HandleOpenDetailMessage);
			MessengerInstance.Register<Messages.OpenObjectMessage>(this, HandleOpenObjectMessage);
			MessengerInstance.Register<Messages.OpenListMessage>(this, HandleOpenListMessage);
			MessengerInstance.Register<Messages.OpenArrayMessage>(this, HandleOpenArrayMessage);

			CloseTabCommand = new RelayCommand<Model.ProcessInfoTab>(pit => Tabs.Remove(pit));
		}

		public void HandleOpenDetailMessage(Messages.OpenDetailMessage openMessage)
		{
			if (openMessage.DetailsRequest == typeof(ClrAppDomain))
			{
				AddTab(new Model.ProcessInfoTab()
				{
					Name = "App Domains",
					Closeable = true,
					ViewModel = new ViewModel.AppDomainListViewModel(_dataTarget, openMessage.AutoSelectFirst)
				});
			}
			else if (openMessage.DetailsRequest == typeof(ClrThread))
			{
				AddTab(new Model.ProcessInfoTab()
				{
					Name = "Threads",
					Closeable = true,
					ViewModel = new ViewModel.ThreadsListViewModel(_dataTarget)
				});
			}
		}

		public void HandleOpenObjectMessage(Messages.OpenObjectMessage openMessage)
		{
			AddTab(new Model.ProcessInfoTab()
			{
				Name = openMessage.Title,
				Closeable = true,
				ViewModel = new ViewModel.PropertyViewViewModel(openMessage.Object)
			});			
		}

		public void HandleOpenListMessage(Messages.OpenListMessage openMessage)
		{
			AddTab(new Model.ProcessInfoTab()
			{
				Name = openMessage.Title,
				Closeable = true,
				ViewModel = new ViewModel.ListViewViewModel(openMessage.Object)
			});
		}

		public void HandleOpenArrayMessage(Messages.OpenArrayMessage openMessage)
		{
			AddTab(new Model.ProcessInfoTab()
			{
				Name = openMessage.Title,
				Closeable = true,
				ViewModel = new ViewModel.ListViewViewModel(openMessage.Object)
			});
		}

		private void AddTab(Model.ProcessInfoTab tab)
		{
			Tabs.Add(tab);
			SelectedTab = Tabs[Tabs.Count - 1];
		}

		public override void Cleanup()
		{
			foreach (var tab in Tabs)
			{
				tab.ViewModel.Cleanup();
			}
			_dataTarget.Dispose();
			base.Cleanup();
		}
	}
}