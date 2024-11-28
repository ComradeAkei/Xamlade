using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Xamlade.jClasses;
using Newtonsoft.Json.Linq;
using Xamlade.FunctionalAreas;

namespace Xamlade.Extensions;

public static class JCopy
{
    public static JControl Copy(JControl original)
    {
        var parent = original.jParent;
        var elementType = Type.GetType("Xamlade.jClasses.j" + original.Type);
        var element = ElementGenerator.GenerateElement(elementType, parent);
        element.xPropertiesGroup = new Dictionary<string, Dictionary<string, Property>>(original.xPropertiesGroup);
        Reflector.SetName($"{original.Name}_copy",element);
        element.Beholder.mTreeItem = new mTreeViewItem(element);
        foreach (var KVP in element.xPropertiesGroup["main"])
        {
            if (KVP.Key != "Name")
                PropertiesControl.SetPropertyValue(KVP.Key, KVP.Value.Value, element, null);
        }

        parent.AddChild(element);
        return element;
    }
}