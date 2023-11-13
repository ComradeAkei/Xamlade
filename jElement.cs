using System;
using System.Collections.Generic;
using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.LogicalTree;
using Avalonia.Media;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Xamlade;


public enum jElementType
{
    Border,
    Canvas,
    DockPanel,
    Grid,
    Panel,
    ScrollViewer,
    StackPanel,
    TabControl,
    TabItem,
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
    TextBlock,
    Image
}


// Для контейнеров
public interface IChildContainer
{
    List<JControl> jChildren { get; }
    void AddChild(JControl child, double top = 0, double left = 0);

    public void RemoveChild(JControl child);

}
//Для простых объектов
public interface JControl
{
    public IChildContainer? jParent { get; set; }
    public string Type { get; }
    
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

   public event EventHandler<PointerEventArgs>? PointerEntered;
   public event EventHandler<PointerEventArgs>? PointerExited;
   public event EventHandler<RoutedEventArgs>? Click;
   public event EventHandler<PointerPressedEventArgs>? PointerPressed;
   public event EventHandler<PointerReleasedEventArgs>? PointerReleased;
   public event EventHandler<KeyEventArgs>? KeyDown;
   public event EventHandler<KeyEventArgs>? KeyUp;

   
   public void SetParent(IChildContainer parent)
   {
       if(parent==null) return;
       if(this.Name == "MainCanvas") return;
       double top = 0;
       double left = 0;

       if (((Control)this).Parent != null)
       {
           if (((Control)this).Parent is Canvas)
           {
               top = Canvas.GetTop((Control)this);
               left = Canvas.GetLeft((Control)this);
           }

           var _parent = ((Control)this).Parent as Panel;
           _parent.Children.Remove((Control)this);
       }

       var oldParent = this.jParent;
       if (oldParent != null)
       {
           oldParent.RemoveChild(this);
       }
       parent.AddChild(this,top,left);
   }
   
   public bool Focus(NavigationMethod method = NavigationMethod.Unspecified,
       KeyModifiers keyModifiers = KeyModifiers.None);
   
   public void Dispose()
   {
       if(this.Name == "MainCanvas") return;
       mTreeItem = null;
       Reflector.SetName(null, this);
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
    private string controlType => jElementType.Button.ToString();
    public string Type => controlType;
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
        Broadcast.OnBroadcast += HandleBroadcast;
        XAMLPiece = new List<string>();
        mTreeItem = new mTreeViewItem(this);
    }
    
    private void HandleBroadcast(int mode)
    {
        if(mode == 0) XAMLGenerator.XAMLRatingInit(this);
        else if (mode == 1) XAMLGenerator.XAMLize(this);
        else if (mode == 2) MainWindow._MainWindow.CorrectLoadedjElement(this);
        else if (mode == 3)
        {
            Broadcast.OnBroadcast -= HandleBroadcast; 
            Broadcast.DisposeElement(this);
        }
        else if (mode == 4) MainWindow._MainWindow.CorrectTree(this);
    }
    
}

public class jImage : Image, JControl
{
    protected override Type StyleKeyOverride => typeof(Image);
    public jImage()
    {
        Broadcast.OnBroadcast += HandleBroadcast;
        XAMLPiece = new List<string>();
        mTreeItem = new mTreeViewItem(this);
        
    }
    public IChildContainer? jParent { get; set; }
    
    private string controlType => jElementType.Image.ToString();
    public string Type => controlType;
    public mTreeViewItem? mTreeItem { get; set; }
    public int XAMLRating { get; set; }
    public List<string> XAMLPiece { get; set; }
    //Понизили доступ к свойству Background, лол. Хотя интерфейс регламентирует public
    IBrush? JControl.Background { get; set; }
    public bool IsPressed { get; set; }
    public event EventHandler<RoutedEventArgs>? Click;
    
    public string? jImageSource { get; set; }
    
    private void HandleBroadcast(int mode)
    {
        if(mode == 0) XAMLGenerator.XAMLRatingInit(this);
        else if (mode == 1) XAMLGenerator.XAMLize(this);
        else if (mode == 2) MainWindow._MainWindow.CorrectLoadedjElement(this);
        else if (mode == 3)
        {
            Broadcast.OnBroadcast -= HandleBroadcast; 
            Broadcast.DisposeElement(this);
        }
        else if (mode == 4) MainWindow._MainWindow.CorrectTree(this);
    }
}


