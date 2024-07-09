using System;
using Avalonia.Controls;

namespace Xamlade.jClasses;

public interface MControl
{
    
}

//Модифицированные элементы дерева со встроенными jControl
public class mTreeViewItem : TreeViewItem, MControl
{
    protected override Type StyleKeyOverride => typeof(TreeViewItem); 
    public JControl element { get; set; }
    public mTreeViewItem(JControl element)
    {
        this.element = element;
        Header = element.Name;
        //Обратная связь с jElement
        element.mTreeItem = this;
    }
}