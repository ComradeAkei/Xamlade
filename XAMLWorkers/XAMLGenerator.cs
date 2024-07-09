using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Xamlade;

public static class Broadcast
{
    public delegate void EventHandler(int mode);

    public static event EventHandler OnBroadcast;

    public static void InitXAML()=> OnBroadcast?.Invoke(0);
    public static void XAMLize()=> OnBroadcast?.Invoke(1);
    
   //Восстановить поведение jElement после выгрузки из XAML
    public static void RestoreBehavior()=> OnBroadcast?.Invoke(2);
    //Убить все объекты
    public static void KillAll()=> OnBroadcast?.Invoke(3);
    //Восстановить дерево объектов
    public static void RestoreTree()=> OnBroadcast?.Invoke(4);
    //Снять выделение
  //  public static void RemoveSelection()=> OnBroadcast?.Invoke(5);



    public static void DisposeElement(JControl element) => element.Dispose();
}

public static class XAMLGenerator
{
    public static string GetProperties(JControl element)
    {
        if (element is MControl) return "";
        string getProperties = "";
        Type type = element.GetType();
        var props = type.GetProperties();
        ConstructorInfo? constructor = type.GetConstructor(new Type[] { });
        var DefaultObject = constructor.Invoke(new object[] { });
        
        foreach (var prop in props)
        {
            if((element.Name == "MainCanvas") && prop.Name is "Width" or "Height") continue;
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
        if ((element.jParent as JControl).Type == jElementType.Canvas.ToString())
        {
            getProperties+=$"Canvas.Left=\"{Convert.ToInt32(Canvas.GetLeft((Control)element))}\" ";
            getProperties+=$"Canvas.Top=\"{Convert.ToInt32(Canvas.GetTop((Control)element))}\" ";
        }

        return getProperties;

    }
    public static void XAMLRatingInit(JControl element)
    {
        if(element.Name==null) return;
        element.XAMLPiece.Clear();
        element.XAMLRating = element is IChildContainer container ? container.jChildren.Count : 0;
        element.XAMLPiece.Add($"<{element.Type} {GetProperties(element)}>");
    }
    public static void XAMLizeElement(JControl element)
    {
        if(element.Name == null) return;
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
        
        string filePath = @"XamladeDemo/MainWindow.axaml";
        var outputXAML = new List<string>();
        outputXAML.Add(@"<Window xmlns=""https://github.com/avaloniaui""
         xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
         xmlns:d=""http://schemas.microsoft.com/expression/blend/2008""
         xmlns:mc=""http://schemas.openxmlformats.org/markup-compatibility/2006""
         mc:Ignorable=""d"" Width="""+wWidth+@""" Height="""+wHeight+@"""
         x:Class=""XamladeDemo.MainWindow""
         Title=""TestWindow"">");
        outputXAML.AddRange(Workspace.MainCanvas.XAMLPiece);
        outputXAML.Add(@"</Window>");
        File.WriteAllLines(filePath, outputXAML);
    }

}