public class jToggleButton : ToggleButton, JControl
{
    protected override Type StyleKeyOverride => typeof(ToggleButton);
    public jToggleButton()
    {
        Broadcast.OnBroadcast += HandleBroadcast;
        XAMLPiece = new List<string>();
        mTreeItem = new mTreeViewItem(this);
    }
    private string controlType => jElementType.ToggleButton.ToString();
    public string Type => controlType;
    public IChildContainer? jParent { get; set; }
    public mTreeViewItem? mTreeItem { get; set; }
    public int XAMLRating { get; set; }
    public List<string> XAMLPiece { get; set; }
    public new bool IsPressed
    {
        get => base.IsPressed;
        set => SetValue(IsPressedProperty, value);
    }
    
    private void HandleBroadcast(int mode)
    {
        if(mode == 0) XAMLGenerator.XAMLRatingInit(this);
        else if (mode == 1) XAMLGenerator.XAMLize(this);
        else if (mode == 2) MainWindow._MainWindow.CorrectLoadedjElement(this);
        else if (mode == 3)
        {
            Broadcast.OnBroadcast -= HandleBroadcast; 
            Broadcast.DisposeElement(this);
        }
        else if (mode == 4) MainWindow._MainWindow.CorrectTree(this);
    }
    protected override void OnClick() {}
}

public class jCheckBox : CheckBox, JControl
{
    public jCheckBox()
    {
        Broadcast.OnBroadcast += HandleBroadcast;
        XAMLPiece = new List<string>();
        mTreeItem = new mTreeViewItem(this);
    }

    protected override Type StyleKeyOverride => typeof(CheckBox);
    public IChildContainer? jParent { get; set; }
    
    private string controlType => jElementType.CheckBox.ToString();
    public string Type => controlType;
    public mTreeViewItem? mTreeItem { get; set; }
    
    public int XAMLRating { get; set; }
    public List<string> XAMLPiece { get; set; }

    public new bool IsPressed
    {
        get => base.IsPressed;
        set => SetValue(IsPressedProperty, value);
    }
    private void HandleBroadcast(int mode)
    {
        if(mode == 0) XAMLGenerator.XAMLRatingInit(this);
        else if (mode == 1) XAMLGenerator.XAMLize(this);
        else if (mode == 2) MainWindow._MainWindow.CorrectLoadedjElement(this);
        else if (mode == 3)
        {
            Broadcast.OnBroadcast -= HandleBroadcast; 
            Broadcast.DisposeElement(this);
        }
        else if (mode == 4) MainWindow._MainWindow.CorrectTree(this);
    }

    protected override void OnClick() {}
}

public class jTextBlock : TextBlock, JControl
{
    public jTextBlock()
    {
        Broadcast.OnBroadcast += HandleBroadcast;
        XAMLPiece = new List<string>();
        mTreeItem = new mTreeViewItem(this);
    }

    protected override Type StyleKeyOverride => typeof(TextBlock);
    public IChildContainer? jParent { get; set; }
    private string controlType => jElementType.TextBlock.ToString();
    public string Type => controlType;
    public mTreeViewItem? mTreeItem { get; set; }
    
    public int XAMLRating { get; set; }
    public List<string> XAMLPiece { get; set; }

    private void HandleBroadcast(int mode)
    {
        if(mode == 0) XAMLGenerator.XAMLRatingInit(this);
        else if (mode == 1) XAMLGenerator.XAMLize(this);
        else if (mode == 2) MainWindow._MainWindow.CorrectLoadedjElement(this);
        else if (mode == 3)
        {
            Broadcast.OnBroadcast -= HandleBroadcast; 
            Broadcast.DisposeElement(this);
        }
        else if (mode == 4) MainWindow._MainWindow.CorrectTree(this);
    }
 
    public bool IsPressed { get; set; }
    public event EventHandler<RoutedEventArgs>? Click;
}

public class jTextBox : TextBox, JControl
{
    public jTextBox()
    {
        Broadcast.OnBroadcast += HandleBroadcast;
        XAMLPiece = new List<string>();
        mTreeItem = new mTreeViewItem(this);
    }

    protected override Type StyleKeyOverride => typeof(TextBox);
    public IChildContainer? jParent { get; set; }
    private string controlType => jElementType.TextBox.ToString();
    public string Type => controlType;
    public mTreeViewItem? mTreeItem { get; set; }

