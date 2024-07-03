using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Gif;
using Avalonia.Media.Imaging;
using AvaloniaColorPicker;
using Avalonia.Markup;
using Microsoft.Diagnostics.Runtime;

namespace Xamlade;



public partial class MainWindow : Window
{
    public static MainWindow _MainWindow;
    public string SelectedGif => @"avares://Xamlade/RES/loading.gif";


    #region Globals

    //Отладочный итератор
    private int i = 0;

    //Случайное число
    private static Random random;


    //Перемещаемый по холсту объект
    private JControl movable;
    
    //Кандидат на перемещение
    private JControl premovable;

    //Выбранный в дереве элемент
    private mTreeViewItem? selectedTreeItem;


    //Оригинальный фон выбранного элемента
    private IBrush? selectedOriginalBackground;

    // Половина ширины перемещаемого элемента
    private double mov_hw;

    // Половина высоты перемещаемого элемента 
    private double mov_hh;

    #endregion

    
    public MainWindow()
    {
        _MainWindow = this;
        //
        InitializeComponent();
        this.Icon = new WindowIcon(@"assets/Icon.png");
        DataContext = this;
        WindowState = WindowState.Maximized;
        selectedOriginalBackground = MainCanvas.Background;
        MainHierarchyTree.Items.Add(new mTreeViewItem(MainCanvas));
        selectedTreeItem = MainCanvas.mTreeItem;
        MainHierarchyTree.SelectedItem = selectedTreeItem;
        random = new Random();
        PropListItems = constructor.Invoke(new object[] { }) as ItemCollection;
        var listener = new GlobalKeyListener(this);
        listener.KeyPressed += GlobalKeyPressed;
        listener.KeyReleased += GlobalKeyReleased;

    }
    

    private void GlobalKeyPressed(KeyEventArgs e)
    {
        if (e.Key == Key.LeftCtrl)
            LCtrlPressed = true;
    }
    private void GlobalKeyReleased(KeyEventArgs e)
    {
        if (e.Key == Key.LeftCtrl)
            LCtrlPressed = false;
        else if(e.Key == Key.Delete)
            RemovejElement(null,null);
    }

    
    
    private void jCanvas_OnPointerMoved(object? sender, PointerEventArgs e)
    {
        if (movable == null || Equals((JControl)sender!, movable) || Equals(movable, MainCanvas))
            return;

        e.Handled = true;

        if (movable.jParent is not jCanvas) return;

        var parentCanvas = movable.jParent as jCanvas;
        if (e.GetCurrentPoint(parentCanvas).Properties.PointerUpdateKind != PointerUpdateKind.Other)
            return;

        Point mousePosition = e.GetPosition(parentCanvas);
        var element = movable as Control;

        if (movable.IsPressed && !LCtrlPressed)
        {
            Canvas.SetLeft(element, CorrectCoords(mousePosition.X - mov_hw));
            Canvas.SetTop(element, CorrectCoords(mousePosition.Y - mov_hh));

            if (Canvas.GetLeft(element) < 0)
                Canvas.SetLeft(element, 0);

            if (Canvas.GetTop(element) < 0)
                Canvas.SetTop(element, 0);

            if (Canvas.GetLeft(element) + 2 * mov_hw > parentCanvas.Bounds.Width)
                Canvas.SetLeft(element, parentCanvas.Bounds.Width - 2 * mov_hw);

            if (Canvas.GetTop(element) + 2 * mov_hh > parentCanvas.Bounds.Height)
                Canvas.SetTop(element, parentCanvas.Bounds.Height - 2 * mov_hh);
        }
        else if (movable.IsPressed && LCtrlPressed)
        {
            if (ResizeFlag)
            {
                UndoList.Remove(UndoList.Last());
                AddHistoryItem(new Change(movable, "Size", new Size(element.Bounds.Width, element.Bounds.Height)));
            }

            ResizeFlag = false;
            if (double.IsNaN(element.Width))
                element.Width = element.Bounds.Width;
            mousePosition = e.GetPosition(element);
            if(mousePosition.X<5 || mousePosition.Y<5)
                return;
            element.Width = CorrectSize(mousePosition.X);
            element.Height = CorrectSize(mousePosition.Y);
        }
    }


