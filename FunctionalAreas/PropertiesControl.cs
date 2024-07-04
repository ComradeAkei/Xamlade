using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using AvaloniaColorPicker;

namespace Xamlade;

public static class PropertiesControl
{
    public static ListBox PropListBox { get; set; }
    private static ItemCollection PropListItems;
    public static void Init(ListBox propListBox)
    {
        PropListBox = propListBox;
        //Принудительный вызов конструктора ItemCollection
        ConstructorInfo? constructor = typeof(ItemCollection).GetConstructor(
            BindingFlags.Instance | BindingFlags.NonPublic,
            null,
            Type.EmptyTypes,
            null);
        PropListItems = constructor.Invoke(new object[] { }) as ItemCollection;
    }
    
    private static SolidColorBrush GetColor(string color)
        => new SolidColorBrush(Color.Parse(color));
    public static void ShowProperties()
    {
        try
        {
            PropListItems.Clear();
        }
        catch
        {
            return;
        }


        if (HierarchyControl.Selected.element == Workspace.MainCanvas) return;
        //  Type type = selectedTreeItem.element.GetType();
        //   = type.GetProperties();

        Type type = HierarchyControl.Selected.element.GetType();
        var props = type.GetProperties();
    
        foreach (var prop in props)
        {
            if (!Constants.ExcludedWords.Contains(prop.Name))
            {
                var prop_type = type.GetProperty(prop.Name).PropertyType;
                AddPropItem(prop.Name, prop.GetValue(HierarchyControl.Selected.element), prop_type);
                //KeyValueList.Add(new KeyValue { Key = prop.Name, Value = prop.GetValue(selectedTreeItem.element)?.ToString() });
            }
        }

        FieldInfo privateField =
            typeof(ItemsControl).GetField("_items", BindingFlags.NonPublic | BindingFlags.Instance);
        privateField.SetValue(PropListBox, PropListItems);
    }
    
    
     public static void AddPropItem(string name, object? value,Type type)
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
            Foreground = GetColor("#0ab076"),
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
            _propElement.Foreground = GetColor("#0ab076");
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
                _textBlock.Foreground = GetColor("#0ab076");
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
            
           _propElement.Foreground = GetColor("#0ab076");
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
            _propElement.Foreground = GetColor("#0ab076");
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
            _propElement.Foreground = GetColor("#0ab076");
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
     
     private static void OnPropertyChanged(object? sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter) return;
        var textBox = sender as TextBox;

        var parentPanel = textBox.Parent as DockPanel;
        var txt_blc = parentPanel.Children[0] as TextBlock;
        var prop_name = txt_blc.Text;
        Type jElement_type = HierarchyControl.Selected.element.GetType();
        var prop_type = jElement_type.GetProperty(prop_name).PropertyType;
        var prop = jElement_type.GetProperty(prop_name);
        textBox.Foreground = new SolidColorBrush(Color.Parse("#88F1FF"));
        
        object prevalue = prop.GetValue(HierarchyControl.Selected.element);
        History.AddHistoryItem(new History.Change(HierarchyControl.Selected.element,prop_name,prevalue));

