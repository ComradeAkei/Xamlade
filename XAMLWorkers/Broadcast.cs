using Xamlade.jClasses;

namespace Xamlade.XAMLWorkers;

public static class Broadcast
{
    public delegate void EventHandler(int mode);

    public static event EventHandler? OnBroadcast;

    public static void InitXAML()=> OnBroadcast?.Invoke(0);
    public static void XAMLize()=> OnBroadcast?.Invoke(1);
    
    //Восстановить поведение jElement после выгрузки из XAML
    public static void RestoreBehavior()=> OnBroadcast?.Invoke(2);
    //Убить все объекты
    public static void KillAll()=> OnBroadcast?.Invoke(3);
    //Восстановить дерево объектов
    public static void RestoreTree()=> OnBroadcast?.Invoke(4);
    //Снять выделение
    //  public static void RemoveSelection()=> OnBroadcast?.Invoke(5);



    public static void DisposeElement(JControl element) => element.Dispose();
}