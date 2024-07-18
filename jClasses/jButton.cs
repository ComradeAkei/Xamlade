//МОИ ИНТЕРФЕЙСЫ ИМЕНУЮТСЯ С J


//Повторное использование кода в классах

//Рамку вынести в jSelectable

//Добавить приоритеты отрисовки (задний - передний план)


using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Avalonia.Controls;
using Xamlade.XAMLWorkers;

namespace Xamlade.jClasses;

// Для контейнеров

//Интерфейс рамки выделения объекта

//Для простых объектов

//Приёмник широковещательных сообщений jObject

public class jButton : Button, JControl, JBroadcastHandler<JControl>, JSelectable
{
    public int ID = 0;
    protected override Type StyleKeyOverride => typeof(Button);
    public mBorder selectionBorder { get; set; }

    [field: NonSerialized]
    public JChildContainer? jParent { get; set; }
    private string controlType => jElementType.Button.ToString();
    public string Type => controlType;
    [JsonIgnore]
    public mTreeViewItem? mTreeItem { get; set; }
    public int XAMLRating { get; set; }
    [field: NonSerialized]
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
        Broadcast.OnBroadcast += (this as JBroadcastHandler<JControl>).HandleBroadcast;
        XAMLPiece = new List<string>();
        mTreeItem = new mTreeViewItem(this);
    }
    
    
    
}

//Доделать