    private void InitMovable(JControl obj)
    {
        if (obj is null) return;
        AddHistoryItem(new Change(obj, 
            "Coordinates",
            new Coordinates(Canvas.GetLeft(obj as Control),Canvas.GetTop(obj as Control))));
        movable = obj;
        mov_hw = obj.Bounds.Width / 2;
        mov_hh = obj.Bounds.Height / 2;
    }

    public void InitPremovable()
    {
        if(premovable is null) return;
        InitMovable(premovable);
        MainHierarchyTree.SelectedItem = (premovable).mTreeItem;
        
    }


    private void XAMLIZE(object? sender, RoutedEventArgs? e)
    {
        ((Button)sender).Content = "   XAMLize   ";

        // selectedTreeItem.element.Background = selectedOriginalBackground;
        Broadcast.InitXAML();
        while (MainCanvas.XAMLRating > -1)
        {
            Broadcast.XAMLize();
        }

        string filePath = @"XamladeDemo/MainWindow.axaml";
        var outputXAML = new List<string>();
        outputXAML.Add(@"<Window xmlns=""https://github.com/avaloniaui""
         xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
         xmlns:d=""http://schemas.microsoft.com/expression/blend/2008""
         xmlns:mc=""http://schemas.openxmlformats.org/markup-compatibility/2006""
         mc:Ignorable=""d"" Width=""700"" Height=""600""
         x:Class=""XamladeDemo.MainWindow""
         Title=""TestWindow"">");
        outputXAML.AddRange(MainCanvas.XAMLPiece);
        outputXAML.Add(@"</Window>");
        File.WriteAllLines(filePath, outputXAML);
    }


    public static readonly List<string> ExcludedWords = new()
    {
        "jParent", "mTreeItem", "Presenter", "Template", "IsLoaded",
        "DesiredSize", "IsMeasureValid", "IsArrangeValid", "RenderTransform",
        "DataContext", "IsInitialized", "Parent", "ActualThemeVariant",
        "Transitions", "Item", "Type", "IsPressed", "ClickMode", "IsDefault", "IsCancel",
        "DataTemplates", "Focusable", "IsEnabled", "IsKeyboardFocusWithin",
        "IsFocused", "IsHitTestVisible", "IsPointerOver", "IsTabStop",
        "IsEffectivelyEnabled", "TabIndex", "KeyBindings", "GestureRecognizers",
        "UseLayoutRounding", "ClipToBounds", "IsEffectivelyVisible",
        "HasMirrorTransform", "RenderTransformOrigin", "ZIndex", "Classes",
        "Styles", "StyleKey", "Resources", "Command", "HotKey",
        "CommandParameter", "Flyout", "Theme", "Clip", "TemplatedParent", "Effect",
        "OpacityMask", "Bounds", "Cursor", "Tag", "ContextFlyout", "ContextMenu", "FocusAdorner", "IsItemsHost",
        "Children", "jChildren", "FontFamily", "TextDecoration", "ContentTemplate", "FlowDirection", "Inlines",
        "TextLayout",
        "XAMLRating", "XAMLPiece", "CanPaste", "CanUndo","jImageSource"
    };

    private void ShowProperties()
    {
        try
        {
            PropListItems.Clear();
        }
        catch
        {
            return;
        }


        if (selectedTreeItem.element == MainCanvas) return;
        //  Type type = selectedTreeItem.element.GetType();
        //   = type.GetProperties();

        Type type = selectedTreeItem.element.GetType();
        var props = type.GetProperties();
    
        foreach (var prop in props)
        {
            if (!ExcludedWords.Contains(prop.Name))
            {
                var prop_type = type.GetProperty(prop.Name).PropertyType;
                AddPropItem(prop.Name, prop.GetValue(selectedTreeItem.element), prop_type);
                //KeyValueList.Add(new KeyValue { Key = prop.Name, Value = prop.GetValue(selectedTreeItem.element)?.ToString() });
            }
        }

        FieldInfo privateField =
            typeof(ItemsControl).GetField("_items", BindingFlags.NonPublic | BindingFlags.Instance);
        privateField.SetValue(PropListBox, PropListItems);
    }

    // Выбрать редактируемый элемент
    private void SelectjElement(JControl element)
    {
        if ((element is null) || (element.Name == null)) return;
        selectedTreeItem = element.mTreeItem;
        MainHierarchyTree.SelectedItem = selectedTreeItem;
        selectedOriginalBackground = selectedTreeItem.element.Background;
        InitMovable(selectedTreeItem.element);
        selectedTreeItem.element.Focus();
        ShowProperties();
    }


    private void MainHierarchyTree_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        var item = ((TreeView)sender).SelectedItem as mTreeViewItem;
        //     if(item == selectedTreeItem) return;
        if (item != null)
            SelectjElement(item.element);
        else
            SelectjElement(MainCanvas);
        e.Handled = true;
    }

