using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Controls.Primitives;

namespace UpdatedUIApp.Resources
{
    //following code obtained from https://docs.microsoft.com/en-us/dotnet/framework/wpf/data/how-to-find-datatemplate-generated-elements and https://stackoverflow.com/questions/25229503/findvisualchild-reference-issue answer by Rohit Vas and Shocked
    //Also, thanks to BionicCode from StackOverflow for helping me out in this class (https://stackoverflow.com/questions/62113194/findvisualchild-get-properties-of-named-ui-elements-in-itemscontrol answer by BionicCode)
    public static class Utils
    {
      
    public static bool TryFindVisualChildByName<TChild>(
   this DependencyObject parent,
   string childElementName,
   out TChild childElement,
   bool isCaseSensitive = false)
   where TChild : FrameworkElement
        {
            childElement = null;

            // Popup.Child content is not part of the visual tree.
            // To prevent traversal from breaking when parent is a Popup,
            // we need to explicitly extract the content.
            if (parent is Popup popup)
            {
                parent = popup.Child;
            }

            if (parent == null)
            {
                return false;
            }

            var stringComparison = isCaseSensitive
              ? StringComparison.Ordinal
              : StringComparison.OrdinalIgnoreCase;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                if (child is TChild resultElement
                  && resultElement.Name.Equals(childElementName, stringComparison))
                {
                    childElement = resultElement;
                    return true;
                }

                if (child.TryFindVisualChildByName(childElementName, out childElement))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
