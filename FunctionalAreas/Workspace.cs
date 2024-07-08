using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;

namespace Xamlade;

public static class Workspace
{
    
    public static jCanvas MainCanvas { get; set; }

    private static List<JControl> SelectedObjects = new List<JControl>(); 
    
    public static jCanvas SelectionCanvas { get; set; }
    
    private static Rectangle SelectionRectangle { get; set; }
    
    //Перемещаемый по холсту объект
    public static JControl? movable;
    
    //Кандидат на перемещение (кнопки)
    public static JControl? premovable;
    
    // Половина ширины перемещаемого элемента
    public static double mov_hw;

    // Половина высоты перемещаемого элемента 
    public static double mov_hh;
    
    //Оригинальный фон выбранного элемента
    public static IBrush? selectedOriginalBackground;

    public static void Init(jCanvas mainCanvas)
    {
        MainCanvas = mainCanvas;
        MainCanvas.selectionBorder = new mBorder(MainCanvas);
        mainCanvas.PointerMoved += jCanvas_OnPointerMoved;
        selectedOriginalBackground = MainCanvas.Background;
  //      Utils.DebugTimer.Elapsed += DebugWorkspace;
     //   InitSelectionBorder();
        InitSelectionCanvas();
        InitSelectionRectangle();
        

    }

    #region Selection
    
    
    //Канвас выделения
    private static void InitSelectionCanvas()
    {
        SelectionCanvas = new jCanvas();
        SelectionCanvas.Background = Brushes.Aqua;
    }

    private static void InitSelectionRectangle()
    {
        SelectionRectangle = new Rectangle
        {
            Stroke = Brushes.Aqua,
            StrokeThickness = 3,
            Fill = Brushes.Chartreuse,
            Opacity = 0.1,
            IsHitTestVisible = false,
        };
        SelectionRectangle.IsVisible = false;
        MainCanvas.Children.Add(SelectionRectangle);
        SelectionRectangle.SetValue(Panel.ZIndexProperty, Int32.MaxValue);
        
    }
    //Выделить попавшие в рамку объекты
    private static void CheckSelectionRectIntersections()
    {
        var parentCanvas = SelectionRectangle.Parent as jCanvas;
        
        var selectionRect = new Rect(Canvas.GetLeft(SelectionRectangle), Canvas.GetTop(SelectionRectangle), SelectionRectangle.Width, SelectionRectangle.Height);

        foreach (var child in (parentCanvas).jChildren)
        {
            var controlRect = new Rect(Canvas.GetLeft(child as Control), Canvas.GetTop(child as Control), (child as Control).Bounds.Width,
                (child as Control).Bounds.Height);

                if (selectionRect.Intersects(controlRect))
                    BindSelectionBorder(child);
                
            
        }
        //Сам контейнер не выделять
        parentCanvas.selectionBorder.IsVisible = false;
        movable = null;
    }
    
    

    
    
    //Поместить выделенные элементы на метаканвас
    private static void ApplySelectionCanvas(List<JControl> objects, jCanvas parentCanvas)
    {
        
    }
    
    //Вернуть элементы с метаканваса выделения
    private static void RestoreSelectionCanvas()
    {
        
    }
    
    //Оптимизировать: не повторять определение размера при каждом перемещении
    public static void BindSelectionBorder(JControl obj)
    {
        if(obj is null) return;
        if(obj.selectionBorder is null) return;
        if (obj.Name == "MainCanvas")
        {
           // MainCanvas.selectionBorder.IsVisible = false;
           //Тут Broadcast на снятие всех выделений
           //Бля или нет
            return;
        }
        obj.selectionBorder.IsVisible = true;
        obj.selectionBorder.Width = obj.Bounds.Width;
        obj.selectionBorder.Height = obj.Bounds.Height;
        obj.selectionBorder.IsHitTestVisible = false;
        Point? position = ((Control)obj).TranslatePoint(new Point(0, 0), MainCanvas);
        Canvas.SetLeft(obj.selectionBorder,position.Value.X);
        Canvas.SetTop(obj.selectionBorder,position.Value.Y);
    }

    private static void CancelSelection()
    {
        if(SelectedObjects.Any())
            foreach (var obj in SelectedObjects)
                obj.selectionBorder.IsVisible = false;
        SelectedObjects.Clear();
    }
    
    #endregion
    
