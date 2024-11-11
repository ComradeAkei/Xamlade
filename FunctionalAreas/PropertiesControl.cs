using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.LogicalTree;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Media.Immutable;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using AvaloniaColorPicker;
using Xamlade.ColorPicker;
using Xamlade.Extensions;
using Xamlade.jClasses;
using Xamlade.LinkWorkers;
using Xamlade.ProgramWindow;
using MyColorButton = Xamlade.ColorPicker.MyColorButton;
using ColorPicker = AvaloniaColorPicker.ColorPicker;
using Enum = System.Enum;

// ReSharper disable All

namespace Xamlade.FunctionalAreas;

public static class PropertiesControl
{
    public static ListBox PropListBox { get; set; }
    private static ItemCollection PropListItems;

    public static void Init(ListBox propListBox)
    {
        PropListBox = propListBox;


        var listBoxItemStyle = new Style(x => x.OfType<ListBoxItem>())
        {
            Setters =
            {
                new Setter(ListBoxItem.MarginProperty, new Thickness(0)),
                new Setter(ListBoxItem.PaddingProperty, new Thickness(0)),
                new Setter(ListBoxItem.HorizontalAlignmentProperty, HorizontalAlignment.Stretch),
                new Setter(ListBoxItem.MinHeightProperty, (double)0),
                new Setter(ListBoxItem.MaxHeightProperty, double.PositiveInfinity)
            }
        };

        // Добавляем стиль в коллекцию стилей ListBox
        PropListBox.Styles.Add(listBoxItemStyle);

        // Использование рефлексии для создания экземпляра ItemCollection
        var constructor = typeof(ItemCollection).GetConstructor(
            BindingFlags.Instance | BindingFlags.NonPublic,
            null,
            Type.EmptyTypes,
            null);

        PropListItems = (ItemCollection)constructor.Invoke(new object[] { });
    }

    private static SolidColorBrush GetColor(string color) => new(Color.Parse(color));

    private static JControl? PropElement = null;

    public static void ShowProperties()
    {

        //ОСТАВИТЬ ЧТОБЫ СВОЙСТВА НЕ ДЁРГАЛИСЬ
         if (PropElement != null && PropElement.Equals(HierarchyControl.Selected.Beholder.element))
             return;
         PropElement = HierarchyControl.Selected.Beholder.element as JControl;
        //
        
        if (PropListBox != null)
        {
            if ((Equals(HierarchyControl.Selected.Beholder.element, Workspace.MainCanvas)) || (Equals(HierarchyControl.Selected.Beholder.element, Workspace.SelectionCanvas)))
            {
                PropListBox.ItemsSource = null;
                return;
            }
            // Отключаем виртуализацию
            PropListBox.ItemsPanel = new FuncTemplate<Panel>(() => new StackPanel());
            PropListBox.ItemsSource = null;
            // Обновляем ItemsSource в главном UI потоке
            Dispatcher.UIThread.InvokeAsync(() => 
                PropListBox.ItemsSource = HierarchyControl.Selected.Beholder.PropListItems);
        }
    }
    
    public static ListBoxItem CreatePropItem(string name, Property prop)
    {
        if (name == "main") return null;
        else if (name == "specials") return SpecialsLabel("Специальные");
        else if (name == "container") return SpecialsLabel(prop.Value?.ToString() ?? "Контейнер");

        if (name == "ColumnDefinitions" || name == "RowDefinitions") return null;
        var listItem = new ListBoxItem
        {
            Content = new Border
            {
                BorderThickness = new Thickness(0, 0, 0, 1),
                BorderBrush = GetColor("#8897FF"),
                Child = CreatePropertyPanel(name, prop.Value, prop.Type)
            }
        };
        listItem.Name = name;
        return listItem;
        //beholder.PropListItems.Add(listItem);
        //PropListItems?.Add(listItem);
    }

    public static ListBoxItem SpecialsLabel(string LABEL)
    {
        var listItem = new ListBoxItem
        {
            Content = new Border
            {
                BorderThickness = new Thickness(0, 0, 0, 1),
                BorderBrush = GetColor("#8897FF"),
                Child = new DockPanel
                {
                    Height = 40,
                    HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
                    VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                    Children =
                    {
                        new TextBlock
                        {
                            Text = " " + LABEL,
                            Foreground = GetColor("#9cd638"),
                            FontSize = 20,
                            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                            VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
                        }
                    }
                }
            }
        };
        return listItem;
    }


