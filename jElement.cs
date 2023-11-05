using System;
using System.Collections.Generic;
using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Xamlade;


public enum ContainerType
{
    Border,
    Canvas,
    DockPanel,
    Grid,
    Panel,
    ScrollViewer,
    StackPanel,
    TabControl,
    TabItem
}

public enum ControlType
{
    Button,
    CheckBox,
    ComboBox,
    DatePicker,
    ListBox,
    ListView,
    Menu,
    MenuItem,
    ProgressBar,
    RadioButton,
    Slider,
    TextBox,
    ToggleButton,
    TextBlock
}


// Для контейнеров
public interface IChildContainer
{
    List<JControl> jChildren { get; }
    void AddChild(JControl child);
    public void RemoveChild(JControl child);
    
}
//Для простых объектов
public interface JControl
{
    public IChildContainer? jParent { get; set; }
    public object Type { get; }
    public mTreeViewItem? mTreeItem { get; set; }
    
    public int XAMLRating { get; set; }
    public List<string> XAMLPiece { get; set; }

    // Устанавливает фон элемента
    IBrush? Background { get; set; }
    

    // Указывает, активен ли элемент для взаимодействия с пользователем
    bool IsEnabled { get; set; }

    // Указывает, виден ли элемент
    bool IsVisible { get; set; }

    // Устанавливает прозрачность элемента (от 0 до 1)
    double Opacity { get; set; }

    // Устанавливает горизонтальное выравнивание элемента в его родительском контейнере
    HorizontalAlignment HorizontalAlignment { get; set; }

    // Устанавливает вертикальное выравнивание элемента в его родительском контейнере
    VerticalAlignment VerticalAlignment { get; set; }

    // Устанавливает ширину элемента
    double Width { get; set; }

    // Устанавливает высоту элемента
    double Height { get; set; }

    // Устанавливает минимально допустимые значения для ширины и высоты элемента
    double MinWidth { get; set; }
    double MinHeight { get; set; }

    // Устанавливает максимально допустимые значения для ширины и высоты элемента
    double MaxWidth { get; set; }
    double MaxHeight { get; set; }

    // Устанавливает форму указателя мыши, когда она находится над элементом
    Cursor? Cursor { get; set; }

    // Устанавливает контекст данных для элемента, который используется для привязки данных
    object? DataContext { get; set; }

    // Устанавливает уникальное имя элемента
    public string? Name { get; set; }
    
    public bool IsPressed { get; set; }
    
    public Rect Bounds { get; }
    // Позволяет присваивать один или несколько CSS-классов элементу
   // Classes Classes { get; set; }
   private void XAMLize(int mode)
   {
       if(mode == 0) XAMLGenerator.XAMLRatingInit(this);
       else if (mode == 1) XAMLGenerator.XAMLize(this);
   }
   public void Dispose()
   {
       FieldInfo privateField =
           typeof(StyledElement).GetField("_name", BindingFlags.NonPublic | BindingFlags.Instance);
       privateField.SetValue(this, null);
   }
   
    
}

//Модифицированные элементы дерева со встроенными jControl
public class mTreeViewItem : TreeViewItem
{
    protected override Type StyleKeyOverride => typeof(TreeViewItem); 
    public JControl element { get; set; }
    public mTreeViewItem(JControl element)
    {
        this.element = element;
        Header = element.Name;
        //Обратная связь с jElement
        element.mTreeItem = this;
    }
}
public class jButton : Button, JControl
{
    protected override Type StyleKeyOverride => typeof(Button);
    public IChildContainer? jParent { get; set; }
    private ControlType controlType => ControlType.Button;
    public object Type => controlType;
    public mTreeViewItem? mTreeItem { get; set; }
    public int XAMLRating { get; set; }
    public List<string> XAMLPiece { get; set; }


    public new bool IsPressed
    {
        get => base.IsPressed;
        set => SetValue(IsPressedProperty, value);
    }

    public new string? Name
    {
        get => base.Name;
        set => SetValue(NameProperty, value);
    }

    public jButton()
    {
        Broadcast.OnBroadcast += XAMLize;
        XAMLPiece = new List<string>();
    }
    
