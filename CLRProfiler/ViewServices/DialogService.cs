using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLRProfiler.ViewServices
{
	public class DialogService
	{
		public Model.ProcessItem ShowProcessList()
		{
			View.ProcessList pl = new View.ProcessList();
			ViewModel.ProcessListViewModel context = pl.DataContext as ViewModel.ProcessListViewModel;

			if (pl.ShowDialog() == true)
			{
				return context.SelectedProcess;
			}
			SimpleIoc.Default.Unregister<ViewModel.ProcessListViewModel>(context);
			return null;
		}
	}
}