    private void jElementClick(object? sender, RoutedEventArgs e)
    {
        e.Handled = true;
        InitMovable((JControl)sender);
        MainHierarchyTree.SelectedItem = ((JControl)sender).mTreeItem;
    }

    private void OnjControlPointerEntered(object? sender, PointerEventArgs e)
    {
        e.Handled = true;
      //  Console.WriteLine(((JControl)sender).Type);
        if (!((JControl)sender).Type.Contains("Button")) return;
      //  Console.WriteLine("Ok");
        premovable = sender as JControl;
     //   InitMovable((JControl)sender);
       // var element = sender as JControl;
       // MainHierarchyTree.SelectedItem = (element).mTreeItem;
    }

    private void OnjControlPointerExited(object? sender, PointerEventArgs e)
    {
        e.Handled = true;
        premovable = null;
    }

    
    
    private void RemovejElement(object? sender, RoutedEventArgs? e)
    {
        if (selectedTreeItem == MainCanvas.mTreeItem) return;
        var element = selectedTreeItem.element;
        var jparent = selectedTreeItem.element.jParent;
        jparent.RemoveChild(selectedTreeItem.element);
        
        var parent = selectedTreeItem.Parent as mTreeViewItem;
        parent.Items.Remove(selectedTreeItem);
        MainHierarchyTree.SelectedItem  = (jparent.jChildren.Count > 0) ? jparent.jChildren.Last().mTreeItem : ((JControl)jparent).mTreeItem;
        
        var data = new Object[] {jparent,element,element.mTreeItem};
        AddHistoryItem(new Change(element,"Removed",data));
       // MainHierarchyTree.SelectedItem=selectedTreeItem.element.mTreeItem;
        element.Dispose();
    }
    
    private void OnjControlPressed(object? sender, PointerPressedEventArgs e)
    {
        e.Handled = true;
        
        InitMovable((JControl)sender);
        var element = sender as JControl;
        element.IsPressed = true;
        MainHierarchyTree.SelectedItem = (element).mTreeItem;
    }

    private void OnjControlReleased(object? sender, PointerReleasedEventArgs e)
    {
        e.Handled = true;
        var element = sender as JControl;
        element.IsPressed = false;
    }

