using CLRProfiler.Attributes;
using GalaSoft.MvvmLight.Command;
using Microsoft.Diagnostics.Runtime;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace CLRProfiler.ViewModel
{
	[WeaveRaisePropertyChanged]
	public class ThreadsListViewModel : HandleLinkFromViewModel
	{
		public ObservableCollection<ClrThread> Threads { get; set; }

		[AttachToPropertyView]
		[RaiseCanExecuteDependency(new string[] { "ShowStackObjects" })]
		public ClrThread SelectedItem { get; set; }

		public PropertyViewViewModel SelectedThread { get; set; }

		public ICommand ShowStackObjects { get; private set; }

		public ThreadsListViewModel(Model.CLRDataTarget dataTarget)
		{
			SelectedThread = new PropertyViewViewModel();
			Threads = new ObservableCollection<ClrThread>();
			foreach (var appdomain in dataTarget.ClrRuntime.Threads.OrderByDescending(t => t.IsAlive).ThenBy(t => t.ManagedThreadId))
				Threads.Add(appdomain);

			ShowStackObjects = new RelayCommand(() =>
			{
				bool is32bit = SelectedItem.Runtime.PointerSize == 4;
				ClrHeap heap = SelectedItem.Runtime.GetHeap();

				ulong start = SelectedItem.StackBase;
				ulong stop = SelectedItem.StackLimit;

				if (start > stop)
				{
					ulong tmp = start;
					start = stop;
					stop = tmp;
				}

				List<object> so = new List<object>();
				for (ulong ptr = start; ptr <= stop; ptr += (ulong)SelectedItem.Runtime.PointerSize)
				{
					ulong obj;
					if (!SelectedItem.Runtime.ReadPointer(ptr, out obj))
						break;

					try
					{
						if (is32bit && obj > int.MaxValue)
						{
							continue;
						}
						ClrType type = heap.GetObjectType(obj);
						if (type == null)
							continue;

						if (!type.IsFree)
							so.Add(new Model.StackObject(ptr, obj, type.Name, type));
					}
					catch { }
				}
				MessengerInstance.Send<Messages.OpenListMessage>(new Messages.OpenListMessage(so, "Stack Objects for: " + SelectedItem.ManagedThreadId));
			}, (() => SelectedItem != null));
		}
	}
}