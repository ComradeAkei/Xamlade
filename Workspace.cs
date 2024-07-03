using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace Xamlade;

public static class Workspace
{
    
    public static jCanvas MainCanvas { get; set; }
    
    //Перемещаемый по холсту объект
    public static JControl movable;
    
    //Кандидат на перемещение
    public static JControl premovable;
    
    // Половина ширины перемещаемого элемента
    public static double mov_hw;

    // Половина высоты перемещаемого элемента 
    public static double mov_hh;

    public static void Init(jCanvas mainCanvas)
    {
        MainCanvas = mainCanvas;
        mainCanvas.PointerMoved += jCanvas_OnPointerMoved;
    }
    
    public static void InitMovable(JControl obj)
    {
        if (obj is null) return;
        MainWindow.AddHistoryItem(new MainWindow.Change(obj, 
            "Coordinates",
            new MainWindow.Coordinates(Canvas.GetLeft(obj as Control),Canvas.GetTop(obj as Control))));
        movable = obj;
        mov_hw = obj.Bounds.Width / 2;
        mov_hh = obj.Bounds.Height / 2;
    }

    public static void InitPremovable()
    {
        if(premovable is null) return;
        InitMovable(premovable);
        HierarchyControl.HierarchyTree.SelectedItem = (premovable).mTreeItem;
        
    }
    
    
    public static void jCanvas_OnPointerMoved(object? sender, PointerEventArgs e)
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

        if (movable.IsPressed && !MainWindow.LCtrlPressed)
        {
            Canvas.SetLeft(element, MainWindow.CorrectCoords(mousePosition.X - mov_hw));
            Canvas.SetTop(element, MainWindow.CorrectCoords(mousePosition.Y - mov_hh));

            if (Canvas.GetLeft(element) < 0)
                Canvas.SetLeft(element, 0);

            if (Canvas.GetTop(element) < 0)
                Canvas.SetTop(element, 0);

            if (Canvas.GetLeft(element) + 2 * mov_hw > parentCanvas.Bounds.Width)
                Canvas.SetLeft(element, parentCanvas.Bounds.Width - 2 * mov_hw);

            if (Canvas.GetTop(element) + 2 * mov_hh > parentCanvas.Bounds.Height)
                Canvas.SetTop(element, parentCanvas.Bounds.Height - 2 * mov_hh);
        }
        else if (movable.IsPressed && MainWindow.LCtrlPressed)
        {
            if (MainWindow.ResizeFlag)
            {
                MainWindow.UndoList.Remove(MainWindow.UndoList.Last());
                MainWindow.AddHistoryItem(new MainWindow.Change(movable, "Size", new MainWindow.Size(element.Bounds.Width, element.Bounds.Height)));
            }

            MainWindow.ResizeFlag = false;
            if (double.IsNaN(element.Width))
                element.Width = element.Bounds.Width;
            mousePosition = e.GetPosition(element);
            if(mousePosition.X<5 || mousePosition.Y<5)
                return;
            element.Width = MainWindow.CorrectSize(mousePosition.X);
            element.Height = MainWindow.CorrectSize(mousePosition.Y);
        }
    }
    
    public static void jElementClick(object? sender, RoutedEventArgs e)
    {
        e.Handled = true;
        InitMovable((JControl)sender);
        HierarchyControl.HierarchyTree.SelectedItem = ((JControl)sender).mTreeItem;
    }

    public static void OnjControlPointerEntered(object? sender, PointerEventArgs e)
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

    public static void OnjControlPointerExited(object? sender, PointerEventArgs e)
    {
        e.Handled = true;
        premovable = null;
    }
    
}