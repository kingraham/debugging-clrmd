using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace CLRProfiler.Behaviors
{
	public class PropertyDataTemplateSelector: DataTemplateSelector
	{
		public DataTemplate DefaultDataTemplate { get; set; }
		public DataTemplate LinkDataTemplate { get; set; }
		public DataTemplate ArrayDataTemplate { get; set; }

		public override DataTemplate SelectTemplate(object item, DependencyObject container)
		{
			if (!(container is UIElement) || item == null)
				return base.SelectTemplate(item, container);

			ViewModel.PropertyViewViewModel.Property property = item as ViewModel.PropertyViewViewModel.Property;

			if (property.Value != null)
			{				
				// all reference types that aren't string can be explored further.
				if (property.Value.GetType().IsArray && ArrayDataTemplate != null)
					return ArrayDataTemplate;

				if (property.Value.GetType().Name.Contains("List"))
					return ArrayDataTemplate;

				if (!property.Value.GetType().IsValueType && !property.Value.GetType().Name.Equals("String", StringComparison.InvariantCultureIgnoreCase) && LinkDataTemplate != null)
					return LinkDataTemplate;	
										
			}

			if (DefaultDataTemplate != null)
				return DefaultDataTemplate;

			return base.SelectTemplate(item, container);
		}		
	}	
}
