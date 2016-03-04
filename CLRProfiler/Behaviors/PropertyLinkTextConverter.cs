using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace CLRProfiler.Behaviors
{
	public class PropertyLinkTextConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value is ViewModel.PropertyViewViewModel.Property)
			{
				if (((ViewModel.PropertyViewViewModel.Property)value).Value is Microsoft.Diagnostics.Runtime.PdbInfo)
					return "PDB Info";
				else if (((ViewModel.PropertyViewViewModel.Property)value).Value is Microsoft.Diagnostics.Runtime.ClrRuntime)
					return "Clr Runtime";				
			}			
			return "View";
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return null;
		}
	}
}
