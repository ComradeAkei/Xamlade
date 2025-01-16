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
        var element = CreateElement(original, newParent);

        if (original is JChildContainer container)
            CopyChildren(container, (JChildContainer)element);

        if (original.Name == "SelectionCanvas")
            return HandleSelectionCanvas((JChildContainer)element);

        return element;
    }

    private static JControl CreateElement(JControl source, JChildContainer parent)
    {
        var elementType = Type.GetType("Xamlade.jClasses.j" + source.Type);
        var element = ElementGenerator.GenerateElement(elementType, source.jParent);

        // Копирование свойств
        element.xPropertiesGroup = CloneProperties(source.xPropertiesGroup);
        element.SpecialSetDelegates = CloneDelegates(source.SpecialSetDelegates);

        Reflector.SetName($"{source.Name}_copy", element);
        InitializeBeholder(element, parent);
        Workspace.movable = element;

        SetPropertiesFromGroups(element, "main");
        SetPropertiesFromGroups(element, "container");

        parent.AddChild(element);
        return element;
    }

    private static void CopyChildren(JChildContainer source, JChildContainer target)
    {
        foreach (var child in source.jChildren)
            Copy(child, target);
    }

    private static JControl HandleSelectionCanvas(JChildContainer element)
    {
        var parent = element.jParent;
        var selectedList = new List<JControl>(element.jChildren);

        var bounds = CalculateBounds(selectedList);
        RelocateChildren(selectedList, element, parent, bounds);

        // Обновление интерфейса
        CleanupAndRestoreSelectionCanvas(element, parent, selectedList);
        return Workspace.SelectionCanvas;
    }

    private static Dictionary<string, Dictionary<string, Property>> CloneProperties(
        Dictionary<string, Dictionary<string, Property>> source)
    {
        return source.ToDictionary(
            group => group.Key,
            group => group.Value.ToDictionary(kvp => kvp.Key, kvp => kvp.Value));
    }

    private static Dictionary<string, JChildContainer.ContainerSetPropertyDelegate> CloneDelegates(
        Dictionary<string, JChildContainer.ContainerSetPropertyDelegate> source)
    {
        return new Dictionary<string, JChildContainer.ContainerSetPropertyDelegate>(source);
    }

    private static void InitializeBeholder(JControl element, JChildContainer parent)
    {
        element.Beholder.mTreeItem = new mTreeViewItem(element);
        (parent as JControl).Beholder.mTreeItem.Items.Add(element.Beholder.mTreeItem);
    }

    private static void SetPropertiesFromGroups(JControl element, string groupName)
    {
        if (!element.xPropertiesGroup.ContainsKey(groupName)) return;

        foreach (var kvp in element.xPropertiesGroup[groupName])
        {
            if (kvp.Key != "Name")
                PropertiesControl.SetPropertyValue(kvp.Key, kvp.Value.Value, element, null);
        }
    }

    private static (double MinX, double MinY, double MaxX, double MaxY) CalculateBounds(IEnumerable<JControl> controls)
    {
        double minX = double.MaxValue, minY = double.MaxValue;
        double maxX = double.MinValue, maxY = double.MinValue;

        foreach (var obj in controls)
        {
            var bounds = obj.Bounds;
            minX = Math.Min(minX, bounds.X);
            minY = Math.Min(minY, bounds.Y);
            maxX = Math.Max(maxX, bounds.X + bounds.Width);
            maxY = Math.Max(maxY, bounds.Y + bounds.Height);
        }

        return (minX, minY, maxX, maxY);
    }

    private static void RelocateChildren(IEnumerable<JControl> children, JChildContainer source, JChildContainer target,
        (double MinX, double MinY, double MaxX, double MaxY) bounds)
    {
        foreach (var child in children)
        {
            var absX = Canvas.GetLeft(child as Control);
            var absY = Canvas.GetTop(child as Control);

            source.RemoveChild(child);
            target.AddChild(child);

            source.Beholder.mTreeItem.Items.Remove(child.Beholder.mTreeItem);
            target.Beholder.mTreeItem.Items.Add(child.Beholder.mTreeItem);

            Canvas.SetLeft(child as Control, absX - bounds.MinX);
            Canvas.SetTop(child as Control, absY - bounds.MinY);
        }
    }

    private static void CleanupAndRestoreSelectionCanvas(JChildContainer element, JChildContainer parent, List<JControl> selectedList)
    {
        parent.Beholder.mTreeItem.Items.Remove(element.Beholder.mTreeItem);
        (parent as Control).UpdateLayout();

        foreach (var control in selectedList)
            (control as Control).UpdateLayout();

        Workspace.RestoreSelectionCanvas();
        Workspace.SelectedList.Clear();

        foreach (var control in selectedList)
        {
            Workspace.SelectedList.Add(control);
            Workspace.BindSelectionBorder(control);
        }

        Workspace.ApplySelectionCanvas();
        Workspace.InitMovable(Workspace.SelectionCanvas);
    }

    public static void CopySelection(JChildContainer selectionCanvas, JChildContainer newParent)
    {
        foreach (var cp_child in selectionCanvas.jChildren)
            Copy(cp_child, (JChildContainer)newParent);
    }
    
}