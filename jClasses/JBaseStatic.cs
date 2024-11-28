using System.Collections.Generic;
using Avalonia.Controls;
using Xamlade.FunctionalAreas;
using Xamlade.XAMLWorkers;

namespace Xamlade.jClasses;

public static class JBaseStatic
{
    /// <summary>
    /// Универсальный статический конструктор JControl
    /// </summary>
    /// <param name="element">JObject</param>
    public static void Jctor(JControl element)
    {
        Broadcast.OnBroadcast += (element as JBroadcastHandler<JControl>).HandleBroadcast;
        element.XAMLPiece = new List<string>();
        if (element is JChildContainer container)
            if (container.jChildren is null)
                container.jChildren = new List<JControl>();
    }

    public static void CorrectProperties(JControl element)
    {
        element.UpdateProperty("Width", (element as Control).Bounds.Width);
        element.UpdateProperty("Height", (element as Control).Bounds.Height);
        element.Beholder.UpdatePropList("Width");
        element.Beholder.UpdatePropList("Height");
    }
}