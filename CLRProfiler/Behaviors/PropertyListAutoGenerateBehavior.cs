using Microsoft.Diagnostics.Runtime;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Interactivity;
using System.Windows.Markup;

namespace CLRProfiler.Behaviors
{
	public class PropertyListAutoGenerateBehavior : Behavior<DataGrid>
	{
		protected override void OnAttached()
		{
			AssociatedObject.AutoGeneratingColumn +=
				new EventHandler<DataGridAutoGeneratingColumnEventArgs>(OnAutoGeneratingColumn);
		}

		private void OnAutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
		{
			if (e.PropertyType.IsAssignableFrom(typeof(ClrMethod)) ||
				e.PropertyType.IsAssignableFrom(typeof(ClrType)) || 
				e.PropertyName == "Pointer")
			{
				var templateColumn = new DataGridTemplateColumn();
				templateColumn.Header = e.PropertyName;
				templateColumn.SortMemberPath = e.PropertyName;
				templateColumn.CellTemplate = CreateTemplate(e.PropertyName);
				e.Column = templateColumn;
			}
		}

		DataTemplate CreateTemplate(String caption)
		{			
			string xamlTemplate = "<DataTemplate><TextBlock><Hyperlink Command=\"{Binding DataContext.HandleLinkCommand, RelativeSource={RelativeSource AncestorType={x:Type UserControl}}}\"" +
											" CommandParameter=\"{Binding " + caption + "}\"><InlineUIContainer><TextBlock Text=\"{Binding " + caption + "}\" /></InlineUIContainer></Hyperlink></TextBlock></DataTemplate>";
		
			if (caption == "Pointer")
			{
				xamlTemplate = "<DataTemplate><TextBlock><Hyperlink Command=\"{Binding DataContext.HandleLinkCommand, RelativeSource={RelativeSource AncestorType={x:Type UserControl}}}\"" +
											" CommandParameter=\"{Binding}\"><InlineUIContainer><TextBlock Text=\"{Binding " + caption + "}\" /></InlineUIContainer></Hyperlink></TextBlock></DataTemplate>";
			}		

			var context = new ParserContext();

			context.XamlTypeMapper = new XamlTypeMapper(new string[0]);			
		context.XmlnsDictionary.Add("", "http://schemas.microsoft.com/winfx/2006/xaml/presentation");
			context.XmlnsDictionary.Add("x", "http://schemas.microsoft.com/winfx/2006/xaml");
			

			var template = (DataTemplate)XamlReader.Parse(xamlTemplate, context);
			return template;
		}

		protected override void OnDetaching()
		{
			AssociatedObject.AutoGeneratingColumn -=
				new EventHandler<DataGridAutoGeneratingColumnEventArgs>(OnAutoGeneratingColumn);
		}
	}
}