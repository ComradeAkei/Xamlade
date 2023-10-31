using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.Controls.Primitives;

namespace Xamlade;

public class KeyValue
{
    public string Key { get; set; }
    public string? Value { get; set; }
}

public static class Broadcast
{
    public delegate void EventHandler(int mode);

    public static event EventHandler OnBroadcast;

    public static void InitXAML()=> OnBroadcast?.Invoke(0);
    public static void XAMLize()=> OnBroadcast?.Invoke(1);
    
}

public partial class MainWindow : Window
{
    
    
    #region  Globals
    
    //Отладочный итератор
    private int i = 2;
    
    //Случайное число
    private static Random random;
    
    
    //Перемещаемый по холсту объект
    private JControl movable;
    
    //Выбранный в дереве элемент
    private mTreeViewItem? selectedTreeItem;
    
    
    //Оригинальный фон выбранного элемента
    private IBrush? selectedOriginalBackground;
    
    // Половина ширины перемещаемого элемента
    private double mov_hw;

    // Половина высоты перемещаемого элемента 
    private double mov_hh;


    
    #endregion
    
    
    public ObservableCollection<KeyValue> KeyValueList { get; set; }
    public MainWindow()
    {
        KeyValueList = new ObservableCollection<KeyValue>();
        InitializeComponent();
        DataContext = this;
        WindowState = WindowState.Maximized;
        selectedOriginalBackground = MainCanvas.Background;
        MainHierarchyTree.Items.Add(new mTreeViewItem(MainCanvas));
        selectedTreeItem = MainCanvas.mTreeItem;
        MainHierarchyTree.SelectedItem = selectedTreeItem;
        random = new Random();
    }
    
    
//Переписать с привязкой к родителю

    //Всё к хуям заново
    private void jCanvas_OnPointerMoved(object? sender, PointerEventArgs e)
    {
        if (movable == null) return;
        if(Equals((JControl)sender!, movable)) return;
        if(Equals(movable, MainCanvas)) return;
        
        e.Handled = true;
        var ParentCanvas = (jCanvas)sender!;
        if (e.GetCurrentPoint(ParentCanvas).Properties.PointerUpdateKind == PointerUpdateKind.Other)
        {
            Point mousePosition = e.GetPosition(ParentCanvas);
            var element = movable as Control;
            
            if (movable.IsPressed)
            {
                Canvas.SetLeft(element, mousePosition.X - mov_hw);
                Canvas.SetTop(element, mousePosition.Y - mov_hh);

                if (Canvas.GetLeft(element) < 0) Canvas.SetLeft(element, 0);
                if (Canvas.GetTop(element) < 0) Canvas.SetTop(element, 0);

                if (Canvas.GetLeft(element) + 2 * mov_hw > ParentCanvas.Bounds.Width)
                    Canvas.SetLeft(element, ParentCanvas.Bounds.Width - 2 * mov_hw);

                if (Canvas.GetTop(element) + 2 * mov_hh > ParentCanvas.Bounds.Height)
                    Canvas.SetTop(element, ParentCanvas.Bounds.Height - 2 * mov_hh);
            }

        }
    }
    
    

    private void GenerateButton_OnClick(object? sender, RoutedEventArgs e)
    {
        if(selectedTreeItem.element is not IChildContainer) return;
        var btn = new jButton
        {
            Name = $"Button{i++}",
            Content = TEXT.Text,
            Background = Brushes.Blue,
            Foreground = Brushes.White
        }; 
         btn.PointerEntered += OnjControlPointerEntered;
        btn.PointerExited += OnjControlPointerExited;
        btn.Click += jButtonClick;
       // btn.PointerPressed += OnjControlPressed;
    //    btn.PointerReleased += OnjControlReleased;
        Canvas.SetLeft(btn, 0);
        Canvas.SetTop(btn, 0);
        selectedTreeItem.Items.Add(new mTreeViewItem(btn));
        var parentCanvas = selectedTreeItem.element as jCanvas;
        parentCanvas.AddChild(btn);
    }


    private void InitMovable(JControl obj)
    {
        movable = obj;
        mov_hw = obj.Bounds.Width / 2;
        mov_hh = obj.Bounds.Height / 2;
    }


