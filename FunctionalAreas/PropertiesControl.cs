using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
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

    public static void ShowProperties()
    {
        try
        {
            PropListItems?.Clear();
        }
        catch
        {
        }

        if (Equals(HierarchyControl.Selected.element, Workspace.MainCanvas)) return;

        var type = HierarchyControl.Selected.element.GetType();
        var props = type.GetProperties()
            .Where(prop => !Constants.ExcludedWords.Contains(prop.Name));

        foreach (var prop in props)
        {
            var propType = type.GetProperty(prop.Name).PropertyType;
            AddPropItem(prop.Name, prop.GetValue(HierarchyControl.Selected.element), propType);
        }

        AddSpecialProperties();
        AddContainerProperties();
        if (PropListBox != null)
        {
            typeof(ItemsControl)
                .GetField("_items", BindingFlags.NonPublic | BindingFlags.Instance)
                ?.SetValue(PropListBox, PropListItems);
        }
    }

    private static void AddSpecialProperties()
    {
        var obj = HierarchyControl.Selected.element;
        if (obj.jParent is null) return;

        var elementType = HierarchyControl.Selected.element.GetType();

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
                            Text = " " + "Специальные",
                            Foreground = GetColor("#9cd638"),
                            FontSize = 20,
                            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                            VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
                        }
                    }
                }
            }
        };
        PropListItems?.Add(listItem);


        switch (elementType.UnderlyingSystemType.Name)
        {
            case "jGrid":
                AddPropItem("Rows", (obj as jGrid).RowDefinitions.Count, typeof(int));
                AddPropItem("Columns", (obj as jGrid).ColumnDefinitions.Count, typeof(int));
                break;
            case "jComboBox" :
                SetAddButton();
                break;
            default:
                PropListItems?.Remove(listItem);
                break;
        }
    }

  
    private static void SetAddButton()
    {
        var obj = HierarchyControl.Selected.element;

        var button = new Button
        {
            Content = "Добавить элемент",
            Foreground = GetColor("#9cd638"),
            FontSize = 20,
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
            VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
        };
        if (obj is jComboBox comboBox)
        {
            var addItemListItem = new ListBoxItem
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
                            button
                        }
                    }
                }
            };
            button.AddHandler(Button.ClickEvent,
                (sender, e) =>
                {
                    var item = new jComboBoxItem(obj.Name + $"Item {comboBox.Items.Count}");
                    item.Click+= (_, _) =>
                    {
                        comboBox.SelectedItem = item;
                        var _obj = item.Content as Control;
                        if (_obj is not null)
                            Reflector.SetXYBoundsZero(_obj);
                    };

                    comboBox.AddChild(item);
                    HierarchyControl.Selected.Items.Add(item.mTreeItem);
                    comboBox.mTreeItem.IsExpanded = true;
                    var data = new Object[] { comboBox, item, item.mTreeItem };
                    History.AddHistoryItem(new History.Change(item, "Created", data));
                });
            PropListItems?.Add(addItemListItem);
        }
    }
    
    private static void AddContainerProperties()
    {
        var obj = HierarchyControl.Selected.element;
        if (obj.jParent is null) return;

        var ContainerType = HierarchyControl.Selected.element.jParent.GetType();

        var parentPropertiesListItem = new ListBoxItem
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
                            Text = " Родительский " + ContainerType.UnderlyingSystemType.Name.Substring(1),
                            Foreground = GetColor("#9cd638"),
                            FontSize = 20,
                            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                            VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
                        }
                    }
                }
            }
        };
        PropListItems?.Add(parentPropertiesListItem);


        

        


        switch (ContainerType.UnderlyingSystemType.Name)
        {
        
            case "jDockPanel":
                AddPropItem("Docked", DockPanel.GetDock(obj as Control), typeof(Dock));
                break;

            case "jCanvas":

                AddPropItem("Left", Double.IsNaN(Canvas.GetLeft(obj as Control)) ? 0 : Canvas.GetLeft(obj as Control),
                    typeof(double));
                AddPropItem("Top", Double.IsNaN(Canvas.GetTop(obj as Control)) ? 0 : Canvas.GetTop(obj as Control),
                    typeof(double));
                break;

            case "jGrid":
                var grid = obj.jParent as jGrid;
                AddPropItem("Row", Grid.GetRow(obj as Control), typeof(int));
                AddPropItem("Column", Grid.GetColumn(obj as Control), typeof(int));
                AddPropItem("RowSpan", Grid.GetRowSpan(obj as Control), typeof(int));
                AddPropItem("ColumnSpan", Grid.GetColumnSpan(obj as Control), typeof(int));
                AddPropItem("RowHeight", grid!.RowDefinitions[Grid.GetRow(obj as Control)].Height.Value,
                    typeof(double));
                AddPropItem("RowType", grid!.RowDefinitions[Grid.GetRow(obj as Control)].Height.GridUnitType,
                    typeof(GridUnitType));
                AddPropItem("ColumnWidth", grid!.ColumnDefinitions[Grid.GetColumn(obj as Control)].Width.Value,
                    typeof(double));
                AddPropItem("ColumnType", grid!.ColumnDefinitions[Grid.GetColumn(obj as Control)].Width.GridUnitType,
                    typeof(GridUnitType));
                break;

            case "jRelativePanel":
                AddPropItem("AlignLeftWithPanel", RelativePanel.GetAlignLeftWithPanel(obj as Control), typeof(bool));
                AddPropItem("AlignRightWithPanel", RelativePanel.GetAlignRightWithPanel(obj as Control), typeof(bool));
                AddPropItem("AlignTopWithPanel", RelativePanel.GetAlignTopWithPanel(obj as Control), typeof(bool));
                AddPropItem("AlignBottomWithPanel", RelativePanel.GetAlignBottomWithPanel(obj as Control),
                    typeof(bool));
                AddPropItem("AlignHorizontalCenterWithPanel",
                    RelativePanel.GetAlignHorizontalCenterWithPanel(obj as Control), typeof(bool));
                AddPropItem("AlignVerticalCenterWithPanel",
                    RelativePanel.GetAlignVerticalCenterWithPanel(obj as Control), typeof(bool));
                AddPropItem("LeftOf", RelativePanel.GetLeftOf(obj as Control), typeof(Control));
                AddPropItem("RightOf", RelativePanel.GetRightOf(obj as Control), typeof(Control));
                AddPropItem("Above", RelativePanel.GetAbove(obj as Control), typeof(Control));
                AddPropItem("Below", RelativePanel.GetBelow(obj as Control), typeof(Control));
                AddPropItem("AlignLeftWith", RelativePanel.GetAlignLeftWith(obj as Control), typeof(Control));
                AddPropItem("AlignRightWith", RelativePanel.GetAlignRightWith(obj as Control), typeof(Control));
                AddPropItem("AlignTopWith", RelativePanel.GetAlignTopWith(obj as Control), typeof(Control));
                AddPropItem("AlignBottomWith", RelativePanel.GetAlignBottomWith(obj as Control), typeof(Control));
                AddPropItem("AlignHorizontalCenterWith", RelativePanel.GetAlignHorizontalCenterWith(obj as Control),
                    typeof(Control));
                AddPropItem("AlignVerticalCenterWith", RelativePanel.GetAlignVerticalCenterWith(obj as Control),
                    typeof(Control));
                break;

            // Add cases for other panel types as needed
        }
    }

    //Вернуть private
    public static void AddPropItem(string name, object value, Type type)
    {
        if (name == "ColumnDefinitions" || name == "RowDefinitions") return;
        var listItem = new ListBoxItem
        {
            Content = new Border
            {
                BorderThickness = new Thickness(0, 0, 0, 1),
                BorderBrush = GetColor("#8897FF"),
                Child = CreatePropertyPanel(name, value, type)
            }
        };
        listItem.Name = name;

        PropListItems?.Add(listItem);
        //PropListItems?.Add(listItem);
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

            var jImage = (jImage)HierarchyControl.Selected.element;
            jImage.jImageSource = $@"assets/{fileName}";
            jImage.Source = new Bitmap(jImage.jImageSource);
        }
    }

    private static void SpecialPropertySet(string propName, string value)
    {
        var element = HierarchyControl.Selected.element as Control;
        if (element == null) return;

        switch (propName)
        {
            case "Rows":
            {
                var grid = element as jGrid;
                var rows = grid.RowDefinitions.Count;
                int.TryParse(value, out int newRows);

                if (rows == newRows)
                {
                    return;
                }

                if (newRows > rows)
                {
                    for (int i = rows; i < newRows; i++)
                    {
                        grid.RowDefinitions.Add(new mRowDefinition(grid, 100));
                    }
                }
                else
                {
                    // Создаем список строк с детьми
                    var rowsWithChildren = new List<int>();
                    foreach (var child in grid.Children)
                    {
                        int row = Grid.GetRow(child);
                        if (!rowsWithChildren.Contains(row))
                        {
                            rowsWithChildren.Add(row);
                        }
                    }

                    // Удаляем строки, не затрагивая строки с детьми
                    for (int i = rows - 1; i >= newRows; i--)
                    {
                        if (!rowsWithChildren.Contains(i))
                        {
                            grid.RowDefinitions.RemoveAt(i);
                        }
                    }
                }
            }
                break;
            case "Columns":
            {
                var grid = element as jGrid;
                var columns = grid.ColumnDefinitions.Count;
                int.TryParse(value, out int newColumns);

                if (columns == newColumns)
                {
                    return;
                }

                if (newColumns > columns)
                {
                    for (int i = columns; i < newColumns; i++)
                    {
                        grid.ColumnDefinitions.Add(new mColumnDefinition(grid, 100));
                    }
                }
                else
                {
                    // Создаем список колонок с детьми
                    var columnsWithChildren = new List<int>();
                    foreach (var child in grid.Children)
                    {
                        int column = Grid.GetColumn(child);
                        if (!columnsWithChildren.Contains(column))
                        {
                            columnsWithChildren.Add(column);
                        }
                    }
                    
                    for (int i = columns - 1; i >= newColumns; i--)
                    {
                        if (!columnsWithChildren.Contains(i))
                        {
                            grid.ColumnDefinitions.RemoveAt(i);
                        }
                    }
                }
                
            }
                break;
        }
    }

    private static void ContainerPropertySet(string propName, string value)
    {
        var element = HierarchyControl.Selected.element;
        if (element == null) return;

        switch (propName)
        {
            case "Docked":
                if (Enum.TryParse(value, out Dock dock))
                {
                    jDockPanel.SetDock(element, dock);
                }

                break;

            case "Left":
                if (double.TryParse(value, out double left))
                {
                    jCanvas.SetLeft(element, left);
                }

                break;

            case "Top":
                if (double.TryParse(value, out double top))
                {
                    jCanvas.SetTop(element, top);
                }

                break;

            case "Right":
                if (double.TryParse(value, out double right))
                {
                    jCanvas.SetRight(element, right);
                }

                break;

            case "Bottom":
                if (double.TryParse(value, out double bottom))
                {
                    jCanvas.SetBottom(element, bottom);
                }

                break;

            case "Row":
                if (int.TryParse(value, out int row))
                {
                    jGrid.SetRow(element, row);
                }

                break;

            case "Column":
                if (int.TryParse(value, out int column))
                {
                    jGrid.SetColumn(element, column);
                }

                break;

            case "RowSpan":
                if (int.TryParse(value, out int rowSpan))
                {
                    jGrid.SetRowSpan(element, rowSpan);
                }

                break;

            case "ColumnSpan":
                if (int.TryParse(value, out int columnSpan))
                {
                    jGrid.SetColumnSpan(element, columnSpan);
                }

                break;
            case "RowHeight":
            {
                var grid = element.jParent as jGrid;
                if (double.TryParse(value, out double height))
                {
                    var _Height = grid!.RowDefinitions[jGrid.GetRow(element)].Height;
                    grid!.RowDefinitions[jGrid.GetRow(element)].Height = new GridLength(height, _Height.GridUnitType);
                }
            }
                break;
            case "ColumnWidth":
            {
                var grid = element.jParent as Grid;
                if (double.TryParse(value, out double width))
                {
                    var _Width = grid!.ColumnDefinitions[jGrid.GetRow(element)].Width;
                    grid!.RowDefinitions[jGrid.GetRow(element)].Height = new GridLength(width, _Width.GridUnitType);
                }
            }
                break;
            case "RowType":
            {
                var grid = element.jParent as Grid;
                if (Enum.TryParse(value, out GridUnitType unitType))
                {
                    var _Height = grid!.RowDefinitions[jGrid.GetRow(element)].Height;
                    grid!.RowDefinitions[jGrid.GetRow(element)].Height = new GridLength(_Height.Value, unitType);
                }
            }
                break;

            case "ColumnType":
            {
                var grid = element.jParent as jGrid;
                if (Enum.TryParse(value, out GridUnitType unitType))
                {
                    var _Width = grid!.ColumnDefinitions[jGrid.GetColumn(element)].Width;
                    grid!.ColumnDefinitions[jGrid.GetColumn(element)].Width = new GridLength(_Width.Value, unitType);
                }
            }
                break;


           /* case "AlignLeftWithPanel":
                if (bool.TryParse(value, out bool alignLeftWithPanel))
                {
                    RelativePanel.SetAlignLeftWithPanel(element, alignLeftWithPanel);
                }

                break;

            case "AlignRightWithPanel":
                if (bool.TryParse(value, out bool alignRightWithPanel))
                {
                    RelativePanel.SetAlignRightWithPanel(element, alignRightWithPanel);
                }

                break;

            case "AlignTopWithPanel":
                if (bool.TryParse(value, out bool alignTopWithPanel))
                {
                    RelativePanel.SetAlignTopWithPanel(element, alignTopWithPanel);
                }

                break;

            case "AlignBottomWithPanel":
                if (bool.TryParse(value, out bool alignBottomWithPanel))
                {
                    RelativePanel.SetAlignBottomWithPanel(element, alignBottomWithPanel);
                }

                break;

            case "AlignHorizontalCenterWithPanel":
                if (bool.TryParse(value, out bool alignHorizontalCenterWithPanel))
                {
                    RelativePanel.SetAlignHorizontalCenterWithPanel(element, alignHorizontalCenterWithPanel);
                }

                break;

            case "AlignVerticalCenterWithPanel":
                if (bool.TryParse(value, out bool alignVerticalCenterWithPanel))
                {
                    RelativePanel.SetAlignVerticalCenterWithPanel(element, alignVerticalCenterWithPanel);
                }

                break;

            // Additional cases for other properties related to RelativePanel
            case "LeftOf":
            case "RightOf":
            case "Above":
            case "Below":
            case "AlignLeftWith":
            case "AlignRightWith":
            case "AlignTopWith":
            case "AlignBottomWith":
            case "AlignHorizontalCenterWith":
            case "AlignVerticalCenterWith":
                //ИСПРАИТЬ
                var relativeElement = new Control(); ///
                if (relativeElement != null)
                {
                    switch (propName)
                    {
                        case "LeftOf":
                            RelativePanel.SetLeftOf(element, relativeElement);
                            break;
                        case "RightOf":
                            RelativePanel.SetRightOf(element, relativeElement);
                            break;
                        case "Above":
                            RelativePanel.SetAbove(element, relativeElement);
                            break;
                        case "Below":
                            RelativePanel.SetBelow(element, relativeElement);
                            break;
                        case "AlignLeftWith":
                            RelativePanel.SetAlignLeftWith(element, relativeElement);
                            break;
                        case "AlignRightWith":
                            RelativePanel.SetAlignRightWith(element, relativeElement);
                            break;
                        case "AlignTopWith":
                            RelativePanel.SetAlignTopWith(element, relativeElement);
                            break;
                        case "AlignBottomWith":
                            RelativePanel.SetAlignBottomWith(element, relativeElement);
                            break;
                        case "AlignHorizontalCenterWith":
                            RelativePanel.SetAlignHorizontalCenterWith(element, relativeElement);
                            break;
                        case "AlignVerticalCenterWith":
                            RelativePanel.SetAlignVerticalCenterWith(element, relativeElement);
                            break;
                    }
                }

                break;*/

            default:
                SpecialPropertySet(propName, value);
                break;
        }
    }

    private static void OnEnumPropertyChanged(object sender, SelectionChangedEventArgs e)
    {
        var comboBox = (ComboBox)sender;
        var propName = ((TextBlock)((DockPanel)comboBox.Parent).Children[0]).Text;
        Type propType;
        try
        {
            propType = HierarchyControl.Selected.element.GetType().GetProperty(propName).PropertyType;
        }
        catch
        {
            ContainerPropertySet(propName, comboBox.SelectedItem.ToString());
            return;
        }

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

        PropertyInfo? prop;
        try
        {
            prop = element.GetType().GetProperty(propName);
        }
        catch
        {
            ContainerPropertySet(propName, value.ToString());
            return;
        }

        if (prop == null)
        {
            ContainerPropertySet(propName, value.ToString());
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