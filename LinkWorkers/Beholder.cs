using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using Avalonia;
using Avalonia.Controls;
using Xamlade.Extensions;
using Xamlade.FunctionalAreas;
using Xamlade.jClasses;

namespace Xamlade.LinkWorkers;

/// <summary>
/// Наблюдатель хранит ссылки на служебные объекты, необходимые для управления  jObject
/// </summary>
public class Beholder
{
    public JControl? element { get; set; }
    public mTreeViewItem? mTreeItem { get; set; }
    public ItemCollection? PropListItems { get; set; }

    private ulong UID { get; }

    private bool isEXIST = true;

    public Beholder(JControl obj)
    {
        element = obj;
        obj.Beholder = this;
        //Заменить на глобальный генератор
        UID = (ulong)Utils.NextgenIterator;
        mTreeItem = new mTreeViewItem(obj);
        element.JControlInit();
        //PropListItemsInit();
        
    }

    //Вызывать после присоединения объекта на контейнер
    public void PropListItemsInit()
    {
        // Использование рефлексии для создания экземпляра ItemCollection
        var constructor = typeof(ItemCollection).GetConstructor(
            BindingFlags.Instance | BindingFlags.NonPublic,
            null,
            Type.EmptyTypes,
            null);

        PropListItems = (ItemCollection)constructor.Invoke(new object[] { });


        //Добавляем все свойства со словаря свойств
        foreach (var KVP in element.xPropertiesGroup)
        {
            var title = PropertiesControl.CreatePropItem(KVP.Key, new Property());
            PropListItems.Add(title);
            var enumerator = KVP.Value.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var kv = enumerator.Current;
                var item = PropertiesControl.CreatePropItem(kv.Key, kv.Value);
                PropListItems.Add(item);
            }
        }
        
        
        
    }
    public static void OnPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (sender is null) return;
        (sender as JControl).Beholder.HandlePropertyChange(e);
        //   Console.WriteLine($"{(sender as JControl).Name} " +
                                 // $"Свойство {e.Property.Name} изменилось с {e.OldValue} на {e.NewValue}");
    }

    private void HandlePropertyChange(AvaloniaPropertyChangedEventArgs e)
    {
        element.UpdateProperty(e.Property.Name, e.NewValue);
        UpdatePropList(e.Property.Name);
    }

    //Всё оно чужое - у тебя лишь Имя Твоё!
    public static void HandleNameUpdate(JControl element)
    {
        element.UpdateProperty("Name", element.Name);
        element.Beholder.UpdatePropList("Name");
    }
    public void UpdatePropList(string propname)
    {
        if(PropListItems is null) return;
        // Найти индекс элемента с нужным именем.
        int index = -1;
        for (int i = 0; i < PropListItems.Count; i++)
        {
            if (PropListItems[i] is ListBoxItem item && item.Name == propname)
            {
                index = i;
                break;
            }
        }

        // Если элемент найден, заменить его.
        if (index != -1)
        {
            var tmp = PropertiesControl.CreatePropItem(propname, element.GetProperty(propname));//element.xPropertiesGroup["main"][propname]);
            PropListItems[index] = tmp;
        }
    }
    
    }

