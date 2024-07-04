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

public record jCoordinates(double X, double Y)
{
    public double X { get; set; } = X;
    public double Y { get; set; } = Y;
        
}
    
public record jSize(double Width, double Height)
{
    public double Width { get; set; } = Width;
    public double Height { get; set; } = Height;
        
}

public static class History
{
    private static bool HistoryOperationFlag = false;
    
    
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
    
    public static List<Change> UndoList = new();
    public static List<Change> RedoList = new();
    
    
    
    //Добавить изменение в очередь отмены
    public static void AddHistoryItem(Change change)
    {
        if (change.jObject.Name == "MainCanvas") return;
        if(HistoryOperationFlag) return;
        RedoList.Clear();
        if(UndoList.Count>0)
            if(change==UndoList.Last()) return;
        UndoList.Add(change);
        if (change.FieldName == "Coordinates")
            Console.WriteLine(change.jObject.Name + " " + change.FieldName + " " + (change.Value as jCoordinates).X + " " +
                              (change.Value as jCoordinates).Y);
        else
            Console.WriteLine(change.jObject.Name + " " + change.FieldName + " " + change.Value.ToString());
    }


    private static void HistoryOperation(bool mode)
    {
        Change state;
        HistoryOperationFlag = true;
        if(!mode)
        {
            if (UndoList.Count == 0)
            {
                HistoryOperationFlag = false;
                return;
            }

            state = UndoList.Last();
            
            object? CurStateValue;
            if (state.FieldName == "Coordinates")
                CurStateValue = new jCoordinates(Canvas.GetLeft(state.jObject as Control), Canvas.GetTop(state.jObject as Control));
            else if (state.FieldName == "Size")
                CurStateValue = new jSize((state.jObject as Control).Width, (state.jObject as Control).Height);
            else if (state.FieldName == "Created")
                CurStateValue = state.Value;
            else if (state.FieldName == "Removed")
                CurStateValue = state.Value;
           
            else
            {
                Type type = state.jObject.GetType();
                var fieldInfo = type.GetProperty(state.FieldName);
                CurStateValue = fieldInfo.GetValue(state.jObject);
            }
                
            if (CurStateValue != null)
            {
                var CurState = new Change(UndoList.Last().jObject, UndoList.Last().FieldName, CurStateValue);
                RedoList.Add(CurState);
            }
            
            if(state.FieldName=="Coordinates")
                if (CurStateValue as jCoordinates == state.Value as jCoordinates)
                {
                    UndoList.Remove(UndoList.Last());
                    HistoryOperation(false);
                    HistoryOperationFlag = false;
                    return;
                }
            
            
            UndoList.Remove(UndoList.Last());
        }
        else
        {
            if(RedoList.Count==0)
            {
                HistoryOperationFlag = false;
                return;
            }
            state = RedoList.Last();
            
            object? CurStateValue;
            switch (state.FieldName)
            {
                case "Coordinates":
                    CurStateValue = new jCoordinates(Canvas.GetLeft(state.jObject as Control), Canvas.GetTop(state.jObject as Control));
                    break;
                case "Size":
                    CurStateValue = new jSize((state.jObject as Control).Width, (state.jObject as Control).Height);
                    break;
                case "Created":
                case "Removed":
                    CurStateValue = state.Value;
                    break;
                default:
                {
                    Type type = state.jObject.GetType();
                    var fieldInfo = type.GetProperty(state.FieldName);
                    CurStateValue = fieldInfo.GetValue(state.jObject);
                    break;
                }
            }
            var CurState = new Change(RedoList.Last().jObject, RedoList.Last().FieldName, CurStateValue);
            UndoList.Add(CurState);
           

            RedoList.Remove(RedoList.Last());
        }

        var tst = mode ? "Redo" : "Undo";
        Console.WriteLine("History: "+ tst+" " + state.jObject.Name + " " + state.FieldName + " " + state.Value.ToString());
        
        
       //ЧО ЗА ХУЙНЯ
    //    HierarchyControl.HierarchyTree.SelectedItem = state.jObject.mTreeItem;
        HierarchyControl.Selected = state.jObject.mTreeItem;
        
        switch (state.FieldName)
        {
            case "Coordinates":
                Canvas.SetLeft(state.jObject as Control,(state.Value as jCoordinates).X);
                Canvas.SetTop(state.jObject as Control,(state.Value as jCoordinates).Y);
                HistoryOperationFlag = false;
                return;
            case "Size":
                (state.jObject as Control).Width = (state.Value as jSize).Width;
                (state.jObject as Control).Height = (state.Value as jSize).Height;
                HistoryOperationFlag = false;
                return;
            case "Created" when (!mode):
            case "Removed" when (mode):
                HierarchyControl.Selected.element = state.jObject;
                Workspace.RemoveSelectedjElement();
                HistoryOperationFlag = false;
                return;
            case "Created" when (mode):
            case "Removed" when (!mode):
            {
                var array = state.Value as object[];
                var parent = (IChildContainer)array[0];
                var element = (JControl)array[1];
                var mTree = (mTreeViewItem)array[2];
                ResurrectElement(parent, element, mTree);
                HistoryOperationFlag = false;
                return;
            }
        }

        var jElement_type = HierarchyControl.Selected.element.GetType();
        var prop = jElement_type.GetProperty(state.FieldName);
        prop.SetValue(state.jObject, state.Value);
        HistoryOperationFlag = false;
        
    }
    
    
   
    public static void UNDO(object? sender, RoutedEventArgs e)
    {
        HistoryOperation(false);
    }


    private static void ResurrectElement(IChildContainer parent, JControl element, mTreeViewItem mtree)
    {
        //Блок реанимации
        Reflector.SetName(mtree.Header.ToString(),element);
        element.mTreeItem = mtree;
        element.jParent = parent;
        (parent as JControl).mTreeItem.Items.Add(mtree);
        parent.AddChild(element,Canvas.GetTop(element as Control),Canvas.GetLeft(element as Control));
        HierarchyControl.Selected = element.mTreeItem;
        HierarchyControl.Selected = element.mTreeItem;
    }
    public static void REDO(object? sender, RoutedEventArgs e)
    {
        HistoryOperation(true);
    }
}