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
    // Половина ширины
    private double mov_hw;
    // Половина высоты
    private double mov_hh;
    public MainWindow()
    {
        InitializeComponent();
        WindowState = WindowState.Maximized;
        currentCanvas = myCanvas;
    }
    
//Переписать с привязкой к родителю
    private void InputElement_OnPointerMoved(object? sender, PointerEventArgs e)
    {
        e.Handled = true;
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
                    Canvas.SetLeft(movable, mouseX-mov_hw);
                    Canvas.SetTop(movable, mouseY-mov_hh);
                  
                    if(Canvas.GetLeft(movable)<0) Canvas.SetLeft(movable, 0);
                    if(Canvas.GetTop(movable)<0) Canvas.SetTop(movable, 0);
                  
                    if (Canvas.GetLeft(movable) + 2 * mov_hw > ((Canvas)sender!).Bounds.Width)
                        Canvas.SetLeft(movable, ((Canvas)sender!).Bounds.Width -2* mov_hw);
                   
                    if (Canvas.GetTop(movable) + 2 * mov_hh > ((Canvas)sender!).Bounds.Height)
                        Canvas.SetTop(movable, ((Canvas)sender!).Bounds.Height -2* mov_hh);
                }

        }
    }

    private bool IS_PRESSED(Control mvbl)
    {
        if(mvbl is jButton) return ((jButton)mvbl).IsPressed;
        if(mvbl is jCanvas) return ((jCanvas)mvbl).IsPressed;
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
        btn.PointerEntered+=Button1_OnPointerEntered;
        btn.PointerExited+=Button1_OnPointerExited;
        Canvas.SetLeft(btn, 0);
        Canvas.SetTop(btn, 0);
        currentCanvas.Children.Add(btn);
    }


    private void InitMovable(Control obj)
    {
        movable = obj;
        mov_hw = obj.Bounds.Width/2;
        mov_hh = obj.Bounds.Height/2;
    }
    

    private void DEBUG(object? sender, RoutedEventArgs e)
    {
      Canvas.SetLeft(currentCanvas,0);
    }

    private void GenerateCanvas_OnClick(object? sender, RoutedEventArgs e)
    {
        jCanvas cnv1 = new jCanvas
        {
            Background = Brushes.Aqua,
            Height = 200,
            Width = 200,
            Name = $"Canvas{i++}"
        };
        cnv1.PointerPressed += MyCanvas_OnPointerPressed;
        cnv1.PointerMoved += InputElement_OnPointerMoved;
        cnv1.PointerReleased+= MyCanvas_OnPointerReleased;
        cnv1.PointerEntered += Button1_OnPointerEntered;
        cnv1.PointerExited += Button1_OnPointerExited;
        Canvas.SetTop(cnv1, 200);
        Canvas.SetLeft(cnv1, 200);
        myCanvas.Children.Add(cnv1);
        
    }
    private void MyCanvas_OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (sender is jCanvas) ((jCanvas)sender).IsPressed = false;
        e.Handled = true;
    }
    private void MyCanvas_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (sender is jCanvas) ((jCanvas)sender).IsPressed = true;
        if (currentCanvas != null) currentCanvas.Background = Brushes.Aqua;
        ((Canvas)sender!).Background = Brushes.Khaki;
        currentCanvas = (Canvas)sender;
        e.Handled = true;
    }
    
    
}