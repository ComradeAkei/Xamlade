using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
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
        if ((HierarchyControl.Selected.element is jComboBox)) return;
        if (!(HierarchyControl.Selected.element is JChildContainer parent)) return;


        var elementType = Type.GetType("Xamlade.jClasses.j" + typeName);
        var element = (JControl)Activator.CreateInstance(elementType);

        element.Name = typeName + (Utils.NextgenIterator++);
        SetDefaultValues(element, parent as JControl);
        element.PointerEntered += Workspace.OnjControlPointerEntered;
        element.PointerExited += Workspace.OnjControlPointerExited;
        //  element.Click += jElementClick;
        element.PointerPressed += Workspace.OnjControlPressed;
        element.PointerReleased += Workspace.OnjControlReleased;
        //  if (element is jComboBox comboBox)
        //  comboBox.

        parent.AddChild(element);
        var item = new mTreeViewItem(element);
        HierarchyControl.Selected.Items.Add(item);
        (((JControl)(item.element.jParent))!).mTreeItem.IsExpanded = true;
        var data = new Object[] { parent, element, element.mTreeItem };
        History.AddHistoryItem(new History.Change(element, "Created", data));
        InitSelectionBorder(element as JSelectable);
    }

    //Бордюр это отдельный прикол в Avalonia
    //Для выделения
    private static void GenerateBorders()
    {
        if(HierarchyControl.Selected is jComboBoxItem) return;
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
        SetDefaultBorderValues(element);
        element.PointerEntered += Workspace.OnjControlPointerEntered;
        element.PointerExited += Workspace.OnjControlPointerExited;
        element.PointerPressed += Workspace.OnjControlPressed;
        element.PointerReleased += Workspace.OnjControlReleased;
        (element as jBorder).AddChild(obj);
        InitSelectionBorder(element as JSelectable);
    }

    private static void SetDefaultBorderValues(JControl element)
    {
        string randomHexColor = $"#{Utils.random.Next(0x1000000):X6}";
        var borderColor = Color.Parse(randomHexColor);
        var border = (jBorder)element;
        border.BorderBrush = new SolidColorBrush(borderColor);
        border.Background = new SolidColorBrush(borderColor);
        border.BorderThickness = new Thickness(5);
    }

    public static void SetDefaultValues(JControl element, JControl? parent)
    {
        // Получаем размеры родительского элемента
        double parentWidth = parent.Bounds.Width;
        double parentHeight = parent.Bounds.Height;


        // Вычисляем максимальные размеры для дочерних элементов (70% от размеров родителя)
        double maxWidth = parentWidth * 0.7;
        double maxHeight = parentHeight * 0.7;
        if (parent is jGrid _grid)
        {
            maxWidth = parentWidth / _grid.ColumnDefinitions.Count / _grid.RowDefinitions.Count;
            maxHeight = parentHeight / _grid.ColumnDefinitions.Count / _grid.RowDefinitions.Count;
        }


        switch (element.Type)
        {
            case "Grid":
            {
                var grid = element as jGrid;
                string randomHexColor = $"#{Utils.random.Next(0x1000000):X6}";
                var randomColor = Color.Parse(randomHexColor);

                grid.Background = new SolidColorBrush(randomColor);
                grid.Width = Math.Min(400, maxWidth); // Устанавливаем ширину
                grid.Height = Math.Min(400, maxHeight); // Устанавливаем высоту
                grid.ShowGridLines = true;

                grid.RowDefinitions.Add(new mRowDefinition(grid, 100));
                grid.RowDefinitions.Add(new mRowDefinition(grid, 100));
                grid.ColumnDefinitions.Add(new mColumnDefinition(grid, 100));
                grid.ColumnDefinitions.Add(new mColumnDefinition(grid, 100));
            }
                break;
            case "DockPanel":
            {
                string randomHexColor = $"#{Utils.random.Next(0x1000000):X6}";
                var randomColor = Color.Parse(randomHexColor);
                var dockPanel = (jDockPanel)element;

                dockPanel.Background = new SolidColorBrush(randomColor);
                dockPanel.Width = Math.Min(400, maxWidth); // Устанавливаем ширину
                dockPanel.Height = Math.Min(400, maxHeight); // Устанавливаем высоту
            }
                break;
            case "ComboBox":
            {
                string randomHexColor = $"#{Utils.random.Next(0x1000000):X6}";
                var randomColor = Color.Parse(randomHexColor);
                var comboBox = (jComboBox)element;

                comboBox.Background = new SolidColorBrush(randomColor);
                comboBox.Width = Math.Min(120, maxWidth); // Устанавливаем ширину
                comboBox.Height = Math.Min(50, maxHeight); // Устанавливаем высоту
                comboBox.IsDropDownOpen = false;
            }
                break;
            case "Button":
            {
                var button = (jButton)element;
                button.Content = "Text";
                button.Background = Brushes.Blue;
                button.Foreground = Brushes.White;
                button.FontSize = 20;
                button.HorizontalAlignment = HorizontalAlignment.Center;
            }
                break;
            case "TextBox":
            {
                var textBox = (jTextBox)element;
                textBox.Background = Brushes.Transparent;
                textBox.Text = "Text";
                textBox.FontSize = 20;
                textBox.Cursor = new Cursor(StandardCursorType.Arrow);
                textBox.Foreground = Brushes.Blue;
            }
                break;
            case "TextBlock":
            {
                var textBlock = (jTextBlock)element;
                textBlock.Background = Brushes.Blue;
                textBlock.Text = "Text";
                textBlock.FontSize = 20;
                textBlock.Foreground = Brushes.White;
            }
                break;
            case "Image":
            {
                var image = (jImage)element;
                image.Source = new Bitmap("assets/Xamlade.png");
                image.Width = Math.Min(400, maxWidth); // Устанавливаем ширину
                image.Height = Math.Min(400, maxHeight); // Устанавливаем высоту
                image.jImageSource = @"assets/Xamlade.png";
            }
                break;
            case "ToggleButton":
            {
                var toggleButton = (ToggleButton)element;
                toggleButton.Content = "Text";
                toggleButton.IsChecked = false;
            }
                break;
            case "CheckBox":
            {
                var checkBox = (jCheckBox)element;
                checkBox.Background = Brushes.Blue;
                checkBox.Content = "Text";
                checkBox.FontSize = 20;
                checkBox.Foreground = Brushes.White;
            }
                break;
            case "Canvas":
            {
                string randomHexColor = $"#{Utils.random.Next(0x1000000):X6}";
                var randomColor = Color.Parse(randomHexColor);
                var canvas = (jCanvas)element;

                canvas.Background = new SolidColorBrush(randomColor);
                canvas.Width = Math.Min(400, maxWidth); // Устанавливаем ширину
                canvas.Height = Math.Min(400, maxHeight); // Устанавливаем высоту
            }
                break;
            case "StackPanel":
            {
                string randomHexColor = $"#{Utils.random.Next(0x1000000):X6}";
                var randomColor = Color.Parse(randomHexColor);
                var stackPanel = (jStackPanel)element;

                stackPanel.Background = new SolidColorBrush(randomColor);
                stackPanel.Width = Math.Min(400, maxWidth); // Устанавливаем ширину
                stackPanel.Height = Math.Min(400, maxHeight); // Устанавливаем высоту
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