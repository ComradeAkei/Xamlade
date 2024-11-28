using System;
using System.Reflection;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.LogicalTree;
using Avalonia.Media;
using AvaloniaColorPicker;
using Xamlade.FunctionalAreas;
using Xamlade.ProgramWindow;

namespace Xamlade.ColorPicker;

public class CustomColorButton<T> : ColorButton<T> where T : Window, IColorPickerWindow
{
    Type baseType = typeof(ColorButton<T>);

    public CustomColorButton() : base()
    {
        /*
         * Суть в том, чтобы отписать от события смены цвета богомерзкий обработчик с палеткой,
         * из-за которого Xamlade падает по непонятным внутренним NullRefferenceException.
         */


        PropertyInfo contentButtonProperty =
            baseType.GetProperty("ContentButton", BindingFlags.NonPublic | BindingFlags.Instance);

        var contentButton = contentButtonProperty?.GetValue(this);

        if (contentButton != null)
        {
            //событие PropertyChanged 
            EventInfo propertyChangedEvent = contentButton.GetType().GetEvent("PropertyChanged");


            if (propertyChangedEvent != null)
            {
                FieldInfo eventField = null;
                Type currentType = contentButton.GetType();

                //Target type = AvaloniaColorPicker.ColorButton[Xamlade.ColorPicker.MyColorPickerWindow]
                //Глубина рефлексии ебануца со стула
                //Ищем поле у базовых классов
                while (currentType != null)
                {
                    eventField = currentType.GetField("_propertyChanged",
                        BindingFlags.NonPublic | BindingFlags.Instance);
                    //Закатный отряд
                    if (eventField != null)
                        break;
                    currentType = currentType.BaseType;
                }

                if (eventField != null)
                {
                    // Получаем делегат
                    Delegate currentDelegate = (Delegate)eventField.GetValue(contentButton);
                    // Привет атписчекам!!!
                    if (currentDelegate != null)
                        foreach (Delegate del in currentDelegate.GetInvocationList())
                            propertyChangedEvent.RemoveEventHandler(contentButton, del);
                }

                // Подписываем православный обработчик
                MethodInfo eventHandler = typeof(CustomColorButton<T>).GetMethod("CustomContentButton_PropertyChanged",
                    BindingFlags.NonPublic | BindingFlags.Instance);
                Delegate handler = Delegate.CreateDelegate(propertyChangedEvent.EventHandlerType, this, eventHandler);
                propertyChangedEvent.AddEventHandler(contentButton, handler);
            }
        }
    }

    private async void CustomContentButton_PropertyChanged(object sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property == ToggleButton.IsCheckedProperty)
            if ((e.NewValue as bool?).Value)
                // Нахуй палетку!!
                ShowColorPickerDialog();
    }

    private async Task ShowColorPickerDialog()
    {
        // Ищем приватный метод CreateColourPickerWindow
        MethodInfo createColourPickerWindowMethod =
            baseType.GetMethod("CreateColourPickerWindow", BindingFlags.NonPublic | BindingFlags.Static);
        object[] parameters = { new Color?(this.Color) };
        T colourPickerWindow = (T)createColourPickerWindowMethod?.Invoke(this, parameters);

        await colourPickerWindow.ShowDialog<object>(MainWindow._MainWindow);
        this.Color = colourPickerWindow.Color;
    }
}

/// <summary>
/// A control that can be used to select a <see cref="T:Avalonia.Media.Color" />.
/// </summary>
public class MyColorButton : CustomColorButton<MyColorPickerWindow>
{
}