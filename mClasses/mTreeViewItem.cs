using System;
using Avalonia.Controls;
using Xamlade.LinkWorkers;
using Xamlade.mClasses;

namespace Xamlade.jClasses;

//Модифицированные элементы дерева со встроенными jControl
public class mTreeViewItem : TreeViewItem, MControl
{
    protected override Type StyleKeyOverride => typeof(TreeViewItem); 
   // public JControl element { get; set; }
   public Beholder Beholder { get; set; }
    public mTreeViewItem(JControl element)
    {
        this.Beholder = element.Beholder;
       // this.Name = $"{Beholder.element.Name}_mTree";
        Header = element.Name;
        //Обратная связь с jElement
        element.Beholder.mTreeItem = this;
    }
}