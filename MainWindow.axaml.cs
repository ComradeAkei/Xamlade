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
    private int i = 2;
    private double mouseX;
    private double mouseY;

    //Перемещаемый объект
    private Control movable;

    //Current canvas
    private Canvas? currentCanvas;
    
    //Выбранный в дереве элемент
    private mTreeViewItem selectedTreeItem;

    // Половина ширины
    private double mov_hw;

    // Половина высоты
    private double mov_hh;

    private static Random random;
    
    
    
    public MainWindow()
    {
        InitializeComponent();
        WindowState = WindowState.Maximized;
        currentCanvas = MainCanvas;
        MainHierarchyTree.Items.Add(new mTreeViewItem(MainCanvas));
        selectedTreeItem = MainCanvas.mTreeItem;
        random = new Random();
    }

    
    
//Переписать с привязкой к родителю

    //Всё к хуям заново
    private void InputElement_OnPointerMoved(object? sender, PointerEventArgs e)
    {
        e.Handled = true;
        var jc = (JControl)sender;
        if (e.GetCurrentPoint((Canvas)sender!).Properties.PointerUpdateKind == PointerUpdateKind.Other)
        {

            // if (movable != null)
            // {
            // Info.Text = movable.Name;
            // }

            Point position = e.GetPosition((Canvas)sender!);
            mouseX = position.X;
            mouseY = position.Y;

            // MousePosition.Text = $"X:{position.X}, Y:{position.Y}";
            if (movable != null)
                if (IS_PRESSED(movable))
                {
                    Canvas.SetLeft(movable, mouseX - mov_hw);
                    Canvas.SetTop(movable, mouseY - mov_hh);

                    if (Canvas.GetLeft(movable) < 0) Canvas.SetLeft(movable, 0);
                    if (Canvas.GetTop(movable) < 0) Canvas.SetTop(movable, 0);

                    if (Canvas.GetLeft(movable) + 2 * mov_hw > ((Canvas)sender!).Bounds.Width)
                        Canvas.SetLeft(movable, ((Canvas)sender!).Bounds.Width - 2 * mov_hw);

                    if (Canvas.GetTop(movable) + 2 * mov_hh > ((Canvas)sender!).Bounds.Height)
                        Canvas.SetTop(movable, ((Canvas)sender!).Bounds.Height - 2 * mov_hh);
                }

        }
    }

    private bool IS_PRESSED(Control mvbl)
    {
        if (mvbl is jButton) return ((jButton)mvbl).IsPressed;
        if (mvbl is jCanvas) return ((jCanvas)mvbl).IsPressed;
        return false;
    }

    private void Button1_OnPointerEntered(object? sender, PointerEventArgs e)
    {

        InitMovable((Control)sender);
    }

    private void Button1_OnPointerExited(object? sender, PointerEventArgs e)
    {
        movable = null;
    }

    private void GenerateButton_OnClick(object? sender, RoutedEventArgs e)
    {
        Button btn = new jButton
        {
            Name = $"Button{i++}",
            Content = $"Button{i}",
            Background = Brushes.Blue,
            Foreground = Brushes.White
        };
        btn.PointerEntered += Button1_OnPointerEntered;
        btn.PointerExited += Button1_OnPointerExited;
        Canvas.SetLeft(btn, 0);
        Canvas.SetTop(btn, 0);
        currentCanvas.Children.Add(btn);
    }


    private void InitMovable(Control obj)
    {
        movable = obj;
        mov_hw = obj.Bounds.Width / 2;
        mov_hh = obj.Bounds.Height / 2;
    }


    private void DEBUG(object? sender, RoutedEventArgs e)
    {
        // TreeViewItem selectedNode = tree.SelectedItem as TreeViewItem;
        // selectedNode.Items.Add(new TreeViewItem { Header = $"Debug {i}" });
        // i++;
    }

    


    
    
    private void GenerateCanvas_OnClick(object? sender, RoutedEventArgs e)
    {
        // Генерация случайного цвета в формате HEX
        string randomHexColor = $"#{random.Next(0x1000000):X6}";

        // Преобразование HEX строки в Color объект
        Color randomColor = Color.Parse(randomHexColor);
        
        
        jCanvas cnv1 = new jCanvas
        {
            Background = new SolidColorBrush(randomColor),
            Height = 400-20*i,
            Width = 400-20*i,
            Name = $"Canvas{i++}"
        };
        cnv1.PointerPressed += MainCanvas_OnPointerPressed;
        cnv1.PointerMoved += InputElement_OnPointerMoved;
        cnv1.PointerReleased += MainCanvas_OnPointerReleased;
        cnv1.PointerEntered += Button1_OnPointerEntered;
        cnv1.PointerExited += Button1_OnPointerExited;
        Canvas.SetTop(cnv1, 0);
        Canvas.SetLeft(cnv1, 0);
        //Тут главное потом сделать проверку на принадлежность IChildContainer
        selectedTreeItem.Items.Add(new mTreeViewItem(cnv1));
        var parentCanvas = selectedTreeItem.element as jCanvas;
        parentCanvas.AddChild(cnv1);

    }

    private void MainCanvas_OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (sender is jCanvas) ((jCanvas)sender).IsPressed = false;
        e.Handled = true;
    }

    private void MainCanvas_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (sender is jCanvas) ((jCanvas)sender).IsPressed = true;
        if (currentCanvas != null) currentCanvas.Background = Brushes.Aqua;
        ((Canvas)sender!).Background = Brushes.Khaki;
        currentCanvas = (Canvas)sender;
        e.Handled = true;
    }

    private void MainHierarchyTree_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        selectedTreeItem = ((TreeView)sender).SelectedItem as mTreeViewItem;
       e.Handled = true;
    }

    private void RemoveElement(object? sender, RoutedEventArgs e)
    {
        selectedTreeItem.element.jParent.RemoveChild(selectedTreeItem.element);
        var parent = selectedTreeItem.Parent as mTreeViewItem;
        parent.Items.Remove(selectedTreeItem);
    }
}