        if (textBox.Text == "не число") return;
        try
        {
            if (prop.Name == "Name")
            {
                FieldInfo privateField =
                    typeof(StyledElement).GetField("_name", BindingFlags.NonPublic | BindingFlags.Instance);
                privateField.SetValue(HierarchyControl.Selected.element, textBox.Text);
                HierarchyControl.Selected.Header = textBox.Text;
            }
            else if (prop.Name == "Content")
                prop.SetValue(HierarchyControl.Selected.element, textBox.Text);
            else if (prop_type == typeof(string))
                prop.SetValue(HierarchyControl.Selected.element, textBox.Text);
            else if (prop_type == typeof(int))
                prop.SetValue(HierarchyControl.Selected.element, Convert.ToInt32(textBox.Text));
            else if (prop_type == typeof(double))
            {
                textBox.Text = textBox.Text.Replace('.', ',');
                prop.SetValue(HierarchyControl.Selected.element, Convert.ToDouble(textBox.Text));
            }
            else if (prop_type == typeof(IBrush))
            {
                var brush = new SolidColorBrush(Color.Parse(textBox.Text));
                prop.SetValue(HierarchyControl.Selected.element, brush);
                textBox.Foreground = new SolidColorBrush(Color.Parse(textBox.Text));
            }
            else if (prop_type == typeof(Thickness))
            {
                var values = textBox.Text.Split(',');
                var rect = new Thickness(Convert.ToInt32(values[0]), Convert.ToInt32(values[1]),
                    Convert.ToInt32(values[2]), Convert.ToInt32(values[3]));
                prop.SetValue(HierarchyControl.Selected.element, rect);
            }
            else if (prop_type == typeof(CornerRadius))
            {
                var values = textBox.Text.Split(',');
                var rect = new CornerRadius(Convert.ToInt32(values[0]), Convert.ToInt32(values[1]),
                    Convert.ToInt32(values[2]), Convert.ToInt32(values[3]));
                prop.SetValue(HierarchyControl.Selected.element, rect);
            }
            else if (prop_type == typeof(Rect))
            {
                var values = textBox.Text.Split(',');
                var rect = new Rect(Convert.ToInt32(values[0]), Convert.ToInt32(values[1]),
                    Convert.ToInt32(values[2]), Convert.ToInt32(values[3]));
                prop.SetValue(HierarchyControl.Selected.element, rect);
            }
        }
        catch
        {
            textBox.Text = "Некорректное значение";
            textBox.Foreground = Brushes.Red;
        }
    }
     
     private static async void OnChooseImageClick(object? sender, RoutedEventArgs e)
    {
        OpenFileDialog dialog = new OpenFileDialog();
        dialog.Title = "Выберите изображение";
        dialog.AllowMultiple = false;
        dialog.Filters.Add(new FileDialogFilter
        {
            Name = "Изображения",
            Extensions = { "png", "jpg", "jpeg", "gif", "bmp" }
        });
        Task<string[]> task = dialog.ShowAsync(MainWindow._MainWindow);
        
        // Дожидаемся завершения задачи (await)
        string[] result = await task;

        // Обрабатываем результат
        if (result != null && result.Length > 0)
        {
            string fileName = Path.GetFileName(result[0]);
            string targetFilePath = Path.Combine("assets", fileName);
            File.Copy(result[0], targetFilePath, true);
            ((jImage)HierarchyControl.Selected.element).jImageSource = @"assets/" + fileName;
            ((jImage)HierarchyControl.Selected.element).Source = new Bitmap(((jImage)HierarchyControl.Selected.element).jImageSource);
        }
        else
        {
            return;
        }
    }
    private static void OnEnumPropertyChanged(object? sender, SelectionChangedEventArgs e)
    {
        var comboBox = sender as ComboBox;
        var parentPanel = comboBox.Parent as DockPanel;
        var txt_blc = parentPanel.Children[0] as TextBlock;
        var prop_name = txt_blc.Text;
        Type jElement_type = HierarchyControl.Selected.element.GetType();
        var prop_type = jElement_type.GetProperty(prop_name).PropertyType;
        var prop = jElement_type.GetProperty(prop_name);
        object enumValue = Enum.Parse(prop_type, comboBox.SelectedItem.ToString());
        
        object prevalue = prop.GetValue(HierarchyControl.Selected.element);
        History.AddHistoryItem(new History.Change(HierarchyControl.Selected.element,prop_name,prevalue));
        
        prop.SetValue(HierarchyControl.Selected.element, enumValue);
    }

    private static void OnColorChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property == AvaloniaColorPicker.ColorButton.ColorProperty)
        {
            var colorButton = sender as ColorButton;
            var parentPanel = colorButton.Parent.Parent as DockPanel;
            var txt_blc = parentPanel.Children[0] as TextBlock;
            var prop_name = txt_blc.Text;
           
            Type jElement_type = HierarchyControl.Selected.element.GetType();
            var prop_type = jElement_type.GetProperty(prop_name).PropertyType;
            var prop = jElement_type.GetProperty(prop_name);
           
            object prevalue = prop.GetValue(HierarchyControl.Selected.element);
            History.AddHistoryItem(new History.Change(HierarchyControl.Selected.element,prop_name,prevalue));
           
            prop.SetValue(HierarchyControl.Selected.element, new SolidColorBrush(colorButton.Color));
            var textBlock = ((StackPanel)(colorButton.Parent)).Children[0] as TextBlock;
            textBlock.Text = colorButton.Color.ToString();
        }
    }
    
    private static void OnBoolPropertyChanged(object? sender, RoutedEventArgs e)
    {
        var checkBox = sender as CheckBox;
        var parentPanel = checkBox.Parent as DockPanel;
        var txt_blc = parentPanel.Children[0] as TextBlock;
        var prop_name = txt_blc.Text;
        Type jElement_type = HierarchyControl.Selected.element.GetType();
        var prop = jElement_type.GetProperty(prop_name);
       
        object prevalue = prop.GetValue(HierarchyControl.Selected.element);
        History.AddHistoryItem(new History.Change(HierarchyControl.Selected.element,prop_name,prevalue));
        
        prop.SetValue(HierarchyControl.Selected.element, checkBox.IsChecked);
    }
     
}