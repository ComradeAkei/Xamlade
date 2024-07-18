using Avalonia.Controls;

namespace Xamlade.jClasses;

public class mBorder : Border, MControl
{
    private JControl master;
    public mBorder(JControl binded)
    {
        master = binded;
        Name = master.Name + "_Border";
    }
}