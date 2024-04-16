using System;
using System.Linq;
using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using AvaloniaColorPicker;

namespace Xamlade;

public partial class MainWindow
{
    private static bool LCtrlPressed = false;
    private static ItemCollection PropListItems;
  //Принудительный вызов конструктора ItemCollection
  ConstructorInfo? constructor = typeof(ItemCollection).GetConstructor(
      BindingFlags.Instance | BindingFlags.NonPublic,
      null,
      Type.EmptyTypes,
      null);


    
    private void AddPropItem(string name, object? value,Type type)
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
            FontWeight = FontWeight.Normal,
            FontSize = 18,
            HorizontalAlignment = HorizontalAlignment.Right,
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(10,0,0,0),
            MaxWidth=150
        };
        DockPanel.SetDock(textBlock,Dock.Left);
        dockPanel.Children.Add(textBlock);
        
        
        if (type == typeof(int) || type == typeof(string) || 
            type == typeof(double) || name == "Content" || 
            type == typeof(Thickness) || type == typeof(CornerRadius) || 
            type == typeof(Rect) )
        {
            var _propElement = new TextBox();
            _propElement.Text = value?.ToString();
            _propElement.Foreground = GetColor("#88F1FF");
            _propElement.FontWeight = FontWeight.Normal;
            _propElement.HorizontalAlignment = HorizontalAlignment.Right;
            _propElement.VerticalAlignment = VerticalAlignment.Center;
            _propElement.Margin = new Thickness(5, 0, 0, 0);
            _propElement.KeyDown += OnPropertyChanged;
            DockPanel.SetDock(_propElement, Dock.Right);
            dockPanel.Children.Add(_propElement);
        }
        //Для цветов
        else if (type == typeof(IBrush))
        {
            var stackPanel = new StackPanel{Orientation = Orientation.Horizontal};
            var _textBlock = new TextBlock();
            
            if (value != null)
            {
                var colorButton = new ColorButton
                {
                    Color = Color.Parse(value.ToString())
                };
                colorButton.PropertyChanged += OnColorChanged;
                _textBlock.Text = colorButton.Color.ToString();
                _textBlock.VerticalAlignment = VerticalAlignment.Center;
                _textBlock.Foreground = GetColor("#88F1FF");
                _textBlock.FontWeight = FontWeight.Normal;
                stackPanel.HorizontalAlignment = HorizontalAlignment.Right;
                stackPanel.Children.Add(_textBlock);
                stackPanel.Children.Add(colorButton);
               // colorButton.HorizontalAlignment = HorizontalAlignment.Right;
                DockPanel.SetDock(stackPanel, Dock.Right);
                dockPanel.Children.Add(stackPanel);
            }
        }
        //Для перечислений
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
           _propElement.FontWeight = FontWeight.Normal;
           _propElement.HorizontalAlignment = HorizontalAlignment.Right;
           _propElement.VerticalAlignment = VerticalAlignment.Center;
           _propElement.VerticalContentAlignment = VerticalAlignment.Bottom;
           _propElement.Margin = new Thickness(5, 0, 0, 0);
            var enumValues = Enum.GetValues(type);
            
            foreach (var _value in enumValues) 
                _propElement.Items.Add(_value.ToString());
            _propElement.SelectedItem = value?.ToString();
            _propElement.SelectionChanged+= OnEnumPropertyChanged;
            DockPanel.SetDock(_propElement, Dock.Right);
            dockPanel.Children.Add(_propElement); 
        }
        else if (type == typeof(Boolean?) || type == typeof(bool))
        {
            var _propElement = new CheckBox();
            _propElement.IsChecked = (bool)value;
            _propElement.Width = 30;
            _propElement.HorizontalAlignment = HorizontalAlignment.Right;
            _propElement.Foreground = GetColor("#88F1FF");
            _propElement.FontWeight = FontWeight.Normal;
            _propElement.HorizontalContentAlignment = HorizontalAlignment.Right;
            _propElement.VerticalAlignment = VerticalAlignment.Center;
            _propElement.VerticalContentAlignment = VerticalAlignment.Bottom;
            _propElement.Margin = new Thickness(5, 0, 0, 0);
            _propElement.IsCheckedChanged += OnBoolPropertyChanged;
            DockPanel.SetDock(_propElement, Dock.Right);
            dockPanel.Children.Add(_propElement);
            
        }
        else if ( type == typeof(IImage))
        {
            var _propElement = new Button();
            _propElement.Content = "Выбрать";
            _propElement.HorizontalAlignment = HorizontalAlignment.Right;
            _propElement.Foreground = GetColor("#88F1FF");
            _propElement.FontWeight = FontWeight.Normal;
            _propElement.HorizontalContentAlignment = HorizontalAlignment.Right;
            _propElement.VerticalAlignment = VerticalAlignment.Center;
            _propElement.VerticalContentAlignment = VerticalAlignment.Bottom;
            _propElement.Margin = new Thickness(5, 0, 0, 0);
            _propElement.Click += OnChooseImageClick;
            DockPanel.SetDock(_propElement, Dock.Right);
            dockPanel.Children.Add(_propElement);
            
        }
        else
        {
            var _propElement = new TextBox();
            _propElement.Text = value?.ToString();
            _propElement.Foreground = Brushes.Red;
            _propElement.FontWeight = FontWeight.Normal;
            _propElement.HorizontalAlignment = HorizontalAlignment.Right;
            _propElement.Margin = new Thickness(5, 0, 0, 0);
            _propElement.Width = 100;
            //_propElement.KeyDown += OnPropertyChanged;
            DockPanel.SetDock(_propElement, Dock.Right);
            dockPanel.Children.Add(_propElement); 
        }

        var border = new Border();
        border.BorderThickness = new Thickness(0, 0, 0, 1);
        border.BorderBrush = GetColor("#8897FF");
        border.Child = dockPanel;
        listItem.Content = border;
        
        PropListItems.Add(listItem);
    }

    private SolidColorBrush GetColor(string color)
        => new SolidColorBrush(Color.Parse(color));

    //Корректировка координат для перемещения и растяжения в строгом режиме
    double CorrectCoords(double coord)
    {
        if ((bool)!StrictModeEnabled.IsChecked) return Math.Round(coord);
        if(StrictModeValue.Text == "") return Math.Round(coord);
        int step = Convert.ToInt32(StrictModeValue.Text);
        if (step <= 0) return coord;
        var _coord = (((int)coord)/step)*step;
        return _coord;

    }
   private double CorrectSize(double coord)
        => CorrectCoords(coord)>0?CorrectCoords(coord):Convert.ToInt32(StrictModeValue.Text);


    private void StrictModeEnabled_OnPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (StrictModeValue == null) return;
        try
        {
            var value = Convert.ToInt32(StrictModeValue.Text);
            StrictModeValue.Text = Math.Abs(value).ToString();
        }
        catch 
        {
            StrictModeValue.Text = "";
        }
    }

   
}