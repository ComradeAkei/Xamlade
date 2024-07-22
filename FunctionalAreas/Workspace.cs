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
using Xamlade.Extensions;
using Xamlade.jClasses;

namespace Xamlade.FunctionalAreas;

public static class Workspace
{
    
    public static jCanvas MainCanvas { get; set; }

    private static List<JControl> SelectedList = new List<JControl>(); 
    
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
        Utils.DebugTimer.Elapsed += DebugWorkspace;
     //   InitSelectionBorder();
        InitSelectionCanvas();
        InitSelectionRectangle();
        

    }

    #region Selection
    
    
    //Канвас выделения
    private static void InitSelectionCanvas()
    {
        SelectionCanvas = new jCanvas();
        SelectionCanvas.Name = "SelectionCanvas";
        SelectionCanvas.Background = Brushes.Transparent;
        SelectionCanvas.SetValue(Panel.ZIndexProperty, Int32.MaxValue-5);
        SelectionCanvas.PointerEntered += OnjControlPointerEntered;
        SelectionCanvas.PointerExited += OnjControlPointerExited;
        SelectionCanvas.PointerPressed += OnjControlPressed;
        SelectionCanvas.PointerReleased += OnjControlReleased;
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
            {
                BindSelectionBorder(child);
                SelectedList.Add(child);
            }

        }
        //Сам контейнер не выделять
        parentCanvas.selectionBorder.IsVisible = false;
        SelectedList.Remove(parentCanvas);
        ApplySelectionCanvas();
    }
    
    //Поместить выделенные элементы на метаканвас
    private static void ApplySelectionCanvas()
    {
        SelectionCanvas.IsVisible = true;
        if (SelectedList.Count < 2) {movable = null; return;}
        var parent = SelectedList[0].jParent;
        // Проверить, что все выделенные элементы находятся в одном контейнере
        foreach (var obj in SelectedList)
        {
            if (obj.jParent != parent)
            {
                movable = null; 
                return;
            }
        }
        parent.AddChild(SelectionCanvas);

        // Найти границы (минимальные и максимальные координаты) всех элементов в SelectedList
        double minX = double.MaxValue, minY = double.MaxValue;
        double maxX = double.MinValue, maxY = double.MinValue;

        foreach (var obj in SelectedList)
        {
            var bounds = obj.Bounds;
            minX = Math.Min(minX, bounds.X);
            minY = Math.Min(minY, bounds.Y);
            maxX = Math.Max(maxX, bounds.X + bounds.Width);
            maxY = Math.Max(maxY, bounds.Y + bounds.Height);
        }

        // Установить размеры и положение SelectionCanvas
        Canvas.SetLeft(SelectionCanvas, minX);
        Canvas.SetTop(SelectionCanvas, minY);
        SelectionCanvas.Width = maxX - minX;
        SelectionCanvas.Height = maxY - minY;

        // Переместить все объекты в SelectionCanvas, сохраняя их абсолютные координаты
        foreach (var obj in SelectedList)
        {
            var absX = Canvas.GetLeft(obj as Control);
            var absY = Canvas.GetTop(obj as Control);
            parent.RemoveChild(obj);
           
            SelectionCanvas.AddChild(obj);
            Canvas.SetLeft(obj as Control, absX - minX);
            Canvas.SetTop(obj as Control, absY - minY);
            (obj as Control).IsHitTestVisible = false;
            (obj as JSelectable).selectionBorder.IsVisible = true;
        }
        
    }



    //Вернуть элементы с метаканваса выделения
    public static void RestoreSelectionCanvas()
    {
        SelectionCanvas.IsVisible = false;
        if (SelectionCanvas.Parent == null)
            return;

        var parent = SelectionCanvas.Parent as jCanvas;
        double selCanvasLeft = Canvas.GetLeft(SelectionCanvas);
        double selCanvasTop = Canvas.GetTop(SelectionCanvas);

        // Переместить все объекты обратно в родительский canvas, восстанавливая их абсолютные координаты
        var children = SelectionCanvas.jChildren.ToList(); // Создаем копию списка, чтобы избежать модификации коллекции во время итерации

        foreach (var child in children)
        {
            var relX = Canvas.GetLeft(child as Control);
            var relY = Canvas.GetTop(child as Control);
            SelectionCanvas.RemoveChild(child);
            parent.AddChild(child);
            Canvas.SetLeft(child as Control, relX + selCanvasLeft);
            Canvas.SetTop(child as Control, relY + selCanvasTop);
            (child as Control).IsHitTestVisible = true;
            (child as JSelectable).selectionBorder.IsVisible = false;
        }

        // Удалить SelectionCanvas из родительского canvas
        parent.RemoveChild(SelectionCanvas);
    }
    
    //Оптимизировать: не повторять определение размера при каждом перемещении
    public static void BindSelectionBorder(JControl obj)
    {
        if(obj is null) return;
        if((obj as JSelectable).selectionBorder is null) return;
        if (obj.Name == "MainCanvas")
        {
           // MainCanvas.selectionBorder.IsVisible = false;
           //Тут Broadcast на снятие всех выделений
           //Бля или нет
            return;
        }
        (obj as JSelectable).selectionBorder.IsVisible = true;
        (obj as JSelectable).selectionBorder.Width = obj.Bounds.Width;
        (obj as JSelectable).selectionBorder.Height = obj.Bounds.Height;
        (obj as JSelectable).selectionBorder.IsHitTestVisible = false;
        Point? position = ((Control)obj).TranslatePoint(new Point(0, 0), MainCanvas);
        position ??= new Point(0, 0); 
        Canvas.SetLeft((obj as JSelectable).selectionBorder,position.Value.X);
        Canvas.SetTop((obj as JSelectable).selectionBorder,position.Value.Y);
    }

    private static void CancelSelection()
    {
        if(SelectedList.Any())
            foreach (var obj in SelectedList)
                if (obj.Name != "SelectionCanvas")
                    if((obj as JSelectable).selectionBorder is not null) 
                        (obj as JSelectable).selectionBorder.IsVisible = false;
        SelectedList.Clear();
    }
    
    #endregion
    
    public static void InitMovable(JControl obj)
    {
        if(!Equals(SelectionCanvas,obj))
            if(SelectionCanvas.IsVisible)
                RestoreSelectionCanvas();
        //Дёргается не из-за этого
         CancelSelection();
         if(Equals(SelectionCanvas,obj))
             foreach (var child in SelectionCanvas.jChildren)
                 (child as JSelectable).selectionBorder.IsVisible = true;
        
        if (obj is null) return;
        if (obj.jParent is jBorder)
            obj = obj.jParent as JControl;
        History.AddHistoryItem(new History.Change(obj, 
            "Coordinates",
            new jCoordinates(Canvas.GetLeft(obj as Control),Canvas.GetTop(obj as Control))));
        movable = obj;
        mov_hw = obj.Bounds.Width / 2;
        mov_hh = obj.Bounds.Height / 2;
        BindSelectionBorder(movable);
        SelectedList.Add(movable);
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
    if(Equals(movable?.jParent,SelectionCanvas))
        return;

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
            ResizeElement(e, element as JControl);
        else
            MoveElement(mousePosition, element, parentCanvas);
    }
    BindSelectionBorder(movable);
    if(Equals(movable,SelectionCanvas))
        foreach (var obj in SelectionCanvas.jChildren)
        {
            var position = ((Control)obj).TranslatePoint(new Point(0, 0), MainCanvas);
            Canvas.SetLeft((obj as JSelectable).selectionBorder,position.Value.X);
            Canvas.SetTop((obj as JSelectable).selectionBorder,position.Value.Y);
        }
   }

    private static Point startPosition = new Point(0,0);

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

    
    private static Point ResizeStartPosition = new Point(0,0);
    private static void ResizeElement(PointerEventArgs e, JControl element)
    {
        if (State.ResizeFlag)
        {
            
            if (History.UndoList.Any())
                History.UndoList.Remove(History.UndoList.Last());

            History.AddHistoryItem(new History.Change(movable, "Size", new jSize(element.Bounds.Width, element.Bounds.Height)));
        }

        if (State.NewResizeFlag)
        {
            startPosition = new Point(Canvas.GetLeft(element as Control), Canvas.GetTop(element as Control));
            State.NewResizeFlag = false;
        }

        State.ResizeFlag = false;

        if (double.IsNaN(element.Width))
            element.Width = element.Bounds.Width;
        if (double.IsNaN(element.Height))
            element.Height = element.Bounds.Height;

        var currentCanvas = element.jParent as Canvas;

        // Получим начальную позицию элемента
        

        // Получим текущую позицию указателя мыши относительно родительского элемента
        var mousePosition = e.GetPosition(currentCanvas);

        // Вычислим новую позицию и размеры элемента
        var newLeft = Math.Min(startPosition.X, mousePosition.X);
        var newTop = Math.Min(startPosition.Y, mousePosition.Y);
        var newWidth = Math.Abs(mousePosition.X - startPosition.X);
        var newHeight = Math.Abs(mousePosition.Y - startPosition.Y);

        // Ограничиваем минимальные размеры элемента
        newWidth = Math.Max(10, newWidth);
        newHeight = Math.Max(10, newHeight);

        // Устанавливаем новые размеры и позицию элемента
        Canvas.SetLeft(element as Control, newLeft);
        Canvas.SetTop(element as Control, newTop);
        element.Width = CorrectSize(newWidth);
        element.Height = CorrectSize(newHeight);
        // Ограничение элемента в пределах холста
        ConstrainElementWithinCanvas(element as Control, (element.jParent) as jCanvas);
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
        if (!(((JControl)sender).Type.Contains("Button") || ((JControl)sender).Type.Contains("CheckBox")|| ((JControl)sender).Type.Contains("ComboBox")))
            return;


        premovable = sender as JControl;
    }

    public static void OnjControlPointerExited(object? sender, PointerEventArgs e)
    {
        e.Handled = true;
        premovable = null;
        PropertiesControl.ShowProperties();
    }

    public static void RemoveSelectedjElementHandler(object? sender, RoutedEventArgs e)
        => RemoveSelectedjElement();
    public static void RemoveSelectedjElement()
    {
        
        if (SelectionCanvas.jChildren.Count > 1)
        {
            foreach (var child in SelectionCanvas.jChildren)
            {
                (child as JSelectable).selectionBorder.IsVisible = false;
                (child?.mTreeItem?.Parent as mTreeViewItem)?.Items?.Remove(child?.mTreeItem);
            }

            var count = SelectionCanvas.jChildren.Count;
            for(int i = 0; i < count;i++)
                SelectionCanvas.RemoveChild(SelectionCanvas.jChildren[0]);
            return;
        }
        else if (movable is jBorder border)
        {
            border.Remove();
            return;
        }

        if (HierarchyControl.Selected == MainCanvas.mTreeItem) return;
        
        
        var element = HierarchyControl.Selected.element;
        var jparent = HierarchyControl.Selected.element.jParent;
        jparent.RemoveChild(element);
        var parent = element.mTreeItem.Parent as mTreeViewItem;
        parent.Items.Remove(element.mTreeItem);
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
    
    
    
    //TODO сделать универсальным
    public static T FindMainCanvasChildByName<T>(string name) where T : JControl => MainCanvas.jChildren.OfType<T>().FirstOrDefault(child => child.Name == name);
}