    public static void InitMovable(JControl obj)
    {
        CancelSelection();
        
        if (obj is null) return;
        History.AddHistoryItem(new History.Change(obj, 
            "Coordinates",
            new jCoordinates(Canvas.GetLeft(obj as Control),Canvas.GetTop(obj as Control))));
        movable = obj;
        mov_hw = obj.Bounds.Width / 2;
        mov_hh = obj.Bounds.Height / 2;
        BindSelectionBorder(movable);
    }

    
    public static void InitPremovable()
    {
        if(premovable is null) return;
        InitMovable(premovable);
        HierarchyControl.Selected = (premovable).mTreeItem;
        
    }

    private static void DebugWorkspace(Object source, ElapsedEventArgs e) =>
        Utils.PrintDebugMessage($"Movable: {movable?.Name} Premovable: {premovable?.Name} + LShift: {State.LShiftPressed}");
    
    
   public static void jCanvas_OnPointerMoved(object? sender, PointerEventArgs e)
   {
    
    if(Equals(sender,SelectionCanvas))
       return;
    e.Handled = true;

    if (State.LShiftPressed && e.GetCurrentPoint(MainCanvas).Properties.IsLeftButtonPressed)
    {
        DrawSelectionFrame(e);
        return;
    }
    else if (!State.LShiftPressed)
    {
        if(SelectionRectangle.IsVisible)
            CheckSelectionRectIntersections();
        SelectionRectangle.IsVisible = false;
    }

    if (movable == null) 
        return;
    // Проверка sender и основного холста
    if (Equals(movable, MainCanvas))
        return;

    // Проверка родительского элемента movable
    if (movable.jParent is not jCanvas parentCanvas || !IsPointerMoveEvent(e, parentCanvas))
        return;

    // Получение позиции указателя относительно родительского холста
    var mousePosition = e.GetPosition(parentCanvas);
    var element = movable as Control;

    if (element == null)
        return; 

    // Проверка состояния нажатия и наличия нажатой клавиши Ctrl
    if (movable.IsPressed)
    {
        if (State.LCtrlPressed)
            ResizeElement(e, element);
        else
            MoveElement(mousePosition, element, parentCanvas);
    }
    BindSelectionBorder(movable);
   }

    private static Point startPosition;

    private static void DrawSelectionFrame(PointerEventArgs e)
    {
        if (movable is not jCanvas)
            return;
        var currentCanvas = movable as jCanvas;
        // Сделаем рамку видимой, если она еще не видна
        SelectionRectangle.IsVisible = true;
        if (State.RectangleFlag)
        {
            (SelectionRectangle.Parent as Canvas).Children.Remove(SelectionRectangle);
            currentCanvas.Children.Add(SelectionRectangle);
            startPosition = e.GetPosition(currentCanvas);
            Canvas.SetLeft(SelectionRectangle, startPosition.X);
            Canvas.SetTop(SelectionRectangle, startPosition.Y);
            State.RectangleFlag = false;
        }
        

        // Установим ширину и высоту movable, если они NaN
        if (double.IsNaN(movable.Width))
            movable.Width = movable.Bounds.Width;
        if (double.IsNaN(movable.Height))
            movable.Height = movable.Bounds.Height;

        // Получим текущую позицию указателя мыши относительно MainCanvas
        var currentPosition = e.GetPosition(currentCanvas);
    
        // Вычисляем новую позицию и размеры рамки
        var x = Math.Min(startPosition.X, currentPosition.X);
        var y = Math.Min(startPosition.Y, currentPosition.Y);
        var width = Math.Abs(currentPosition.X - startPosition.X);
        var height = Math.Abs(currentPosition.Y - startPosition.Y);

        // Устанавливаем позицию и размеры SelectionRectangle
        Canvas.SetLeft(SelectionRectangle, x);
        Canvas.SetTop(SelectionRectangle, y);
        SelectionRectangle.Width = width;
        SelectionRectangle.Height = height;
    }


   // Проверка типа обновления указателя 
   private static bool IsPointerMoveEvent(PointerEventArgs e, jCanvas parentCanvas) =>
       e.GetCurrentPoint(parentCanvas).Properties.PointerUpdateKind == PointerUpdateKind.Other;

    private static void MoveElement(Point mousePosition, Control element, jCanvas parentCanvas)
    {
        // Установка позиции элемента с коррекцией координат
        Canvas.SetLeft(element, CorrectCoords(mousePosition.X - mov_hw));
        Canvas.SetTop(element, CorrectCoords(mousePosition.Y - mov_hh));

        // Ограничение элемента в пределах холста
        ConstrainElementWithinCanvas(element, parentCanvas);
    }
    //Ограничить перемещение в пределах канваса
    private static void ConstrainElementWithinCanvas(Control element, jCanvas parentCanvas)
    {
        // Ограничение позиции элемента в пределах границ холста
        var elementLeft = Canvas.GetLeft(element);
        var elementTop = Canvas.GetTop(element);
        var elementRight = elementLeft + 2 * mov_hw;
        var elementBottom = elementTop + 2 * mov_hh;

        if (elementLeft < 0)
            Canvas.SetLeft(element, 0);
        if (elementTop < 0)
            Canvas.SetTop(element, 0);
        if (elementRight > parentCanvas.Bounds.Width)
            Canvas.SetLeft(element, parentCanvas.Bounds.Width - 2 * mov_hw);
        if (elementBottom > parentCanvas.Bounds.Height)
            Canvas.SetTop(element, parentCanvas.Bounds.Height - 2 * mov_hh);
    }

