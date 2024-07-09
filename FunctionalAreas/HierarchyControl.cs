using Avalonia.Controls;
using Xamlade.jClasses;

namespace Xamlade.FunctionalAreas;

public static class HierarchyControl
{
    private static TreeView HierarchyTree { get; set; }
    /// <summary>
    /// Выбранный в дереве элемент
    /// </summary>
    public static mTreeViewItem? Selected
    {
        get => (mTreeViewItem?)HierarchyTree.SelectedItem;
        set => HierarchyTree.SelectedItem = value;
    }
    public static void Init(TreeView Tree)
    {
        HierarchyTree = Tree;
        HierarchyTree.SelectionChanged += MainHierarchyTree_OnSelectionChanged;
        
        HierarchyTree.Items.Add(new mTreeViewItem(Workspace.MainCanvas));
        Selected = Workspace.MainCanvas.mTreeItem;
    }
    
    private static void MainHierarchyTree_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (((TreeView)sender).SelectedItem is mTreeViewItem item)
            Workspace.SelectjElement(item.element);
        else
            Workspace.SelectjElement(Workspace.MainCanvas);
        e.Handled = true;
    }
}