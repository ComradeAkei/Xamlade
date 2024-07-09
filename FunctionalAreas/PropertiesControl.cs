using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
using Xamlade.Extensions;
using Xamlade.jClasses;
// ReSharper disable All

namespace Xamlade.FunctionalAreas;

public static class PropertiesControl
    {
        public static ListBox PropListBox { get; set; }
        private static ItemCollection PropListItems;

        public static void Init(ListBox propListBox)
        {
            PropListBox = propListBox;
        
            // Использование рефлексии для создания экземпляра ItemCollection
            var constructor = typeof(ItemCollection).GetConstructor(
                BindingFlags.Instance | BindingFlags.NonPublic,
                null,
                Type.EmptyTypes,
                null);
        
            PropListItems = (ItemCollection)constructor.Invoke(new object[] { });
        }

        private static SolidColorBrush GetColor(string color) => new(Color.Parse(color));

        public static void ShowProperties()
        {
            PropListItems?.Clear();

            if (Equals(HierarchyControl.Selected.element, Workspace.MainCanvas)) return;

            var type = HierarchyControl.Selected.element.GetType();
            var props = type.GetProperties()
                .Where(prop => !Constants.ExcludedWords.Contains(prop.Name));

            foreach (var prop in props)
            {
                var propType = type.GetProperty(prop.Name).PropertyType;
                AddPropItem(prop.Name, prop.GetValue(HierarchyControl.Selected.element), propType);
            }

            if (PropListBox != null)
            {
                typeof(ItemsControl)
                    .GetField("_items", BindingFlags.NonPublic | BindingFlags.Instance)
                    ?.SetValue(PropListBox, PropListItems);
            }
        }

        private static void AddPropItem(string name, object value, Type type)
        {
            var listItem = new ListBoxItem
            {
                Content = new Border
                {
                    BorderThickness = new Thickness(0, 0, 0, 1),
                    BorderBrush = GetColor("#8897FF"),
                    Child = CreatePropertyPanel(name, value, type)
                }
            };

            PropListItems?.Add(listItem);
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
            else if (type == typeof(IBrush))
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

        private static TextBox CreateTextBox(string text, EventHandler<KeyEventArgs> keyDownHandler = null, IBrush foreground = null)
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

            if (keyDownHandler != null)
            {
                textBox.KeyDown += keyDownHandler;
            }

            DockPanel.SetDock(textBox, Dock.Right);
            return textBox;
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

                var colorButton = new ColorButton
                {
                    Color = Color.Parse(value.ToString())
                };
                colorButton.PropertyChanged += OnColorChanged;

                stackPanel.HorizontalAlignment = HorizontalAlignment.Right;
                stackPanel.Children.Add(textBlock);
                stackPanel.Children.Add(colorButton);
            }

            DockPanel.SetDock(stackPanel, Dock.Right);
            return stackPanel;
        }

        private static ComboBox CreateEnumComboBox(Type enumType, object selectedValue, EventHandler<SelectionChangedEventArgs> selectionChangedHandler)
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

                var jImage = (jImage)HierarchyControl.Selected.element;
                jImage.jImageSource = $@"assets/{fileName}";
                jImage.Source = new Bitmap(jImage.jImageSource);
            }
        }

        private static void OnEnumPropertyChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = (ComboBox)sender;
            var propName = ((TextBlock)((DockPanel)comboBox.Parent).Children[0]).Text;
            var propType = HierarchyControl.Selected.element.GetType().GetProperty(propName).PropertyType;
            
            var enumValue = Enum.Parse(propType, comboBox.SelectedItem.ToString());
            SetPropertyValue(propName, enumValue);
        }

        private static void OnColorChanged(object sender, AvaloniaPropertyChangedEventArgs e)
        {
            if (e.Property == ColorButton.ColorProperty)
            {
                var colorButton = (ColorButton)sender;
                var propName = ((TextBlock)((DockPanel)colorButton.Parent.Parent).Children[0]).Text;
                
                var newColor = new SolidColorBrush(colorButton.Color);
                SetPropertyValue(propName, newColor);

                var textBlock = ((StackPanel)colorButton.Parent).Children[0] as TextBlock;
                if (textBlock != null)
                {
                    textBlock.Text = colorButton.Color.ToString();
                }
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
            var element = HierarchyControl.Selected.element;
            var prop = element.GetType().GetProperty(propName);

            if (prop == null) return;

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
                prop.SetValue(element, convertedValue);
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
                    return new SolidColorBrush(Color.Parse((string)value));
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