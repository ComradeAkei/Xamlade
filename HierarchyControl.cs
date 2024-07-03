using System.ComponentModel;
using Avalonia.Controls;

namespace Xamlade;

public static class HierarchyControl
{
    public static TreeView HierarchyTree { get; set; }
    public static void Init(TreeView Tree)
    {
        HierarchyTree = Tree;
    }
        
}