    private static DockPanel CreatePropertyPanel(string name, object value, Type type)
    {
        var dockPanel = new DockPanel { Height = 32 };

        var textBlock = CreatePropertyNameTextBlock(name);
        dockPanel.Children.Add(textBlock);

        var controlPanel = CreatePropertyControlPanel(name, value, type);
        dockPanel.Children.Add(controlPanel);

        return dockPanel;
    }

    private static TextBlock CreatePropertyNameTextBlock(string name)
    {
        var textBlock = new TextBlock
        {
            Text = name,
            Foreground = GetColor("#0ab076"),
            FontWeight = FontWeight.Normal,
            FontSize = 18,
            HorizontalAlignment = HorizontalAlignment.Right,
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(10, 0, 0, 0),
            MaxWidth = 150
        };
        DockPanel.SetDock(textBlock, Dock.Left);
        return textBlock;
    }

    private static Control CreatePropertyControlPanel(string name, object value, Type type)
    {
        if (type == typeof(int) || type == typeof(string) ||
            type == typeof(double) || name == "Content" ||
            type == typeof(Thickness) || type == typeof(CornerRadius) ||
            type == typeof(Rect))
        {
            return CreateTextBox(value?.ToString(), OnPropertyChanged);
        }
        else if (type == typeof(IBrush) || type == typeof(ImmutableSolidColorBrush) ||
                 type == typeof(Avalonia.Media.SolidColorBrush))
        {
            return CreateColorPanel(value);
        }
        else if (type.IsEnum)
        {
            return CreateEnumComboBox(type, value, OnEnumPropertyChanged);
        }
        else if (type == typeof(bool) || type == typeof(bool?))
        {
            return CreateCheckBox((bool?)value, OnBoolPropertyChanged);
        }
        else if (type == typeof(IImage))
        {
            return CreateImageButton(OnChooseImageClick);
        }
        else
        {
            return CreateTextBox(value?.ToString(), null, Brushes.Red);
        }
    }

    private static TextBox CreateTextBox(string text, EventHandler<KeyEventArgs> keyDownHandler = null,
        IBrush foreground = null)
    {
        var textBox = new TextBox
        {
            Text = text,
            Foreground = foreground ?? GetColor("#0ab076"),
            FontWeight = FontWeight.Normal,
            HorizontalAlignment = HorizontalAlignment.Right,
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(5, 0, 0, 0)
        };
        //  textBox.KeyDown += PropListkeyDownHandler;


        textBox.KeyDown += keyDownHandler;
        textBox.KeyDown += PropListkeyDownHandler;

        DockPanel.SetDock(textBox, Dock.Right);
        return textBox;
    }

    private static void PropListkeyDownHandler(object? sender, KeyEventArgs e)
    {
        var textBox = sender as TextBox;
        var listBoxItem = textBox.FindLogicalAncestorOfType<ListBoxItem>();
        var listBox = listBoxItem.FindLogicalAncestorOfType<ListBox>();
        if (listBox is null) return;
        int index = listBox.Items.IndexOf(listBoxItem);

        TextBox nextTextBox;
        switch (e.Key)
        {
            case Key.Down:
            case Key.Tab:
                index++;
                break;

            case Key.Up:
                index--;
                break;
        }

        ChangePropiertyFocus(index);
    }

    private static async void ChangePropiertyFocus(int index)
    {
        if (index < 0) return;
        if (index >= PropListBox.Items.Count) return;

        var container = PropListBox.Items[index] as ListBoxItem;
        var visualChild = container?.Content as Border;
        var dockPanel = visualChild?.Child as DockPanel;
        if (dockPanel.Children.Count < 2)
        {
            ChangePropiertyFocus(index++);
            return;
        }

        var control = dockPanel?.Children[1];
        if (control is not null)
            await Dispatcher.UIThread.InvokeAsync(() => control?.Focus(NavigationMethod.Unspecified));
    }

    private static StackPanel CreateColorPanel(object value)
    {
        var stackPanel = new StackPanel { Orientation = Orientation.Horizontal };

        if (value != null)
        {
            var textBlock = new TextBlock
            {
                Text = value.ToString(),
                VerticalAlignment = VerticalAlignment.Center,
                Foreground = GetColor("#0ab076"),
                FontWeight = FontWeight.Normal
            };

            var MyColorButton = new MyColorButton
            {
                Color = Color.Parse(value.ToString())
            };

            MyColorButton.PropertyChanged += OnColorChanged;

            stackPanel.HorizontalAlignment = HorizontalAlignment.Right;
            stackPanel.Children.Add(textBlock);
            stackPanel.Children.Add(MyColorButton);
        }

        DockPanel.SetDock(stackPanel, Dock.Right);
        return stackPanel;
    }

