using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Styling;
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
        
        // Создание стиля для TreeViewItem
       
        
        
        var treeViewItemStyleCorrected = new Style(x => x.OfType<TreeViewItem>())
        {
            Setters = 
            {
                new Setter(TreeViewItem.PaddingProperty, new Thickness(0, 0, 0, 0)),
                new Setter(Control.MarginProperty, new Thickness(-12, 0, 0, 0)), // Без отрицательного отступа
                new Setter(TreeViewItem.FontSizeProperty, 15.0),
            }
        };
        
        
        HierarchyTree.Styles.Add(treeViewItemStyleCorrected);
        
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