    private void XAMLize(int mode)
    {
        if(mode == 0) XAMLGenerator.XAMLRatingInit(this);
        else if (mode == 1) XAMLGenerator.XAMLize(this);
    }
   
}

public class jCheckBox : CheckBox, JControl
{
    public jCheckBox()
    {
        Broadcast.OnBroadcast += XAMLize;
        XAMLPiece = new List<string>();
    }

    protected override Type StyleKeyOverride => typeof(CheckBox);
    public IChildContainer? jParent { get; set; }
    
    private ControlType controlType => ControlType.CheckBox;
    public object Type => controlType;
    public mTreeViewItem? mTreeItem { get; set; }
    
    public int XAMLRating { get; set; }
    public List<string> XAMLPiece { get; set; }

    public new bool IsPressed
    {
        get => base.IsPressed;
        set => SetValue(IsPressedProperty, value);
    }
    private void XAMLize(int mode)
    {
        if(mode == 0) XAMLGenerator.XAMLRatingInit(this);
        else if (mode == 1) XAMLGenerator.XAMLize(this);
    }
    
}

public class jTextBlock : TextBlock, JControl
{
    public jTextBlock()
    {
        Broadcast.OnBroadcast += XAMLize;
        XAMLPiece = new List<string>();
    }

    protected override Type StyleKeyOverride => typeof(TextBlock);
    public IChildContainer? jParent { get; set; }
    private ControlType controlType => ControlType.TextBlock;
    public object Type => controlType;
    public mTreeViewItem? mTreeItem { get; set; }
    
    public int XAMLRating { get; set; }
    public List<string> XAMLPiece { get; set; }

    private void XAMLize(int mode)
    {
        if(mode == 0) XAMLGenerator.XAMLRatingInit(this);
        else if (mode == 1) XAMLGenerator.XAMLize(this);
    }
 
    public bool IsPressed { get; set; }
}

public class jTextBox : TextBox, JControl
{
    public jTextBox()
    {
        Broadcast.OnBroadcast += XAMLize;
        XAMLPiece = new List<string>();
    }

    protected override Type StyleKeyOverride => typeof(TextBox);
    public IChildContainer? jParent { get; set; }
    private ControlType controlType => ControlType.TextBox;
    public object Type => controlType;
    public mTreeViewItem? mTreeItem { get; set; }

    public int XAMLRating { get; set; }
    public List<string> XAMLPiece { get; set; }

    private void XAMLize(int mode)
    {
        if (mode == 0) XAMLGenerator.XAMLRatingInit(this);
        else if (mode == 1) XAMLGenerator.XAMLize(this);
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e) { }
    protected override void OnPointerMoved(PointerEventArgs e) { }
    protected override void OnPointerReleased(PointerReleasedEventArgs e) { }

    public bool IsPressed { get; set; }
}
public class jCanvas : Canvas, IChildContainer, JControl
{ 
    protected override Type StyleKeyOverride => typeof(Canvas); 
    
    public bool IsPressed { get; set; }
    private ContainerType containerType => ContainerType.Canvas;
    public object Type => containerType;
    public mTreeViewItem? mTreeItem { get; set; }
  
    public List<string> XAMLPiece { get; set; }
    public IChildContainer? jParent { get; set; }
    public List<JControl> jChildren { get; }
    public jCanvas()
    {
        jChildren = new List<JControl>();
        Broadcast.OnBroadcast += XAMLize;
        XAMLPiece = new List<string>();
    }

    public void AddChild(JControl child)
    {
        jChildren.Add(child);
        child.jParent = this;
        SetTop((Control)child,0);
        SetLeft((Control)child, 0);
     //   Console.WriteLine(child.GetType().ToString());
        Children.Add((Control)child);
    }
    public void RemoveChild(JControl child)
    {
        jChildren.Remove(child);
        Children.Remove((Control)child);
    }
    public int XAMLRating { get; set; }
    private void XAMLize(int mode)
    {
        if (this.Name == null) return;
        if(mode == 0) XAMLGenerator.XAMLRatingInit(this);
        else if (mode == 1) XAMLGenerator.XAMLize(this);
    }

}

