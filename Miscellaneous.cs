using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Markup.Xaml.Templates;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Themes.Fluent;
using Avalonia.Threading;
using FluentAvalonia.UI.Controls;
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
      

    public void AddPropItem(string name, object? value,Type type)
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
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(10,0,0,0),
            MaxWidth=150
        };
        DockPanel.SetDock(textBlock,Dock.Left);
        dockPanel.Children.Add(textBlock);

        
        
        
        
        
        if (type == typeof(int) || type == typeof(string) || type == typeof(double) || type == typeof(bool) || name == "Content")
        {
            var _propElement = new TextBox();
            _propElement.Text = value.ToString();
            _propElement.Foreground = GetColor("#88F1FF");
            _propElement.FontWeight = FontWeight.DemiBold;
            _propElement.HorizontalAlignment = HorizontalAlignment.Right;
            _propElement.VerticalAlignment = VerticalAlignment.Center;
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
        else if (type.IsEnum)
        {
            var _propElement = new ComboBox();
            _propElement.Margin = new Thickness(0, 0, 0, 10);
            
            //Чтоб нормально отображался Placeholder
            var dataTemplate = new FuncDataTemplate<string>((item, _) =>
            {
                var textBlock = new TextBlock{VerticalAlignment = VerticalAlignment.Center};
                textBlock.Bind(TextBlock.TextProperty, new Binding(".")); 
                return textBlock;
            });
            _propElement.ItemTemplate = dataTemplate;
            
           _propElement.Foreground = GetColor("#88F1FF");
           _propElement.FontWeight = FontWeight.DemiBold;
           _propElement.HorizontalAlignment = HorizontalAlignment.Right;
           _propElement.VerticalAlignment = VerticalAlignment.Center;
           _propElement.VerticalContentAlignment = VerticalAlignment.Bottom;
           _propElement.Margin = new Thickness(5, 0, 0, 0);
           _propElement.Width = 100;
            var enumValues = Enum.GetValues(type);
            
            foreach (var _value in enumValues)
                _propElement.Items.Add(_value.ToString());
            _propElement.SelectedItem = value.ToString();
             _propElement.SelectionChanged+= OnEnumPropertyChanged;
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