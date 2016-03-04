using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CLRProfiler.ViewModel
{
	public class HandleLinkFromViewModel : ViewModelBase
	{
		public ICommand HandleLinkCommand { get; protected set; }
		public ICommand HandleArrayLinkCommand { get; protected set; }

		public HandleLinkFromViewModel()
		{
			if (this.HandleLinkCommand == null)
			{
				this.HandleLinkCommand = new RelayCommand<object>(o => MessengerInstance.Send<Messages.OpenObjectMessage>(new Messages.OpenObjectMessage(o, o.GetType().Name)));
			}

			if (this.HandleArrayLinkCommand == null)
			{
				this.HandleArrayLinkCommand = new RelayCommand<List<object>>(o => {
					MessengerInstance.Send<Messages.OpenListMessage>(new Messages.OpenListMessage(o, o.GetType().Name));
				});
			}
		}
	}
}
