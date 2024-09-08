using System.Linq.Expressions;

namespace Xamlade.mClasses;



//MControl описывает вспомогательные UI-объекты, необходимые для работы GUI Xamlade
public interface MControl
{
    //Строка служебной информации
    public string Label { get; set; }
}