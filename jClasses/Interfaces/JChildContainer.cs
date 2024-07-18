using System.Collections.Generic;

namespace Xamlade.jClasses;

public interface JChildContainer
{
    List<JControl> jChildren { get; }
    void AddChild(JControl child);

    public void RemoveChild(JControl child);

}