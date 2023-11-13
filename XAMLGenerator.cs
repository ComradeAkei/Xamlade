using System;
using System.Reflection;
using Avalonia.Controls;

namespace Xamlade;

public static class Broadcast
{
    public delegate void EventHandler(int mode);

    public static event EventHandler OnBroadcast;

    public static void InitXAML()=> OnBroadcast?.Invoke(0);
    public static void XAMLize()=> OnBroadcast?.Invoke(1);
    
   //Восстановить поведение jElement после выгрузки из XAML
    public static void RestoreBehavior()=> OnBroadcast?.Invoke(2);
    //Убить все объекты
    public static void KillAll()=> OnBroadcast?.Invoke(3);
    //Восстановить дерево объектов
    public static void RestoreTree()=> OnBroadcast?.Invoke(4);



    public static void DisposeElement(JControl element) => element.Dispose();
}

public static class XAMLGenerator
{
    public static string GetProperties(JControl element)
    {
        string getProperties = "";
        Type type = element.GetType();
        var props = type.GetProperties();
        ConstructorInfo? constructor = type.GetConstructor(new Type[] { });
        var DefaultObject = constructor.Invoke(new object[] { });
        
        foreach (var prop in props)
        {
            if((element.Name == "MainCanvas") && prop.Name is "Width" or "Height") continue;
            if (!MainWindow.ExcludedWords.Contains(prop.Name))
            {
                if (prop.Name == "Source")
                    getProperties += ($"{prop.Name}=\"{((jImage)element).jImageSource}\" ");
                else if (prop.GetValue(element)?.ToString() != prop.GetValue(DefaultObject)?.ToString()
                    && prop.GetValue(element) != null)
                    getProperties += ($"{prop.Name}=\"{prop.GetValue(element)}\" ");
            }
        }

        if (element.Name == "MainCanvas")
        {
            getProperties += " HorizontalAlignment=\"Stretch\"\n            VerticalAlignment=\"Stretch\"";
            return getProperties;
        }
        if ((element.jParent as JControl).Type == jElementType.Canvas.ToString())
        {
            getProperties+=$"Canvas.Left=\"{Convert.ToInt32(Canvas.GetLeft((Control)element))}\" ";
            getProperties+=$"Canvas.Top=\"{Convert.ToInt32(Canvas.GetTop((Control)element))}\" ";
        }

        return getProperties;

    }
    public static void XAMLRatingInit(JControl element)
    {
        if(element.Name==null) return;
        element.XAMLPiece.Clear();
        element.XAMLRating = element is IChildContainer container ? container.jChildren.Count : 0;
        element.XAMLPiece.Add($"<{element.Type} {GetProperties(element)}>");
    }
    public static void XAMLize(JControl element)
    {
        if(element.Name==null) return;
        if (element.XAMLRating == 0)
        {
            element.XAMLPiece.Add($"</{element.Type}>");
            element.XAMLRating--;
            if (element.jParent is not JControl parent) return;
            parent.XAMLPiece.AddRange(element.XAMLPiece);
            parent.XAMLRating--;
        }
    }

}