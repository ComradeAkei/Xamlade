using System.Collections.Generic;

namespace Xamlade.jClasses;

public interface JChildContainer
{
    List<JControl> jChildren { get; }
    void AddChild(JControl child, double top = 0, double left = 0);

    public void RemoveChild(JControl child);

}