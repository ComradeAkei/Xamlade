using System;
using System.Collections.Generic;

namespace Xamlade.jClasses;

public interface JChildContainer
{
    public delegate Property ContainerPropertyDelegate(JControl element);
    public delegate void ContainerSetPropertyDelegate(JControl element, Property value);
    public List<(string, ContainerPropertyDelegate)> ContainerProperties { get; set; }
    
    //Методы установки свойства на контейнере
    public static abstract Dictionary<string, ContainerSetPropertyDelegate> ContainerSetProperties { get; set; }
    
    public List<JControl> jChildren { get; }
    public void AddChild(JControl child);

    public void RemoveChild(JControl child);

    public void InitContainerProperties();
}