using System;
using System.Reflection;
using Avalonia;

namespace Xamlade;




public static class Reflector
{
    public static void SetName(string? name, JControl element)
    {
        FieldInfo privateField =
            typeof(StyledElement).GetField("_name", BindingFlags.NonPublic | BindingFlags.Instance);
        privateField.SetValue(element, name);
    }
}