    private void DEBUG(object? sender, RoutedEventArgs? e)
    {
        ((Button)sender).Content = "   XAMLize   ";
      
        
        Broadcast.InitXAML();
        while (MainCanvas.XAMLRating > -1)
        {
            Broadcast.XAMLize();
        }
        string filePath = @"/home/akei/Projects/Xamlade/Xamlade/TestWindow.axaml";
        var outputXAML = new List<string>();
        outputXAML.Add(@"<Window xmlns=""https://github.com/avaloniaui""
        xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
        xmlns:d=""http://schemas.microsoft.com/expression/blend/2008""
        xmlns:mc=""http://schemas.openxmlformats.org/markup-compatibility/2006""
        mc:Ignorable=""d"" d:DesignWidth=""800"" d:DesignHeight=""450""
        x:Class=""Xamlade.TestWindow""
        Title=""TestWindow"">");
        outputXAML.AddRange(MainCanvas.XAMLPiece);
        outputXAML.Add(@"</Window>");
        File.WriteAllLines(filePath, outputXAML);
      
        
        
        
        
    /*    Type type = selectedTreeItem.element.GetType();
        var props = type.GetProperties();
        ConstructorInfo? constructor = type.GetConstructor(new Type[] { });
        var DefaultObject = constructor.Invoke(new object[] { });
        
        StreamWriter sw = new StreamWriter("out.txt");
        foreach (var prop in props)
        {
           // if(prop.PropertyType == typeof(string) || prop.PropertyType == typeof(int) || prop.PropertyType == typeof(double) || prop.PropertyType == typeof(bool) || prop.PropertyType == typeof(IBrush))
           if(prop.Name=="Item") continue;
           if(prop.GetValue(selectedTreeItem.element)?.ToString() == prop.GetValue(DefaultObject)?.ToString())
               sw.WriteLine($"{prop.Name};{prop.GetValue(selectedTreeItem.element)};{prop.GetValue(DefaultObject)}");
        }
        sw.Close();
        */
    
    }


    public static readonly List<string> ExcludedWords = new List<string>
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
        "CommandParameter", "Flyout","Theme", "Clip","TemplatedParent","Effect",
        "OpacityMask","Bounds", "Cursor","Tag", "ContextFlyout","ContextMenu","FocusAdorner","IsItemsHost",
        "Children","jChildren","FontFamily", "TextDecoration","ContentTemplate","FlowDirection","Inlines","TextLayout",
        "XAMLRating", "XAMLPiece"
    };
    
    private void ShowProperties()
    {
        KeyValueList.Clear();
        if(selectedTreeItem.element == MainCanvas) return;
        Type type = selectedTreeItem.element.GetType();
        var props = type.GetProperties();
        
        foreach (var prop in props)
        {
            if (!ExcludedWords.Contains(prop.Name))
            {
                KeyValueList.Add(new KeyValue { Key = prop.Name, Value = prop.GetValue(selectedTreeItem.element)?.ToString() });
            }
        }
    }
    
    // Выбрать редактируемый элемент
   private void SelectjElement(JControl element)
    {
        selectedTreeItem.element.Background = selectedOriginalBackground;
        selectedTreeItem = element.mTreeItem;
        MainHierarchyTree.SelectedItem = selectedTreeItem;
        selectedOriginalBackground = selectedTreeItem.element.Background;
        selectedTreeItem.element.Background = Brushes.LightBlue;
        InitMovable(selectedTreeItem.element);
        ShowProperties();
    }
    
    
    private void GenerateCanvas_OnClick(object? sender, RoutedEventArgs e)
    {
        if(selectedTreeItem.element is not IChildContainer) return;
        // Генерация случайного цвета в формате HEX
        string randomHexColor = $"#{random.Next(0x1000000):X6}";

        // Преобразование HEX строки в Color объект
        Color randomColor = Color.Parse(randomHexColor);
        
        
        jCanvas cnv1 = new jCanvas
        {
            Background = new SolidColorBrush(randomColor),
            Height = Convert.ToInt32(CanvasHeight.Text),
            Width = Convert.ToInt32(CanvasWidth.Text),
            Name = $"Canvas{i++}"
        };
        cnv1.PointerMoved += jCanvas_OnPointerMoved;
        cnv1.PointerReleased += OnjControlReleased;
        cnv1.PointerPressed += OnjControlPressed;
        Canvas.SetTop(cnv1, 0);
        Canvas.SetLeft(cnv1, 0);
        selectedTreeItem.Items.Add(new mTreeViewItem(cnv1));
        var parentCanvas = selectedTreeItem.element as jCanvas;
        parentCanvas.AddChild(cnv1);

    }
    

