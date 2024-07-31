using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Xamlade.Extensions;
using Xamlade.FunctionalAreas;
using Xamlade.jClasses;
using Xamlade.mClasses;

namespace Xamlade.XAMLWorkers;

public static class XAMLGenerator
{
    public static string GetProperties(JControl element)
    {
        if (element.jParent is null && element.Name != "MainCanvas") return "";
        if (element is MControl) return "";
        string getProperties = "";
        Type type = element.GetType();
        var props = type.GetProperties();
        ConstructorInfo? constructor = type.GetConstructor(new Type[] { });
        var DefaultObject = constructor.Invoke(new object[] { });

        foreach (var prop in props)
        {
            if ((element.Name == "MainCanvas") && prop.Name is "Width" or "Height") continue;
            if((element.Type == "ComboBoxItem") && (element as jComboBoxItem).jChildren.Count !=0 && prop.Name is "Content") continue;
            if (element.Type == "Border") continue;
            if (!Constants.ExcludedWords.Contains(prop.Name))
            {
                if (prop.Name == "Source")
                    getProperties += ($"{prop.Name}=\"{((jImage)element).jImageSource}\" ");
                else if (prop.GetValue(element)?.ToString() != prop.GetValue(DefaultObject)?.ToString()
                         && prop.GetValue(element) != null)
                    getProperties += ($"{prop.Name}=\"{prop.GetValue(element)}\" ");
            }
        }

        if (element.Name == "MainCanvas")
        {
            getProperties += " HorizontalAlignment=\"Stretch\"\n            VerticalAlignment=\"Stretch\"";
            return getProperties;
        }

        if (Equals(element, Workspace.SelectionCanvas)) return "";
        if ((element.jParent as JControl).Type == jElementType.Canvas.ToString())
        {
            getProperties += $"Canvas.Left=\"{Convert.ToInt32(jCanvas.GetLeft(element))}\" ";
            getProperties += $"Canvas.Top=\"{Convert.ToInt32(jCanvas.GetTop(element))}\" ";
        }
        else if ((element.jParent as JControl).Type == jElementType.Grid.ToString())
        {
            int row = jGrid.GetRow(element);
            int column = jGrid.GetColumn(element);
            int rowSpan = jGrid.GetRowSpan(element);
            int columnSpan = jGrid.GetColumnSpan(element);

            getProperties += $"Grid.Row=\"{row}\" ";
            getProperties += $"Grid.Column=\"{column}\" ";

            if (rowSpan > 1)
            {
                getProperties += $"Grid.RowSpan=\"{rowSpan}\" ";
            }

            if (columnSpan > 1)
            {
                getProperties += $"Grid.ColumnSpan=\"{columnSpan}\" ";
            }
        }
        else if ((element.jParent as JControl).Type == jElementType.DockPanel.ToString())
        {
            Dock dock = jDockPanel.GetDock(element);
            getProperties += $"DockPanel.Dock=\"{dock}\" ";
        }

        return getProperties;
    }

    public static void XAMLRatingInit(JControl element)
    {
        element.XAMLPiece.Clear();
        element.XAMLRating = element is JChildContainer container ? container.jChildren.Count : 0;
        element.XAMLPiece.Add($"<{element.Type} {GetProperties(element)}>");
    }

    public static void XAMLizeElement(JControl element)
    {
        if (element.Name == null) return;
        if (element.XAMLRating == 0)
        {
            element.XAMLPiece.Add($"</{element.Type}>");
            element.XAMLRating--;
            if (element.jParent is not JControl parent) return;
            parent.XAMLPiece.AddRange(element.XAMLPiece);
            parent.XAMLRating--;
        }
    }

    public static void XAMLIZE(object? sender, RoutedEventArgs? e)
    {
        Workspace.RestoreSelectionCanvas();
        // selectedTreeItem.element.Background = selectedOriginalBackground;
        Broadcast.InitXAML();
        while (Workspace.MainCanvas.XAMLRating > -1)
        {
            Broadcast.XAMLize();
        }

        var wWidth = (int)Workspace.MainCanvas.Bounds.Width;
        var wHeight = (int)Workspace.MainCanvas.Bounds.Height;

        string filePath = @"./XamladeDemo/MainWindow.axaml";
        var outputXAML = new List<string>();
        outputXAML.Add(@"<Window xmlns=""https://github.com/avaloniaui""
         xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
         xmlns:d=""http://schemas.microsoft.com/expression/blend/2008""
         xmlns:mc=""http://schemas.openxmlformats.org/markup-compatibility/2006""
         mc:Ignorable=""d"" Width=""" + wWidth + @""" Height=""" + wHeight + @"""
         x:Class=""XamladeDemo.MainWindow""
         Title=""TestWindow"">");
        outputXAML.AddRange(Workspace.MainCanvas.XAMLPiece);
        outputXAML.Add(@"</Window>");
        File.WriteAllLines(filePath, outputXAML);
    }
}