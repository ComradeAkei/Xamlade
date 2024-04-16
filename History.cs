using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using AvaloniaColorPicker;

namespace Xamlade;

public partial class MainWindow
{
    
    public record Coordinates(double X, double Y)
    {
        public double X { get; set; } = X;
        public double Y { get; set; } = Y;
        
    }
    
    public record Size(double Width, double Height)
    {
        public double Width { get; set; } = Width;
        public double Height { get; set; } = Height;
        
    }
    
    public struct Change
    {
        public JControl jObject { get; set; }
        public string FieldName;
        public object Value;
        public Change(JControl jObjectl, string FieldName, object Value)
        {
            this.jObject = jObjectl;
            this.FieldName = FieldName;
            this.Value = Value;
        }
        public static bool operator ==(Change change1, Change change2)
        {
            return change1.jObject.Name == change2.jObject.Name &&
                   change1.FieldName == change2.FieldName &&
                   Equals(change1.Value, change2.Value);
        }
        public static bool operator !=(Change change1, Change change2)
        {
            return !(change1 == change2);
        }
        public bool Equals(Change other)
        {
            return FieldName == other.FieldName && Value.Equals(other.Value) && jObject.Equals(other.jObject);
        }
        public override bool Equals(object? obj)
        {
            return obj is Change other && Equals(other);
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(FieldName, Value, jObject);
        }
        
    }
    
    private List<Change> UndoList = new();
    private List<Change> RedoList = new();
    
    
    
    //Добавить изменение в очередь отмены
    private void AddHistoryItem(Change change)
    {
        if (change.jObject.Name == "MainCanvas") return;
        if(UndoList.Count>0)
            if(change==UndoList.Last()) return;
        UndoList.Add(change);
        if (change.FieldName == "Coordinates")
            Console.WriteLine(change.jObject.Name + " " + change.FieldName + " " + (change.Value as Coordinates).X + " " +
                              (change.Value as Coordinates).Y);
        else
            Console.WriteLine(change.jObject.Name + " " + change.FieldName + " " + change.Value.ToString());
    }
    
    //TODO: сделать универсальным для REDO
    private void UNDO(object? sender, RoutedEventArgs e)
    {
        if(UndoList.Count == 0) return;
        Change state = UndoList.Last();
        //Перемещать в REDO
        UndoList.Remove(UndoList.Last());

        MainHierarchyTree.SelectedItem = state.jObject.mTreeItem;
        selectedTreeItem = state.jObject.mTreeItem;

        if (state.FieldName == "Coordinates")
        {
            Canvas.SetLeft(state.jObject as Control,(state.Value as Coordinates).X);
            Canvas.SetTop(state.jObject as Control,(state.Value as Coordinates).Y);
            return;
        }
        if(state.FieldName=="Created")
        {
            selectedTreeItem.element = state.jObject;
            RemovejElement(null,null);
            return;
        }

        if (state.FieldName == "Removed")
        {
            var array = state.Value as Object[];
            IChildContainer parent = (IChildContainer)array[0];
            JControl element = (JControl)array[1];
            mTreeViewItem mTree = (mTreeViewItem)array[2];
            ResurrectElement(parent, element, mTree);
            return;
            
            
        }
        
        Type jElement_type = selectedTreeItem.element.GetType();
        var prop = jElement_type.GetProperty(state.FieldName);
        prop.SetValue(state.jObject, state.Value);

    }


    private void ResurrectElement(IChildContainer parent, JControl element, mTreeViewItem mtree)
    {
        //Блок реанимации
        Reflector.SetName(mtree.Header.ToString(),element);
        element.mTreeItem = mtree;
        element.jParent = parent;
        (parent as JControl).mTreeItem.Items.Add(mtree);
        parent.AddChild(element,Canvas.GetTop(element as Control),Canvas.GetLeft(element as Control));
        MainHierarchyTree.SelectedItem = element.mTreeItem;
        selectedTreeItem = element.mTreeItem;
    }
    private void REDO(object? sender, RoutedEventArgs e)
    {
        
        
        
    }
}