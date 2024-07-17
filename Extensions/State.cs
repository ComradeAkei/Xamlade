namespace Xamlade.Extensions;

public static class State
{
    public static bool RectangleFlag = true;
    public static bool LCtrlPressed = false;
    public static bool LShiftPressed = false;
    public static bool ResizeFlag = false;

    public static bool StrictModeEnabled = false;
    public static int StrictModeValue { get; set; }
    public static bool NewResizeFlag = true;
}