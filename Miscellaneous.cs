using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Threading;
using SkiaSharp;

namespace Xamlade;

public enum PropType
{
    String,
    Enum,
    Color
    
}

public partial class MainWindow
{
  //Принудительный вызов конструктора ItemCollection
  ConstructorInfo? constructor = typeof(ItemCollection).GetConstructor(
      BindingFlags.Instance | BindingFlags.NonPublic,
      null,
      Type.EmptyTypes,
      null);
  private ItemCollection PropListItems;
      

    public void AddPropItem(string name, object value)
    {
        var listItem = new ListBoxItem();
        var dockPanel = new DockPanel
        {
            Height = 32
        };
        
        var textBlock = new TextBlock
        {
            Text = name,
            //  DockPanel.Dock="Left" VerticalAlignment="Center"
            Foreground = GetColor("#88F1FF"),
            FontWeight = FontWeight.DemiBold,
            FontSize = 18,
            HorizontalAlignment = HorizontalAlignment.Right,
            Margin = new Thickness(10,0,0,0),
            MaxWidth=150
        };
        DockPanel.SetDock(textBlock,Dock.Left);
        dockPanel.Children.Add(textBlock);
        

        var type = value.GetType();
        if (type == typeof(int) || type == typeof(string) || type == typeof(double) || type == typeof(bool))
        {
            var _propElement = new TextBox();
            _propElement.Text = value.ToString();
            _propElement.Foreground = GetColor("#88F1FF");
            _propElement.FontWeight = FontWeight.DemiBold;
            _propElement.HorizontalAlignment = HorizontalAlignment.Right;
            _propElement.Margin = new Thickness(5, 0, 0, 0);
            _propElement.Width = 100;
            _propElement.KeyDown += OnPropertyChanged;
            DockPanel.SetDock(_propElement, Dock.Right);
            dockPanel.Children.Add(_propElement);
        }
        else if (type == typeof(IBrush))
        {
            var _propElement = new TextBox();
            _propElement.Text = value.ToString();
            _propElement.Foreground = (IBrush)value;
            _propElement.FontWeight = FontWeight.DemiBold;
            _propElement.HorizontalAlignment = HorizontalAlignment.Right;
            _propElement.Margin = new Thickness(5, 0, 0, 0);
            _propElement.Width = 100;
            _propElement.KeyDown += OnPropertyChanged;
            DockPanel.SetDock(_propElement, Dock.Right);
            dockPanel.Children.Add(_propElement);
        }
        else
        {
            var _propElement = new TextBox();
            _propElement.Text = value.ToString();
            _propElement.Foreground = Brushes.Red;
            _propElement.FontWeight = FontWeight.DemiBold;
            _propElement.HorizontalAlignment = HorizontalAlignment.Right;
            _propElement.Margin = new Thickness(5, 0, 0, 0);
            _propElement.Width = 100;
            //_propElement.KeyDown += OnPropertyChanged;
            DockPanel.SetDock(_propElement, Dock.Right);
            dockPanel.Children.Add(_propElement); 
        }
        listItem.Content = dockPanel;
        PropListItems.Add(listItem);
    }

    private SolidColorBrush GetColor(string color)
        => new SolidColorBrush(Color.Parse(color));
    



}