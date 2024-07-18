using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Xamlade.Extensions;
using Xamlade.jClasses;

namespace Xamlade.FunctionalAreas;

public static class ElementGenerator
{
    public static void GenerateElement(object? sender, RoutedEventArgs e)
    {
            string typeName = ((Button)sender).Content.ToString();
            if (typeName == "Border")
            {
                GenerateBorders();
                return;
            }
            if ((HierarchyControl.Selected.element is jBorder)) return;
            if (!(HierarchyControl.Selected.element is JChildContainer parent)) return;
    
            
            var elementType = Type.GetType("Xamlade.jClasses.j" + typeName);
            var element = (JControl)Activator.CreateInstance(elementType);
    
            element.Name = typeName + (Utils.NextgenIterator++);
            SetDefaultValues(element);
            element.PointerEntered += Workspace.OnjControlPointerEntered;
            element.PointerExited += Workspace.OnjControlPointerExited;
          //  element.Click += jElementClick;
            element.PointerPressed += Workspace.OnjControlPressed;
            element.PointerReleased += Workspace.OnjControlReleased;
            
            parent.AddChild(element);
            var item = new mTreeViewItem(element);
            HierarchyControl.Selected.Items.Add(item);
            (((JControl)(item.element.jParent))!).mTreeItem.IsExpanded = true;
            var data = new Object[] {parent,element,element.mTreeItem};
            History.AddHistoryItem(new History.Change(element,"Created",data));
            InitSelectionBorder(element as JSelectable);
    }

    //Бордюр это отдельный прикол в Avalonia
    //Для выделения
    private static void GenerateBorders()
    {
       
        
        if (Workspace.SelectionCanvas.jChildren.Count > 1)
        {
            var collection = Workspace.SelectionCanvas.jChildren.ToList();
            Workspace.RestoreSelectionCanvas();
            foreach (var child in collection)
                GenerateBorder(child);
        }
        else
        {
            if (Workspace.movable is null) return;
            if (Workspace.movable.Name == "MainCanvas") return;
            if (HierarchyControl.Selected.element.jParent is jBorder) return;
            GenerateBorder(Workspace.movable);
        }
    }

    //Бордюр для конкретного объекта
    private static void GenerateBorder(JControl obj)
    {
        var elementType = Type.GetType("Xamlade.jClasses.jBorder");
        var element = (JControl)Activator.CreateInstance(elementType);
        element.Name = "jBorder" + (Utils.NextgenIterator++);
        SetDefaultValues(element);
        element.PointerEntered += Workspace.OnjControlPointerEntered;
        element.PointerExited += Workspace.OnjControlPointerExited;
        element.PointerPressed += Workspace.OnjControlPressed;
        element.PointerReleased += Workspace.OnjControlReleased;
        (element as jBorder).AddChild(obj);
        InitSelectionBorder(element as JSelectable);
    }

    public static void SetDefaultValues(JControl element)
    {
        switch (element.Type)
        {
            
            case "DockPanel":
            {
                // Генерация случайного цвета в формате HEX
                string randomHexColor = $"#{Utils.random.Next(0x1000000):X6}";
                // Преобразование HEX строки в Color объект
                var randomColor = Color.Parse(randomHexColor);
                ((jDockPanel)element).Background = new SolidColorBrush(randomColor);
                ((jDockPanel)element).Height = 400;
                ((jDockPanel)element).Width = 400;
            } 
                break;
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
                string randomHexColor = $"#{Utils.random.Next(0x1000000):X6}";
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
                string randomHexColor = $"#{Utils.random.Next(0x1000000):X6}";
                // Преобразование HEX строки в Color объект
                Color randomColor = Color.Parse(randomHexColor);
                ((jStackPanel)element).Background = new SolidColorBrush(randomColor);
                ((jStackPanel)element).Height = 400;
                ((jStackPanel)element).Width = 400;
            }
                break;
            case "Border":
            {
                // Генерация случайного цвета в формате HEX
                string randomHexColor = $"#{Utils.random.Next(0x1000000):X6}";
                var obj = element as jBorder;
                
                obj.BorderBrush = new SolidColorBrush(Color.Parse(randomHexColor));
                obj.Background = new SolidColorBrush(Color.Parse(randomHexColor));
                obj.BorderThickness = new Thickness(5);
            }
                break;

        }
        
    }

    public static void InitSelectionBorder(JSelectable obj)
    {
        obj.selectionBorder = new mBorder(obj as JControl);
        obj.selectionBorder.Background = Brushes.Transparent;
        obj.selectionBorder.BorderBrush = new SolidColorBrush(Color.Parse("#1D9627"));
        obj.selectionBorder.BorderThickness = new Thickness(2);
        Workspace.MainCanvas.Children.Add(obj.selectionBorder);
        obj.selectionBorder.SetValue(Panel.ZIndexProperty, Int32.MaxValue);
        obj.selectionBorder.IsVisible = false;
    }
}
