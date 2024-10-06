//МОИ ИНТЕРФЕЙСЫ ИМЕНУЮТСЯ С J


//Повторное использование кода в классах

//Рамку вынести в jSelectable

//Добавить приоритеты отрисовки (задний - передний план)


using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Avalonia.Controls;
using Xamlade.Extensions;
using Xamlade.Extensions.Atributes;
using Xamlade.LinkWorkers;
using Xamlade.XAMLWorkers;


namespace Xamlade.jClasses;

// Для контейнеров

//Интерфейс рамки выделения объекта

//Для простых объектов

//Приёмник широковещательных сообщений jObject

public class jButton : Button, JControl, JBroadcastHandler<JControl>, JSelectable
{
    protected override Type StyleKeyOverride => typeof(Button);
    
    
    public mBorder selectionBorder { get; set; }
    public Beholder Beholder { get; set; }
    public Dictionary<string, JChildContainer.ContainerSetPropertyDelegate> SpecialSetDelegates { get; set; }
   
    public JChildContainer? _jParent { get; set; }

    private string controlType => jElementType.Button.ToString();
    public string Type => controlType;
    public int XAMLRating { get; set; }
    public List<string> XAMLPiece { get; set; }
    

    public new bool IsPressed
    {
        get => base.IsPressed;
        set => Reflector.ForceSet(this,"_isPressed", value);
    }

    public new string? Name
    {
        get => base.Name;
        set => SetValue(NameProperty, value);
    }
    

    public Dictionary<string, Dictionary<string, Property>> xPropertiesGroup { get; set; }

    public void AddSpecialProperties()
    {
        return;
    }
}