    public int XAMLRating { get; set; }
    public List<string> XAMLPiece { get; set; }

    private void HandleBroadcast(int mode)
    {
        if (mode == 0) XAMLGenerator.XAMLRatingInit(this);
        else if (mode == 1) XAMLGenerator.XAMLize(this);
        else if (mode == 2) MainWindow._MainWindow.CorrectLoadedjElement(this);
        else if (mode == 3)
        {
            Broadcast.OnBroadcast -= HandleBroadcast; 
            Broadcast.DisposeElement(this);
        }
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e) { }
    protected override void OnPointerMoved(PointerEventArgs e) { }
    protected override void OnPointerReleased(PointerReleasedEventArgs e) { }

    public bool IsPressed { get; set; }
    public event EventHandler<RoutedEventArgs>? Click;
}
public class jCanvas : Canvas, IChildContainer, JControl
{ 
    protected override Type StyleKeyOverride => typeof(Canvas); 
    
    public bool IsPressed { get; set; }
    public event EventHandler<RoutedEventArgs>? Click;
    private string controlType => jElementType.Canvas.ToString();
    public string Type => controlType;
    public mTreeViewItem? mTreeItem { get; set; }
  
    public List<string> XAMLPiece { get; set; }
    public IChildContainer? jParent { get; set; }
    public List<JControl> jChildren { get; }
    public jCanvas()
    {
        jChildren = new List<JControl>();
        Broadcast.OnBroadcast += HandleBroadcast;
        XAMLPiece = new List<string>();
        mTreeItem = new mTreeViewItem(this);
    }

    public void AddChild(JControl child, double top = 0, double left = 0)
    {
        jChildren.Add(child);
        child.jParent = this;
        SetTop((Control)child,top);
        SetLeft((Control)child, left);
     //   Console.WriteLine(child.GetType().ToString());
        Children.Add((Control)child);
    }
    public void RemoveChild(JControl child)
    {
        jChildren.Remove(child);
        Children.Remove((Control)child);
    }
    public int XAMLRating { get; set; }
    private void HandleBroadcast(int mode)
    {
        
        if(mode == 0) XAMLGenerator.XAMLRatingInit(this);
        else if (mode == 1) XAMLGenerator.XAMLize(this);
        else if (mode == 2) MainWindow._MainWindow.CorrectLoadedjElement(this);
        else if (mode == 3)
        {
            if(Name=="MainCanvas") return;
            Broadcast.OnBroadcast -= HandleBroadcast; 
            Broadcast.DisposeElement(this);
        }
        else if (mode == 4) MainWindow._MainWindow.CorrectTree(this);
    }

}

public class jStackPanel : StackPanel, JControl, IChildContainer
{
    protected override Type StyleKeyOverride => typeof(StackPanel); 
    public IChildContainer? jParent { get; set; }
    private string controlType => jElementType.StackPanel.ToString();
    public string Type => controlType;
    public mTreeViewItem? mTreeItem { get; set; }
    public int XAMLRating { get; set; }
    public List<string> XAMLPiece { get; set; }
    public bool IsPressed { get; set; }
    public event EventHandler<RoutedEventArgs>? Click;
    public List<JControl> jChildren { get; }

    public jStackPanel()
    {
        jChildren = new List<JControl>();
        Broadcast.OnBroadcast += HandleBroadcast;
        XAMLPiece = new List<string>();
        mTreeItem = new mTreeViewItem(this);
    }

    public void AddChild(JControl child, double top = 0, double left = 0)
    {
        jChildren.Add(child);
        child.jParent = this;
        Children.Add((Control)child);
    }

    public void RemoveChild(JControl child)
    {
        jChildren.Remove(child);
        Children.Remove((Control)child);
    }
    
    private void HandleBroadcast(int mode)
    {
        if(mode == 0) XAMLGenerator.XAMLRatingInit(this);
        else if (mode == 1) XAMLGenerator.XAMLize(this);
        else if (mode == 2) MainWindow._MainWindow.CorrectLoadedjElement(this);
        else if (mode == 3)
        {
            Broadcast.OnBroadcast -= HandleBroadcast; 
            Broadcast.DisposeElement(this);
        }
        else if (mode == 4) MainWindow._MainWindow.CorrectTree(this);
    }
}