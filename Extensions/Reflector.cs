using System;
using System.Reflection;
using Avalonia;
using Microsoft.Diagnostics.Runtime;
using Xamlade.jClasses;

namespace Xamlade.Extensions;




public static class Reflector
{
    public static void SetName(string? name, JControl element)
    {
        var privateField =
            typeof(StyledElement).GetField("_name", BindingFlags.NonPublic | BindingFlags.Instance);
        privateField?.SetValue(element, name);
    }
    
    
    public static void PrintFieldsForType(ClrRuntime runtime, string targetType)
    {
        int i = 0;
        ClrHeap heap = runtime.Heap;
        foreach (var ptr in heap.EnumerateObjects())
        {
            ClrType type = heap.GetObjectType(ptr);
            if (type.Name == targetType)
            {
                i++;
            }
        }
        Console.WriteLine(targetType+": "+i);
    }

    
      
    
    
}