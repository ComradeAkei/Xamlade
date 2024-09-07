using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Xamlade.Extensions;
using Xamlade.LinkWorkers;
using Xamlade.XAMLWorkers;

namespace Xamlade.jClasses;

public class jCanvas : Canvas, JChildContainer, JControl, JBroadcastHandler<JControl>, JSelectable
{
    protected override Type StyleKeyOverride => typeof(Canvas);

    public bool IsPressed { get; set; }
    public event EventHandler<RoutedEventArgs>? Click;
    private string controlType => jElementType.Canvas.ToString();
    public string Type => controlType;


    public List<string> XAMLPiece { get; set; }
    public mBorder selectionBorder { get; set; }
    public Beholder Beholder { get; set; }
    public Dictionary<string, JChildContainer.ContainerSetPropertyDelegate> SpecialSetDelegates { get; set; }
    public JChildContainer? _jParent { get; set; }
   // public JChildContainer? jParent { get; set; }
    public List<(string, JChildContainer.ContainerPropertyDelegate)> ContainerProperties { get; set; }
    public static Dictionary<string, JChildContainer.ContainerSetPropertyDelegate> ContainerSetProperties { get; set; }
    public List<JControl> jChildren { get; }

    static jCanvas()
    {
        ContainerSetProperties = new Dictionary<string, JChildContainer.ContainerSetPropertyDelegate>
        {
            { "Top", Utils.ConvertSetter<int>(SetTop) },
            { "Left", Utils.ConvertSetter<int>(SetLeft) },
        };
    }
    public void InitContainerProperties()
    {
        ContainerProperties = new List<(string, JChildContainer.ContainerPropertyDelegate)>
        {
            ("Top", Utils.ConvertGetter(GetTop)),
            ("Left", Utils.ConvertGetter(GetLeft)),
        }; 
    }
    public jCanvas()
    {
        SpecialSetDelegates = new();
        jChildren = new List<JControl>();
        Broadcast.OnBroadcast += (this as JBroadcastHandler<JControl>).HandleBroadcast;
        XAMLPiece = new List<string>();
    }

    public static void SetTop(JControl element, int value)
    {
        if (element == null) throw new ArgumentNullException(nameof(element));
        Canvas.SetTop(element as Control, value);
    }

    public static void SetLeft(JControl element, int value)
    {
        if (element == null) throw new ArgumentNullException(nameof(element));
        Canvas.SetLeft(element as Control, value);
    }

    public static void SetRight(JControl element, double value)
    {
        if (element == null) throw new ArgumentNullException(nameof(element));
        Canvas.SetRight(element as Control, value);
    }

    public static void SetBottom(JControl element, double value)
    {
        if (element == null) throw new ArgumentNullException(nameof(element));
        Canvas.SetBottom(element as Control, value);
    }

    
    public static int GetLeft(JControl element) =>
        double.IsNaN((int)Canvas.GetLeft(element as Control)) ? 0 : (int)Canvas.GetLeft(element as Control);

    public static double GetRight(JControl element)
    {
        if (element == null) throw new ArgumentNullException(nameof(element));
        return Canvas.GetRight(element as Control);
    }

    public static int GetTop(JControl element)
    {
        if (element == null) throw new ArgumentNullException(nameof(element));
        return  double.IsNaN((int)Canvas.GetTop(element as Control)) ? 0 : (int)Canvas.GetTop(element as Control);;
    }

    public static double GetBottom(JControl element)
    {
        if (element == null) throw new ArgumentNullException(nameof(element));
        return Canvas.GetBottom(element as Control);
    }
    
    public void AddChild(JControl child)
    {
        jChildren.Add(child);
        child.jParent = this;
       // child.AddContainerProperties();
        
        //Навесить делегаты свойств контейнера в словарь специальных свойств объекта
      //  foreach (var kvp in ContainerSetProperties)
     //       child.SpecialSetDelegates.TryAdd(kvp.Key, kvp.Value);
        
        //   Console.WriteLine(child.GetType().ToString());
        Children.Add((Control)child);
    }

    public void AddChild(JControl child, int top = 0, int left = 0)
    {
        jChildren.Add(child);
        child.jParent = this;
        SetTop(child, top);
        SetLeft(child, left);
        //   Console.WriteLine(child.GetType().ToString());
        Children.Add((Control)child);
    }

    public void RemoveChild(JControl child)
    {
        jChildren.Remove(child);
        Children.Remove((Control)child);
    }

    public int XAMLRating { get; set; }

    public Dictionary<string, Dictionary<string, Property>> xPropertiesGroup { get; set; }

    public void AddSpecialProperties()
    {
        return;
    }
}