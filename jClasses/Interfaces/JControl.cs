using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Xamlade.Extensions;
using Xamlade.LinkWorkers;

namespace Xamlade.jClasses;

public interface JControl : JProperties
{
    public Beholder Beholder { get; set; }
    
   
    
    public Dictionary<string, JChildContainer.ContainerSetPropertyDelegate> SpecialSetDelegates { get; set; }
    public bool IsSelected => (this as JSelectable)?.selectionBorder.IsVisible ?? false;
    
    protected JChildContainer? _jParent { get; set; }
    
    public JChildContainer? jParent
    {
        get => _jParent;
        set
        {
            _jParent = value;
            AfterParentSet();
        }
    }


    public string Type { get; } 

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
    /*  public string? Name
      {
          get => this.Name;
          set { this.Beholder.mTreeItem.Name = value; }
      }
  */
    public string? Name { get; set; }
    public bool IsPressed { get; set; }

    public Rect Bounds { get; }


    public event EventHandler<PointerEventArgs>? PointerEntered;
    public event EventHandler<PointerEventArgs>? PointerExited;
    public event EventHandler<RoutedEventArgs>? Click;
    public event EventHandler<PointerPressedEventArgs>? PointerPressed;
    public event EventHandler<PointerReleasedEventArgs>? PointerReleased;
    public event EventHandler<KeyEventArgs>? KeyDown;
    public event EventHandler<KeyEventArgs>? KeyUp;
    public event EventHandler<AvaloniaPropertyChangedEventArgs>? PropertyChanged;
    

    private void AfterParentSet()
    {
        if ((this as JControl)?.jParent is not { } parent) return;
        xPropertiesGroup["container"].Clear();
        foreach (var kvp in ((jParent as JChildContainer).GetType()
                     .GetProperty("ContainerSetProperties", BindingFlags.Public | BindingFlags.Static)
                     ?.GetValue(jParent)) as Dictionary<string, JChildContainer.ContainerSetPropertyDelegate>)
            SpecialSetDelegates.TryAdd(kvp.Key, kvp.Value);
        AddContainerProperties();
        AddSpecialProperties();
        (this as JControl).Beholder.PropListItemsInit();
    }

    public void SetParent(JChildContainer parent)
    {
        if (parent is null) return;
        if (this.Name == "MainCanvas") return;
        this.jParent?.RemoveChild(this);
        parent.AddChild(this);
    }


    public bool Focus(NavigationMethod method = NavigationMethod.Unspecified,
        KeyModifiers keyModifiers = KeyModifiers.None);

    public void Dispose()
    {
        if (this.Name == "MainCanvas") return;

        //   mTreeItem.element=null;
        this.Beholder.mTreeItem = null;
        var parent = this.jParent;
        if (jParent != null)
            jParent.RemoveChild(this);
        Console.WriteLine(this.Name + " Disposed");
        Reflector.SetName(null, this);
    }

    public string? ToString() =>
            $"Type: {Type}, Name: {Name}, IsSelected: {IsSelected}, IsEnabled: {IsEnabled}, " +
            $"IsVisible: {IsVisible}, DataContext: {DataContext}, Bounds: {Bounds}";

    public void JControlInit()
    {
        InitProperties();
        if (this is JChildContainer container)
            container.InitContainerProperties();
        //СОБЫТИЕ ВЫЗЫВАЕТСЯ ДЛЯ БОЛЬШИНСТВА ИЗМЕНЕНИЙ ЗНАЧЕНИЙ ПОЛЕЙ AVALONIA UI! 
        this.PropertyChanged += Beholder.OnPropertyChanged;
    }
}