    private async void OnChooseImageClick(object? sender, RoutedEventArgs e)
    {
        OpenFileDialog dialog = new OpenFileDialog();
        dialog.Title = "Выберите изображение";
        dialog.AllowMultiple = false;
        dialog.Filters.Add(new FileDialogFilter
        {
            Name = "Изображения",
            Extensions = { "png", "jpg", "jpeg", "gif", "bmp" }
        });
        Task<string[]> task = dialog.ShowAsync(this);
        
        // Дожидаемся завершения задачи (await)
        string[] result = await task;

        // Обрабатываем результат
        if (result != null && result.Length > 0)
        {
            string fileName = Path.GetFileName(result[0]);
            string targetFilePath = Path.Combine("assets", fileName);
            File.Copy(result[0], targetFilePath, true);
            ((jImage)selectedTreeItem.element).jImageSource = @"assets/" + fileName;
            ((jImage)selectedTreeItem.element).Source = new Bitmap(((jImage)selectedTreeItem.element).jImageSource);
        }
        else
        {
            return;
        }
    }
    private void OnEnumPropertyChanged(object? sender, SelectionChangedEventArgs e)
    {
        var comboBox = sender as ComboBox;
        var parentPanel = comboBox.Parent as DockPanel;
        var txt_blc = parentPanel.Children[0] as TextBlock;
        var prop_name = txt_blc.Text;
        Type jElement_type = selectedTreeItem.element.GetType();
        var prop_type = jElement_type.GetProperty(prop_name).PropertyType;
        var prop = jElement_type.GetProperty(prop_name);
        object enumValue = Enum.Parse(prop_type, comboBox.SelectedItem.ToString());
        
        object prevalue = prop.GetValue(selectedTreeItem.element);
        AddHistoryItem(new Change(selectedTreeItem.element,prop_name,prevalue));
        
        prop.SetValue(selectedTreeItem.element, enumValue);
    }