    private void MainHierarchyTree_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        var item = ((TreeView)sender).SelectedItem as mTreeViewItem;
   //     if(item == selectedTreeItem) return;
        if(item != null)
            SelectjElement(item.element);
        e.Handled = true;
    }

    private void jButtonClick(object? sender, RoutedEventArgs e)
    {
     MainHierarchyTree.SelectedItem = ((JControl)sender).mTreeItem;

    }
    private void OnjControlPointerEntered(object? sender, PointerEventArgs e)
    {
        InitMovable((JControl)sender);
    }

    private void OnjControlPointerExited(object? sender, PointerEventArgs e)
    {
        movable = null;
    }

    private void RemovejElement(object? sender, RoutedEventArgs e)
    {
        if(selectedTreeItem == MainCanvas.mTreeItem) return;
        selectedTreeItem.element.jParent.RemoveChild(selectedTreeItem.element);
        var parent = selectedTreeItem.Parent as mTreeViewItem;
        parent.Items.Remove(selectedTreeItem);
    }
    
    //КНОПКИ НА ЭТУ ХУЕТЕНЬ НЕ ПОДПИСЫВАТЬ!!!
    private void OnjControlPressed(object? sender, PointerPressedEventArgs e)
    {
        e.Handled = true;
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

    private void GenerateCheckBox_OnClick(object? sender, RoutedEventArgs e)
    {
        if(selectedTreeItem.element is not IChildContainer) return;
        var checkBox = new jCheckBox
        {
            Name = $"Checkbox{i++}",
            Background = Brushes.Blue,
            Content = TEXT.Text,
            FontSize = 20,
            Foreground = Brushes.White
        }; 
        checkBox.PointerEntered += OnjControlPointerEntered;
        checkBox.PointerExited += OnjControlPointerExited;
        // checkBox.Click += jButtonClick;
        checkBox.PointerPressed += OnjControlPressed;
        checkBox.PointerReleased += OnjControlReleased;
        Canvas.SetLeft(checkBox, 0);
        Canvas.SetTop(checkBox, 0);
        
        selectedTreeItem.Items.Add(new mTreeViewItem(checkBox));
        var parentCanvas = selectedTreeItem.element as jCanvas;
        parentCanvas.AddChild(checkBox);
    }

    private void GenerateTextBlock_OnClick(object? sender, RoutedEventArgs e)
    {
        if(selectedTreeItem.element is not IChildContainer) return;
        var textBlock = new jTextBlock
        {
            Name = $"Textblock{i++}",
            Background = Brushes.Blue,
            Text = TEXT.Text,
            FontSize = 20,
            Foreground = Brushes.White
        }; 
       // textBlock.PointerEntered += Button1_OnPointerEntered;
      //  textBlock.PointerExited += Button1_OnPointerExited;
       //  textBlock.Click += jButtonClick;
         textBlock.PointerPressed += OnjControlPressed;
         textBlock.PointerReleased += OnjControlReleased;
        Canvas.SetLeft(textBlock, 0);
        Canvas.SetTop(textBlock, 0);
        
        selectedTreeItem.Items.Add(new mTreeViewItem(textBlock));
        var parentCanvas = selectedTreeItem.element as jCanvas;
        parentCanvas.AddChild(textBlock);
    }
    
    private void OnPropertyChanged(object? sender, KeyEventArgs e)
    {
        if(e.Key != Key.Enter) return;
        var textBox = sender as TextBox;
        textBox.Foreground = Brushes.Black;
        var parentPanel = textBox.Parent as DockPanel;
        var txt_blc = parentPanel.Children[0] as TextBlock;
        var prop_name = txt_blc.Text;
        Type jElement_type = selectedTreeItem.element.GetType();
        var prop_type = jElement_type.GetProperty(prop_name).PropertyType;
        var prop = jElement_type.GetProperty(prop_name);

        if(textBox.Text == "не число") return;
        try
        {
            

            if (prop.Name == "Name")
            {
                FieldInfo privateField =
                    typeof(StyledElement).GetField("_name", BindingFlags.NonPublic | BindingFlags.Instance);
                privateField.SetValue(selectedTreeItem.element, textBox.Text);
                selectedTreeItem.Header = textBox.Text;
            }
            else if (prop_type == typeof(string))
                prop.SetValue(selectedTreeItem.element, textBox.Text);
            else if (prop_type == typeof(int))
                prop.SetValue(selectedTreeItem.element, Convert.ToInt32(textBox.Text));
            else if (prop_type == typeof(double))
            {
                textBox.Text = textBox.Text.Replace('.', ',');
                prop.SetValue(selectedTreeItem.element, Convert.ToDouble(textBox.Text));
            }
            else if (prop_type == typeof(bool))
                prop.SetValue(selectedTreeItem.element, Convert.ToBoolean(textBox.Text));
            else if (prop_type == typeof(IBrush))
            {
                var brush = new SolidColorBrush(Color.Parse(textBox.Text));
                prop.SetValue(selectedTreeItem.element, brush);
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
        }
        catch
        {
            textBox.Text = "Некорректное значение";
            textBox.Foreground = Brushes.Red;
        }

    }


    private void RUN_WINDOW(object? sender, RoutedEventArgs e)
    {
        TestWindow window = new TestWindow();
        window.Show();
    }
}