using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Xamlade.LinkWorkers;
using Xamlade.XAMLWorkers;

namespace Xamlade.jClasses;

public class jGrid: Grid, JControl, JChildContainer, JSelectable, JBroadcastHandler<JControl>
{
    protected override Type StyleKeyOverride => typeof(Grid);
    public Beholder Beholder { get; set; }
    public Dictionary<string, JChildContainer.ContainerSetPropertyDelegate> SpecialSetDelegates { get; set; }
    public JChildContainer? _jParent { get; set; }
   // public JChildContainer? jParent { get; set; }
    private string controlType => jElementType.Grid.ToString();
    public string Type => controlType;
    public int XAMLRating { get; set; }
    public List<string> XAMLPiece { get; set; }
    public bool IsPressed { get; set; }
    public event EventHandler<RoutedEventArgs>? Click;
    public List<(string, JChildContainer.ContainerPropertyDelegate)> ContainerProperties { get; set; }
    public static Dictionary<string, JChildContainer.ContainerSetPropertyDelegate> ContainerSetProperties { get; set; }

    public List<JControl> jChildren { get; }
    
    
    public jGrid()
    {
        SpecialSetDelegates = new();
        jChildren = new List<JControl>();
        Broadcast.OnBroadcast += (this as JBroadcastHandler<JControl>).HandleBroadcast;
        XAMLPiece = new List<string>();
        AddSpecialSetDelegates();
    }

    
    
    public void AddChild(JControl child)
    {
        jChildren.Add(child);
        child.jParent = this;
        Children.Add((Control)child);
    }
    

    public void RemoveChild(JControl child)
    {
        jChildren.Remove(child);
        Children.Remove((Control)child);
    }

    
    
    static jGrid()
    {
        ContainerSetProperties = new Dictionary<string, JChildContainer.ContainerSetPropertyDelegate>
        {
            { "Row", (JControl element, Property prop) => SetRow(element,(int)(prop.Value ?? 0)) },
            { "Column", (JControl element, Property prop) => SetColumn(element,(int)(prop.Value ?? 0)) },
        };
    }
    public void InitContainerProperties()
    {
        ContainerProperties = new List<(string, JChildContainer.ContainerPropertyDelegate)>
        {
            ( "Row", element => new Property(GetRow(element)) ),
            ( "Column", element => new Property(GetColumn(element))),
        };
    }

    public static void SetRow(JControl element, int value) => 
        Grid.SetRow(element as Control, value);

    public static void SetColumn(JControl element, int value) => 
        Grid.SetColumn(element as Control, value);

    public static void SetRowSpan(JControl element, int value) => 
        Grid.SetRowSpan(element as Control, value);

    public static void SetColumnSpan(JControl element, int value) => 
        Grid.SetColumnSpan(element as Control, value);

    public static int GetRow(JControl element) => 
        Grid.GetRow(element as Control);

    public static int GetColumn(JControl element) => 
        Grid.GetColumn(element as Control);

    public static int GetRowSpan(JControl element) => 
        Grid.GetRowSpan(element as Control);

    public static int GetColumnSpan(JControl element) => 
        Grid.GetColumnSpan(element as Control);

    public mBorder selectionBorder { get; set; }

    public Dictionary<string, Dictionary<string, Property>> xPropertiesGroup { get; set; }

    public void AddSpecialProperties()
    {
        xPropertiesGroup["specials"] = new();
        xPropertiesGroup["specials"]["Rows"] = new Property(this.RowDefinitions.Count, typeof(int),2);
        xPropertiesGroup["specials"]["Columns"] = new Property(this.ColumnDefinitions.Count, typeof(int),2);
        this.Beholder.UpdatePropList("Rows");
        this.Beholder.UpdatePropList("Columns");
    }

    public void AddSpecialSetDelegates()
    {
        SpecialSetDelegates["Rows"] = (element, value) => SetRowsCount(element, (int)(value.Value ?? 0));
        SpecialSetDelegates["Columns"] = (element, value) => SetColumnsCount(element, (int)(value.Value ?? 0));
    }

    private void SetRowsCount(JControl element, int newRows)
    {
        var grid = element as jGrid;
        var rows = grid.RowDefinitions.Count;

        if (rows == newRows)
        {
            return;
        }

        if (newRows > rows)
        {
            for (int i = rows; i < newRows; i++)
            {
                grid.RowDefinitions.Add(new mRowDefinition(grid, 100));
            }
        }
        else
        {
            // Создаем список строк с детьми
            var rowsWithChildren = new List<int>();
            foreach (var child in grid.Children)
            {
                int row = Grid.GetRow(child);
                if (!rowsWithChildren.Contains(row))
                {
                    rowsWithChildren.Add(row);
                }
            }

            // Удаляем строки, не затрагивая строки с детьми
            for (int i = rows - 1; i >= newRows; i--)
            {
                if (!rowsWithChildren.Contains(i))
                {
                    grid.RowDefinitions.RemoveAt(i);
                }
            }
        }
    }

    private void SetColumnsCount(JControl element, int newColumns)
    {
        var grid = element as jGrid;
        var columns = grid.ColumnDefinitions.Count;

        if (columns == newColumns)
        {
            return;
        }

        if (newColumns > columns)
        {
            for (int i = columns; i < newColumns; i++)
            {
                grid.ColumnDefinitions.Add(new mColumnDefinition(grid, 100));
            }
        }
        else
        {
            // Создаем список колонок с детьми
            var columnsWithChildren = new List<int>();
            foreach (var child in grid.Children)
            {
                int column = Grid.GetColumn(child);
                if (!columnsWithChildren.Contains(column))
                {
                    columnsWithChildren.Add(column);
                }
            }
                    
            for (int i = columns - 1; i >= newColumns; i--)
            {
                if (!columnsWithChildren.Contains(i))
                {
                    grid.ColumnDefinitions.RemoveAt(i);
                }
            }
        }
                
    }
    
    
    
    
    
}