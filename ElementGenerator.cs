using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.Controls.Primitives;
using Avalonia.Media.Imaging;

namespace Xamlade;

public partial class MainWindow
{
    public void GenerateElement(object? sender, RoutedEventArgs e)
    {
            if (!(selectedTreeItem.element is IChildContainer parent)) return;
    
            string typeName = ((Button)sender).Content.ToString();
            Type elementType = Type.GetType("Xamlade.j" + typeName);
            JControl element = (JControl)Activator.CreateInstance(elementType);
    
            element.Name = typeName + (i++);
            SetDefaultValues(element);
    
            element.PointerEntered += OnjControlPointerEntered;
            element.PointerExited += OnjControlPointerExited;
            element.Click += jElementClick;
            element.PointerPressed += OnjControlPressed;
            element.PointerReleased += OnjControlReleased;
            
            parent.AddChild(element);
            selectedTreeItem.Items.Add(new mTreeViewItem(element));
    }

    public static void SetDefaultValues(JControl element)
    {
        switch (element.Type)
        {
            case "Button":
            {
                ((jButton)element).Content = "Text";
                ((jButton)element).Background = Brushes.Blue;
                ((jButton)element).Foreground = Brushes.White;
                ((jButton)element).FontSize = 20;
            } 
                break;
            case "TextBox":
            {
                ((jTextBox)element).Background = Brushes.Transparent;
                ((jTextBox)element).Text = "Text";
                ((jTextBox)element).FontSize = 20;
                ((jTextBox)element).Cursor = new Cursor(StandardCursorType.Arrow);
                ((jTextBox)element).Foreground=Brushes.Blue;
                    
            } 
                break;
            case "TextBlock":
            {
                ((jTextBlock)element).Background = Brushes.Blue;
                ((jTextBlock)element).Text = "Text";
                ((jTextBlock)element).FontSize = 20;
                ((jTextBlock)element).Foreground = Brushes.White;
            } 
                break;
            case "Image":
            {
                
                ((jImage)element).Source = new Bitmap("assets/Xamlade.png");
                ((jImage)element).Width = 400;
                ((jImage)element).Height = 400;
                ((jImage)element).jImageSource = @"assets/Xamlade.png";
            } 
                break;
            case "ToggleButton":
            {
             //   ((jTextBlock)element).Background = Brushes.Blue;
                    ((ToggleButton)element).Content = "Text";
                    ((ToggleButton)element).IsChecked = false;
             //   ((jTextBlock)element).FontSize = 20;
             //   ((jTextBlock)element).Foreground = Brushes.White;
            } 
                break;
            case "CheckBox":
            {
                ((jCheckBox)element).Background = Brushes.Blue;
                ((jCheckBox)element).Content = "Text";
                ((jCheckBox)element).FontSize = 20;
                ((jCheckBox)element).Foreground = Brushes.White;
            }
                break;
            case "Canvas":
            {
                // Генерация случайного цвета в формате HEX
                string randomHexColor = $"#{random.Next(0x1000000):X6}";
                // Преобразование HEX строки в Color объект
                Color randomColor = Color.Parse(randomHexColor);
                ((jCanvas)element).Background = new SolidColorBrush(randomColor);
                ((jCanvas)element).Height = 400;
                ((jCanvas)element).Width = 400;
                
            }
                break;
            case "StackPanel":
            {
                // Генерация случайного цвета в формате HEX
                string randomHexColor = $"#{random.Next(0x1000000):X6}";
                // Преобразование HEX строки в Color объект
                Color randomColor = Color.Parse(randomHexColor);
                ((jStackPanel)element).Background = new SolidColorBrush(randomColor);
                ((jStackPanel)element).Height = 400;
                ((jStackPanel)element).Width = 400;
            }
                break;
            
        }
    }
}
