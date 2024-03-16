using NoteStash.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace NoteStash.Resources;

public class InputTemplateSelector : DataTemplateSelector
{
	public override DataTemplate SelectTemplate(object item, DependencyObject container)
	{
		FrameworkElement elem = container as FrameworkElement;
		object? resource = null;
		if (item is InputDialogViewModel vm)
		{
			if (vm.IsComboBoxMode) resource = elem.FindResource("ComboBoxTemplate");
			else
			{
				if (vm.IsGetFileName) resource = elem.FindResource("TextBoxTemplate_RejectInvalidFileName");
				else resource = elem.FindResource("TextBoxTemplate");
			}
		}
		return resource as DataTemplate;
	}
}
