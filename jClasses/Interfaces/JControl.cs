using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Xamlade.Extensions;

namespace Xamlade.jClasses;

public interface JControl
{
    
    public bool IsSelected => (this as JSelectable)?.selectionBorder.IsVisible ?? false;
    public JChildContainer? jParent { get; set; }
    
    
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

   
    public void SetParent(JChildContainer parent)
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
            
             (parent as JControl).mTreeItem.Items.Add(this.mTreeItem);
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
       
        //   mTreeItem.element=null;
        this.mTreeItem = null;
        var parent = this.jParent;
        if(jParent != null)
            jParent.RemoveChild(this);
        Console.WriteLine(this.Name+" Disposed");
        Reflector.SetName(null, this);
       
    }

    public string ToString()
    {
        return $"Type: {Type}, Name: {Name}, IsSelected: {IsSelected}, IsEnabled: {IsEnabled}, IsVisible: {IsVisible}, DataContext: {DataContext}, Bounds: {Bounds}";
    }
}