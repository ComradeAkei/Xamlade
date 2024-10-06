using System.Collections.Generic;

namespace Xamlade.LinkWorkers;


//ТОЛЬКО ДЛЯ ОТЛАДКИ НЕПРЕДСКАЗУЕМОГО ПОВЕДЕНИЯ DEXAMLIZE
public static class BeholderTimeAnalisys
{
    public static List<Beholder> Beholders { get; set; }

    static BeholderTimeAnalisys()
    {
        Beholders = new List<Beholder>();
    }

    public static void DebugBeholder()
    {
        var beh = Beholders;
    }
}