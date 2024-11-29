using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Avalonia.Controls;
using Xamlade.jClasses;
using Newtonsoft.Json.Linq;
using Xamlade.FunctionalAreas;
using Xamlade.ProgramWindow;

namespace Xamlade.Extensions;

public static class JCopy
{
    public static JControl Copy(JControl original, JChildContainer newParent)
    {
        var parent = original.jParent;
        var elementType = Type.GetType("Xamlade.jClasses.j" + original.Type);
        var element = ElementGenerator.GenerateElement(elementType, parent);
        element.xPropertiesGroup = new Dictionary<string, Dictionary<string, Property>>(original.xPropertiesGroup);
        element.SpecialSetDelegates =
            new Dictionary<string, JChildContainer.ContainerSetPropertyDelegate>(original.SpecialSetDelegates);

        Reflector.SetName($"{original.Name}_copy", element);
        element.Beholder.mTreeItem = new mTreeViewItem(element);
        (newParent as JControl).Beholder.mTreeItem.Items.Add(element.Beholder.mTreeItem);
        Workspace.movable = element;
        foreach (var KVP in element.xPropertiesGroup["main"])
        {
            if (KVP.Key != "Name")
                PropertiesControl.SetPropertyValue(KVP.Key, KVP.Value.Value, element, null);
        }

        foreach (var KVP in element.xPropertiesGroup["container"])
            PropertiesControl.SetPropertyValue(KVP.Key, KVP.Value.Value, element, null);
        newParent.AddChild(element);

        if (original is not JChildContainer container) return element;
        foreach (var cp_child in container.jChildren)
            Copy(cp_child, (JChildContainer)element);
        if (original.Name != "SelectionCanvas")
            return element;
        else
        {
            var p_parent = element.jParent;
            var SelectedList = new List<JControl>((element as JChildContainer).jChildren);

            // Найти границы (минимальные и максимальные координаты) всех элементов в SelectedList
            double minX = double.MaxValue, minY = double.MaxValue;
            double maxX = double.MinValue, maxY = double.MinValue;

            for (var index = 0; index < SelectedList.Count; index++)
            {
                var obj = SelectedList[index];
                var bounds = obj.Bounds;
                minX = Math.Min(minX, bounds.X);
                minY = Math.Min(minY, bounds.Y);
                maxX = Math.Max(maxX, bounds.X + bounds.Width);
                maxY = Math.Max(maxY, bounds.Y + bounds.Height);
            }

            foreach (var obj in SelectedList)
            {
                var absX = Canvas.GetLeft(obj as Control);
                var absY = Canvas.GetTop(obj as Control);
                (element as JChildContainer).RemoveChild(obj);
                p_parent.AddChild(obj);
                element.Beholder.mTreeItem.Items.Remove(obj.Beholder.mTreeItem);
                (p_parent as JControl).Beholder.mTreeItem.Items.Add(obj.Beholder.mTreeItem);
                Canvas.SetLeft(obj as Control, absX - minX);
                Canvas.SetTop(obj as Control, absY - minY);
            }

            (p_parent as JControl).Beholder.mTreeItem.Items.Remove(element.Beholder.mTreeItem);
            
            (p_parent as Control).UpdateLayout();
            foreach (var jControl in SelectedList)
                (jControl as Control).UpdateLayout();
            
            Workspace.RestoreSelectionCanvas();
            Workspace.SelectedList.Clear();
            foreach (var jControl in SelectedList)
            {
                Workspace.SelectedList.Add(jControl);
                Workspace.BindSelectionBorder(jControl);
            }


            Workspace.ApplySelectionCanvas();
            Workspace.InitMovable(Workspace.SelectionCanvas);

            
            
       //     Workspace.RestoreSelectionCanvas();


          /*  Workspace.movable = element;
            HierarchyControl.Selected.Beholder.element = element;
            Workspace.RemoveSelectedjElement();
            element.Dispose();
*/
            return Workspace.SelectionCanvas;
        }
    }

    public static void CopySelection(JChildContainer selectionCanvas, JChildContainer newParent)
    {
        foreach (var cp_child in selectionCanvas.jChildren)
            Copy(cp_child, (JChildContainer)newParent);
    }
}