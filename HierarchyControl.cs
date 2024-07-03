using System.ComponentModel;
using Avalonia.Controls;

namespace Xamlade;

public static class HierarchyControl
{
    public static TreeView HierarchyTree { get; set; }
    //Выбранный в дереве элемент
    public static mTreeViewItem? selectedTreeItem;
    public static void Init(TreeView Tree)
    {
        HierarchyTree = Tree;
    }
        
}