    private static ComboBox CreateEnumComboBox(Type enumType, object selectedValue,
        EventHandler<SelectionChangedEventArgs> selectionChangedHandler)
    {
        var comboBox = new ComboBox
        {
            Margin = new Thickness(0, 0, 0, 10),
            Foreground = GetColor("#0ab076"),
            FontWeight = FontWeight.Normal,
            HorizontalAlignment = HorizontalAlignment.Right,
            VerticalAlignment = VerticalAlignment.Center,
            VerticalContentAlignment = VerticalAlignment.Bottom,
            ItemTemplate = new FuncDataTemplate<string>((item, _) =>
                new TextBlock
                {
                    Text = item,
                    VerticalAlignment = VerticalAlignment.Center
                })
        };

        foreach (var value in Enum.GetValues(enumType))
        {
            comboBox.Items.Add(value.ToString());
        }

        comboBox.SelectedItem = selectedValue?.ToString();
        comboBox.SelectionChanged += selectionChangedHandler;

        DockPanel.SetDock(comboBox, Dock.Right);
        return comboBox;
    }

    private static CheckBox CreateCheckBox(bool? isChecked, EventHandler<RoutedEventArgs> checkedChangedHandler)
    {
        var checkBox = new CheckBox
        {
            IsChecked = isChecked,
            Width = 30,
            HorizontalAlignment = HorizontalAlignment.Right,
            Foreground = GetColor("#0ab076"),
            FontWeight = FontWeight.Normal,
            HorizontalContentAlignment = HorizontalAlignment.Right,
            VerticalAlignment = VerticalAlignment.Center,
            VerticalContentAlignment = VerticalAlignment.Bottom,
            Margin = new Thickness(5, 0, 0, 0)
        };

        checkBox.IsCheckedChanged += checkedChangedHandler;

        DockPanel.SetDock(checkBox, Dock.Right);
        return checkBox;
    }

    private static Button CreateImageButton(EventHandler<RoutedEventArgs> clickHandler)
    {
        var button = new Button
        {
            Content = "Выбрать",
            HorizontalAlignment = HorizontalAlignment.Right,
            Foreground = GetColor("#0ab076"),
            FontWeight = FontWeight.Normal,
            HorizontalContentAlignment = HorizontalAlignment.Right,
            VerticalAlignment = VerticalAlignment.Center,
            VerticalContentAlignment = VerticalAlignment.Bottom,
            Margin = new Thickness(5, 0, 0, 0)
        };

        button.Click += clickHandler;

        DockPanel.SetDock(button, Dock.Right);
        return button;
    }

    private static void OnPropertyChanged(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter) return;
        var textBox = (TextBox)sender;
        var propName = ((TextBlock)((DockPanel)textBox.Parent).Children[0]).Text;