    private void OnColorChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property == AvaloniaColorPicker.ColorButton.ColorProperty)
        {
            var colorButton = sender as ColorButton;
            var parentPanel = colorButton.Parent.Parent as DockPanel;
            var txt_blc = parentPanel.Children[0] as TextBlock;
            var prop_name = txt_blc.Text;
           
            Type jElement_type = selectedTreeItem.element.GetType();
            var prop_type = jElement_type.GetProperty(prop_name).PropertyType;
            var prop = jElement_type.GetProperty(prop_name);
           
            object prevalue = prop.GetValue(selectedTreeItem.element);
            AddHistoryItem(new Change(selectedTreeItem.element,prop_name,prevalue));
           
            prop.SetValue(selectedTreeItem.element, new SolidColorBrush(colorButton.Color));
            var textBlock = ((StackPanel)(colorButton.Parent)).Children[0] as TextBlock;
            textBlock.Text = colorButton.Color.ToString();
        }
    }

    private void OnBoolPropertyChanged(object? sender, RoutedEventArgs e)
    {
        var checkBox = sender as CheckBox;
        var parentPanel = checkBox.Parent as DockPanel;
        var txt_blc = parentPanel.Children[0] as TextBlock;
        var prop_name = txt_blc.Text;
        Type jElement_type = selectedTreeItem.element.GetType();
        var prop = jElement_type.GetProperty(prop_name);
       
        object prevalue = prop.GetValue(selectedTreeItem.element);
        AddHistoryItem(new Change(selectedTreeItem.element,prop_name,prevalue));
        
        prop.SetValue(selectedTreeItem.element, checkBox.IsChecked);
    }

    private void OnPropertyChanged(object? sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter) return;
        var textBox = sender as TextBox;

        var parentPanel = textBox.Parent as DockPanel;
        var txt_blc = parentPanel.Children[0] as TextBlock;
        var prop_name = txt_blc.Text;
        Type jElement_type = selectedTreeItem.element.GetType();
        var prop_type = jElement_type.GetProperty(prop_name).PropertyType;
        var prop = jElement_type.GetProperty(prop_name);
        textBox.Foreground = new SolidColorBrush(Color.Parse("#88F1FF"));
        
        object prevalue = prop.GetValue(selectedTreeItem.element);
        AddHistoryItem(new Change(selectedTreeItem.element,prop_name,prevalue));

        if (textBox.Text == "не число") return;
        try
        {
            if (prop.Name == "Name")
            {
                FieldInfo privateField =
                    typeof(StyledElement).GetField("_name", BindingFlags.NonPublic | BindingFlags.Instance);
                privateField.SetValue(selectedTreeItem.element, textBox.Text);
                selectedTreeItem.Header = textBox.Text;
            }
            else if (prop.Name == "Content")
                prop.SetValue(selectedTreeItem.element, textBox.Text);
            else if (prop_type == typeof(string))
                prop.SetValue(selectedTreeItem.element, textBox.Text);
            else if (prop_type == typeof(int))
                prop.SetValue(selectedTreeItem.element, Convert.ToInt32(textBox.Text));
            else if (prop_type == typeof(double))
            {
                textBox.Text = textBox.Text.Replace('.', ',');
                prop.SetValue(selectedTreeItem.element, Convert.ToDouble(textBox.Text));
            }
            else if (prop_type == typeof(IBrush))
            {
                var brush = new SolidColorBrush(Color.Parse(textBox.Text));
                prop.SetValue(selectedTreeItem.element, brush);
                textBox.Foreground = new SolidColorBrush(Color.Parse(textBox.Text));
            }
            else if (prop_type == typeof(Thickness))
            {
                var values = textBox.Text.Split(',');
                var rect = new Thickness(Convert.ToInt32(values[0]), Convert.ToInt32(values[1]),
                    Convert.ToInt32(values[2]), Convert.ToInt32(values[3]));
                prop.SetValue(selectedTreeItem.element, rect);
            }
            else if (prop_type == typeof(CornerRadius))
            {
                var values = textBox.Text.Split(',');
                var rect = new CornerRadius(Convert.ToInt32(values[0]), Convert.ToInt32(values[1]),
                    Convert.ToInt32(values[2]), Convert.ToInt32(values[3]));
                prop.SetValue(selectedTreeItem.element, rect);
            }
            else if (prop_type == typeof(Rect))
            {
                var values = textBox.Text.Split(',');
                var rect = new Rect(Convert.ToInt32(values[0]), Convert.ToInt32(values[1]),
                    Convert.ToInt32(values[2]), Convert.ToInt32(values[3]));
                prop.SetValue(selectedTreeItem.element, rect);
            }
        }
        catch
        {
            textBox.Text = "Некорректное значение";
            textBox.Foreground = Brushes.Red;
        }
    }


    private async void RUN_WINDOW(object? sender, RoutedEventArgs e)
    {
        CopyAssets();
        LoadingGif.IsVisible = true;
        
        await ExecuteLinuxCommandAsync(@"XamladeDemo/BUILD.sh");
        LoadingGif.IsVisible = false;
        await ExecuteLinuxCommandAsync(@"XamladeDemo/RUN.sh");
    }


    public static void CopyAssets()
    {
        string[] files = Directory.GetFiles(@"assets");
        string targetDirectory = @"XamladeDemo/assets";
        if (!Directory.Exists(targetDirectory))
        {
            Directory.CreateDirectory(targetDirectory);
        }
        foreach (string file in files)
        {
            string fileName = Path.GetFileName(file);
            string destFile = Path.Combine(targetDirectory, fileName);
            File.Copy(file, destFile, true);
        }
    }
    public static async Task<string> ExecuteLinuxCommandAsync(string command)
    {
        using (Process process = new Process())
        {
            process.StartInfo.FileName = "/bin/bash";
            process.StartInfo.Arguments = $"-c \"{command}\"";
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;

            process.Start();

            string result = await process.StandardOutput.ReadToEndAsync();
            string error = await process.StandardError.ReadToEndAsync();

            if (!string.IsNullOrEmpty(error))
            {
                throw new Exception($"Error: {error}");
            }

            return result;
        }
    }
 

    private async void DEXAMLIZE(object? sender, RoutedEventArgs e)
    {


        await RunDeXAMLIZE(this);

    }

    public System.String ass;

    private void DEBUG(object? sender, RoutedEventArgs e)
    {
       
    }
}