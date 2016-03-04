using CLRProfiler.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Diagnostics.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;

namespace CLRProfiler.ViewModel
{
	public class PropertyViewViewModel : HandleLinkFromViewModel
	{		
		public class Property
		{
			public string Name { get; set; }
			public object Value { get; set; }

			public Property(string name, object value)
			{
				Name = name; Value = value;
			}
		}

		public ObservableCollection<Property> Properties { get; set; }		

		public PropertyViewViewModel()
		{
			Initialize();			
		}

		public PropertyViewViewModel(object o)
			: base()
		{
			Initialize();			
			if (o is StackObject)
				SetObjectFromPointer(o);
			else
			{
				SetObject(o);
			}
		}		

		private void Initialize()
		{
			Properties = new ObservableCollection<Property>();
			HandleLinkCommand = new RelayCommand<ViewModel.PropertyViewViewModel.Property>(HandleLink);
			HandleArrayLinkCommand = new RelayCommand<ViewModel.PropertyViewViewModel.Property>(HandleArrayLink);
		}

		private void HandleLink(ViewModel.PropertyViewViewModel.Property prop)
		{
			MessengerInstance.Send<Messages.OpenObjectMessage>(new Messages.OpenObjectMessage(prop.Value, prop.Name + " (" + prop.Value.GetType().Name + ")" ));
		}

		private void HandleArrayLink(ViewModel.PropertyViewViewModel.Property prop)
		{
			if (prop.Value == null)
				return;
			if (prop.Value.GetType().IsArray)
			{
				try
				{
					MessengerInstance.Send<Messages.OpenArrayMessage>(new Messages.OpenArrayMessage((object[])prop.Value, prop.Name + " (" + prop.Value.GetType().Name + ")"));
				} catch (Exception ex)
				{
					Debugger.Break();
				}
			}
			else
			{
				MessengerInstance.Send<Messages.OpenListMessage>(new Messages.OpenListMessage((IList)prop.Value, prop.Name + " (" + prop.Value.GetType().Name + ")"));
			}
		}

		public void SetObjectFromPointer(object o)
		{
			Properties.Clear();

			ClrType clrType = null;
			ulong pointer = 0;

			if (o is StackObject)
			{
				clrType = ((StackObject)o).Type;
				pointer = ((StackObject)o).Pointer;
			}
			else
			{
				Debugger.Break();
			}			

			foreach (var field in clrType.Fields)
			{
				try
				{
					Properties.Add(new Property(field.Name, field.GetValue(pointer)));
				}
				catch (Exception ex)
				{
					Properties.Add(new Property(field.Name, "****" + ex.Message));
				}
			}
		}

		public void SetObject(object o)
		{
			Properties.Clear();
			if (o != null)
			{

				foreach (var p in o.GetType().GetProperties().OrderBy(p => p.Name))
				{
					try
					{
						Properties.Add(new Property(p.Name, p.GetValue(o)));
					}
					catch (Exception ex)
					{
						Properties.Add(new Property(p.Name, "!!" + ex.GetType().Name + "!!"));
					}
				}
			}		
		}
	}
}