        SetPropertyValue(propName, textBox.Text, textBox);
    }

    private static async void OnChooseImageClick(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFileDialog
        {
            Title = "Выберите изображение",
            AllowMultiple = false,
            Filters = new List<FileDialogFilter>
            {
                new FileDialogFilter
                {
                    Name = "Изображения",
                    Extensions = new List<string> { "png", "jpg", "jpeg", "gif", "bmp" }
                }
            }
        };

        var result = await dialog.ShowAsync(ProgramWindow.MainWindow._MainWindow);

        if (result != null && result.Length > 0)
        {
            var fileName = Path.GetFileName(result[0]);
            var targetFilePath = Path.Combine("assets", fileName);
            File.Copy(result[0], targetFilePath, true);

            var jImage = (jImage)HierarchyControl.Selected.Beholder.element;
            jImage.jImageSource = $@"assets/{fileName}";
            jImage.Source = new Bitmap(jImage.jImageSource);
        }
    }

    //Посылать делегат (выполнено)
    private static void SpecialPropertySet(string propName, string value)
    {
        var element = Workspace.movable as JControl;
        if (element == null)
            return;

        // Если строка может быть преобразована в число, передаем как int
        if (Int32.TryParse(value, out int intVal))
            element.SpecialSetDelegates[propName](element, new Property(intVal));
        else
            element.SpecialSetDelegates[propName](element, new Property(value));
    }

    private static void OnEnumPropertyChanged(object sender, SelectionChangedEventArgs e)
    {
        var comboBox = (ComboBox)sender;
        var propName = ((TextBlock)((DockPanel)comboBox.Parent).Children[0]).Text;
        Type propType;
        try
        {
            propType = HierarchyControl.Selected.Beholder.element.GetType().GetProperty(propName).PropertyType;
        }
        catch
        {
            SpecialPropertySet(propName, comboBox.SelectedItem.ToString());
            return;
        }

        var enumValue = Enum.Parse(propType, comboBox.SelectedItem.ToString());
        SetPropertyValue(propName, enumValue);
    }

    private static void OnColorChanged(object sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property == MyColorButton.ColorProperty)
        {
            var MyColorButton = (MyColorButton)sender;


            var propName = ((TextBlock)((DockPanel)MyColorButton.Parent.Parent).Children[0]).Text;
            var _color = MyColorButton.Color;
            var newColor = new SolidColorBrush(MyColorButton.Color);
            SetPropertyValue(propName, newColor);

            var textBlock = ((StackPanel)MyColorButton.Parent).Children[0] as TextBlock;

            textBlock.Text = MyColorButton.Color.ToString();
            MyColorButton = new MyColorButton();
            MyColorButton.Color = _color;
        }
    }

    private static void OnBoolPropertyChanged(object sender, RoutedEventArgs e)
    {
        var checkBox = (CheckBox)sender;
        var propName = ((TextBlock)((DockPanel)checkBox.Parent).Children[0]).Text;

        SetPropertyValue(propName, checkBox.IsChecked);
    }

    private static void SetPropertyValue(string propName, object value, TextBox textBox = null)
    {
        var element = HierarchyControl.Selected.Beholder.element;

        PropertyInfo? prop;
        try
        {
            prop = element.GetType().GetProperty(propName);
        }
        catch
        {
            SpecialPropertySet(propName, value.ToString());
            return;
        }

        if (prop == null)
        {
            SpecialPropertySet(propName, value.ToString());
            return;
        }

        object convertedValue = ConvertValue(prop.PropertyType, value);
        if (convertedValue == null && textBox != null)
        {
            textBox.Text = "Некорректное значение";
            textBox.Foreground = Brushes.Red;
            return;
        }

        var prevalue = prop.GetValue(element);
        History.AddHistoryItem(new History.Change(element, propName, prevalue));

        if (propName == "Name")
        {
            typeof(StyledElement)
                .GetField("_name", BindingFlags.NonPublic | BindingFlags.Instance)
                ?.SetValue(element, convertedValue);
            HierarchyControl.Selected.Header = (string)convertedValue;
        }
        else
        {
            //НАЙТИ РЕШЕНИЕ С ПОДАВЛЕНИЕМ ИСКЛЮЧЕНИЙ ПРИ НЕВЕРНОМ ВВОДЕ
            try
            {
                Dispatcher.UIThread.InvokeAsync(() =>
                {
                    try
                    {
                        prop.SetValue(element, convertedValue);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Exception caught in UI thread: {ex.Message}");
                        if (textBox != null)
                        {
                            textBox.Foreground = new SolidColorBrush(Color.Parse("#88F1FF"));
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception caught in main thread: {ex.Message}");
                if (textBox != null)
                {
                    textBox.Foreground = new SolidColorBrush(Color.Parse("#88F1FF"));
                }
            }
        }

        if (textBox != null)
        {
            textBox.Foreground = new SolidColorBrush(Color.Parse("#88F1FF"));
        }
    }

    private static object ConvertValue(Type targetType, object value)
    {
        try
        {
            if (targetType == typeof(int))
                return Convert.ToInt32(value);
            if (targetType == typeof(double))
                return Convert.ToDouble(((string)value).Replace('.', ','));
            if (targetType == typeof(IBrush))
                return new SolidColorBrush(Color.Parse((value.ToString())));
            if (targetType == typeof(Thickness))
            {
                var values = ((string)value).Split(',');
                return new Thickness(Convert.ToInt32(values[0]), Convert.ToInt32(values[1]),
                    Convert.ToInt32(values[2]), Convert.ToInt32(values[3]));
            }

            if (targetType == typeof(CornerRadius))
            {
                var values = ((string)value).Split(',');
                return new CornerRadius(Convert.ToInt32(values[0]), Convert.ToInt32(values[1]),
                    Convert.ToInt32(values[2]), Convert.ToInt32(values[3]));
            }

            if (targetType == typeof(Rect))
            {
                var values = ((string)value).Split(',');
                return new Rect(Convert.ToInt32(values[0]), Convert.ToInt32(values[1]),
                    Convert.ToInt32(values[2]), Convert.ToInt32(values[3]));
            }

            return value;
        }
        catch
        {
            return null;
        }
    }
}