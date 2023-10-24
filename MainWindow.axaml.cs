using System;
using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;

namespace Xamlade;


public partial class MainWindow : Window
{
    #region  Globals
    
    private int i = 2;
    private double mouseX;
    private double mouseY;

    //Перемещаемый объект
    private JControl movable;

    //Current canvas
    private Canvas? currentCanvas;
    
    //Выбранный в дереве элемент
    private mTreeViewItem? selectedTreeItem;
    
    
    //Оригинальный фон выбранного элемента
    private IBrush? selectedOriginalBackground;

    //Костыль для кнопки
    private IBrush? trueButtonBackground;
    // Половина ширины
    private double mov_hw;

    // Половина высоты
    private double mov_hh;

    private static Random random;
    
    #endregion
    
    
    public MainWindow()
    {
        InitializeComponent();
        WindowState = WindowState.Maximized;
        currentCanvas = MainCanvas;
        selectedOriginalBackground = MainCanvas.Background;
        MainHierarchyTree.Items.Add(new mTreeViewItem(MainCanvas));
        selectedTreeItem = MainCanvas.mTreeItem;
        MainHierarchyTree.SelectedItem = selectedTreeItem;
        random = new Random();
    }

    
    
//Переписать с привязкой к родителю

    //Всё к хуям заново
    private void InputElement_OnPointerMoved(object? sender, PointerEventArgs e)
    {
        if(Equals((JControl)sender!, movable)) return;
        if(Equals(movable, MainCanvas)) return;
        e.Handled = true;
        var jc = (JControl)sender;
        if (e.GetCurrentPoint((jCanvas)sender!).Properties.PointerUpdateKind == PointerUpdateKind.Other)
        {

            // if (movable != null)
            // {
            // Info.Text = movable.Name;
            // }

            Point position = e.GetPosition((Canvas)sender!);
            mouseX = position.X;
            mouseY = position.Y;
            var element = movable as Control;
            if (movable != null)
                if (movable.IsPressed)
                {
                    Canvas.SetLeft(element, mouseX - mov_hw);
                    Canvas.SetTop(element, mouseY - mov_hh);

                    if (Canvas.GetLeft(element) < 0) Canvas.SetLeft(element, 0);
                    if (Canvas.GetTop(element) < 0) Canvas.SetTop(element, 0);

                    if (Canvas.GetLeft(element) + 2 * mov_hw > ((Canvas)sender!).Bounds.Width)
                        Canvas.SetLeft(element, ((Canvas)sender!).Bounds.Width - 2 * mov_hw);

                    if (Canvas.GetTop(element) + 2 * mov_hh > ((Canvas)sender!).Bounds.Height)
                        Canvas.SetTop(element, ((Canvas)sender!).Bounds.Height - 2 * mov_hh);
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
         btn.PointerEntered += Button1_OnPointerEntered;
        btn.PointerExited += Button1_OnPointerExited;
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


    private void DEBUG(object? sender, RoutedEventArgs e)
    {
        var a = new jButton();
        Console.WriteLine(a.Bounds.GetType());

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
        cnv1.PointerMoved += InputElement_OnPointerMoved;
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
    private void Button1_OnPointerEntered(object? sender, PointerEventArgs e)
    {
        InitMovable((JControl)sender);
        
    }

    private void Button1_OnPointerExited(object? sender, PointerEventArgs e)
    {
        movable = null;
    }

    private void RemoveElement(object? sender, RoutedEventArgs e)
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
            Name = $"Checkbox {i++}",
            Background = Brushes.Blue,
            Content = TEXT.Text,
            FontSize = 20,
            Foreground = Brushes.White
        }; 
        checkBox.PointerEntered += Button1_OnPointerEntered;
        checkBox.PointerExited += Button1_OnPointerExited;
        checkBox.Click += jButtonClick;
        // btn.PointerPressed += OnjControlPressed;
        //    btn.PointerReleased += OnjControlReleased;
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
            Name = $"Textblock {i++}",
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
}