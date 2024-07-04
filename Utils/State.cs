namespace Xamlade;

public static class State
{
    public static bool LCtrlPressed = false;
    public static bool ResizeFlag = false;

    public static bool StrictModeEnabled = false;
    public static int StrictModeValue { get; set; }
}