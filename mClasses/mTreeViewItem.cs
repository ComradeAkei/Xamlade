using System;
using Avalonia.Controls;

namespace Xamlade.jClasses;

//Модифицированные элементы дерева со встроенными jControl
public class mTreeViewItem : TreeViewItem, MControl
{
    protected override Type StyleKeyOverride => typeof(TreeViewItem); 
    public JControl element { get; set; }
    public mTreeViewItem(JControl element)
    {
        this.Name = $"{element.Name}_mTree";
        this.element = element;
        Header = element.Name;
        //Обратная связь с jElement
        element.mTreeItem = this;
    }
}