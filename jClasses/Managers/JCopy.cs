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
    public static JControl Copy(JControl original, JChildContainer newParent)
    {
        var parent = original.jParent;
        var elementType = Type.GetType("Xamlade.jClasses.j" + original.Type);
        var element = ElementGenerator.GenerateElement(elementType, parent);
        element.xPropertiesGroup = new Dictionary<string, Dictionary<string, Property>>(original.xPropertiesGroup);
        element.SpecialSetDelegates = new Dictionary<string, JChildContainer.ContainerSetPropertyDelegate>(original.SpecialSetDelegates);
        
        Reflector.SetName($"{original.Name}_copy",element);
        element.Beholder.mTreeItem = new mTreeViewItem(element);
        (newParent as JControl).Beholder.mTreeItem.Items.Add(element.Beholder.mTreeItem);
        Workspace.movable = element;
        foreach (var KVP in element.xPropertiesGroup["main"])
        {
            if (KVP.Key != "Name")
                PropertiesControl.SetPropertyValue(KVP.Key, KVP.Value.Value, element, null);
        }
        foreach (var KVP in element.xPropertiesGroup["container"])
                PropertiesControl.SetPropertyValue(KVP.Key, KVP.Value.Value, element, null);
        newParent.AddChild(element);
        
        if (original is not JChildContainer container) return element;
        foreach (var cp_child in container.jChildren)
            Copy(cp_child, (JChildContainer)element);
        
        return element;
    }
}