    private static void ResizeElement(PointerEventArgs e, Control element)
    {
        //Поправить флаг!!
        if (State.ResizeFlag)
        {
            if (History.UndoList.Any())
                History.UndoList.Remove(History.UndoList.Last());

            History.AddHistoryItem(new History.Change(movable, "Size", new jSize(element.Bounds.Width, element.Bounds.Height)));
        }

        State.ResizeFlag = false;

        if (double.IsNaN(element.Width))
            element.Width = element.Bounds.Width;

        var mousePosition = e.GetPosition(element);

        if (mousePosition.X < 5 || mousePosition.Y < 5)
            return;

        element.Width = CorrectSize(mousePosition.X);
        element.Height = CorrectSize(mousePosition.Y);
    }

    
    public static void jElementClick(object? sender, RoutedEventArgs e)
    {
        e.Handled = true;
        InitMovable((JControl)sender!);
        HierarchyControl.Selected = ((JControl)sender!).mTreeItem;
    }

    public static void OnjControlPointerEntered(object? sender, PointerEventArgs e)
    {
        e.Handled = true;
        if (!(((JControl)sender).Type.Contains("Button") || ((JControl)sender).Type.Contains("CheckBox")))
            return;


        premovable = sender as JControl;
    }

    public static void OnjControlPointerExited(object? sender, PointerEventArgs e)
    {
        e.Handled = true;
        premovable = null;
    }

    public static void RemoveSelectedjElementHandler(object? sender, RoutedEventArgs e)
        => RemoveSelectedjElement();
    public static void RemoveSelectedjElement()
    {
        if (HierarchyControl.Selected == MainCanvas.mTreeItem) return;
        var element = HierarchyControl.Selected.element;
        var jparent = HierarchyControl.Selected.element.jParent;
        jparent.RemoveChild(HierarchyControl.Selected.element);
        
        var parent = HierarchyControl.Selected.Parent as mTreeViewItem;
        parent.Items.Remove(HierarchyControl.Selected);
        HierarchyControl.Selected  = (jparent.jChildren.Count > 0) ? jparent.jChildren.Last().mTreeItem : ((JControl)jparent).mTreeItem;
        
        var data = new Object[] {jparent,element,element.mTreeItem};
        History.AddHistoryItem(new History.Change(element,"Removed",data));
        // MainHierarchyTree.SelectedItem=selectedTreeItem.element.mTreeItem;
        element.Dispose();
    }
    
    public static void OnjControlPressed(object? sender, PointerPressedEventArgs e)
    {
        e.Handled = true;
        
        Workspace.InitMovable((JControl)sender);
        var element = sender as JControl;
        element.IsPressed = true;
        HierarchyControl.Selected = (element).mTreeItem;
    }

    public static void OnjControlReleased(object? sender, PointerReleasedEventArgs e)
    {
        e.Handled = true;
        var element = sender as JControl;
        element.IsPressed = false;
    }
    
    // Выбрать редактируемый элемент
    public static void SelectjElement(JControl element)
    {
        if ((element is null) || (element.Name == null)) return;
        HierarchyControl.Selected = element.mTreeItem;
        HierarchyControl.Selected = HierarchyControl.Selected;
        selectedOriginalBackground = HierarchyControl.Selected.element.Background;
        InitMovable(HierarchyControl.Selected.element);
        HierarchyControl.Selected.element.Focus();
        PropertiesControl.ShowProperties();
    }
    
    //Корректировка координат для перемещения и растяжения в строгом режиме
    public static double CorrectCoords(double coord)
    {
        ///ВАЖНО///
        if (!State.StrictModeEnabled) return Math.Round(coord);
        if(State.StrictModeValue == 0) return Math.Round(coord);
        if (State.StrictModeValue <= 0) return coord;
        var _coord = ( ( (int)coord) / State.StrictModeValue) * State.StrictModeValue;
        return _coord;

    }

    public static double CorrectSize(double coord)
        => CorrectCoords(coord) > 0 ? CorrectCoords(coord) : State.StrictModeValue;
    
}