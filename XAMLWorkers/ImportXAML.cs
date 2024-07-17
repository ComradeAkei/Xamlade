using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Xamlade.Extensions;
using Xamlade.FunctionalAreas;
using Xamlade.jClasses;

namespace Xamlade.XAMLWorkers;

public static class ImportXAML
{
    private static string filePathXAML = "";
    private static string ExternalXAML = "";

    private static readonly string[] elementsToReplace;

    static ImportXAML()
    {
        elementsToReplace = new[] {
            "Border", "Canvas", "DockPanel", "Grid", "Panel", "ScrollViewer", "StackPanel",
            "TabControl", "TabItem", "Button", "CheckBox", "ComboBox", "DatePicker",
            "ListBox", "ListView", "Menu", "MenuItem", "ProgressBar", "RadioButton",
            "Slider", "TextBox", "ToggleButton", "TextBlock", "Image"
        };
    }

    public static async Task RunDeXAMLIZE(Window window)
    {
        
        await OpenXAML(window);
        await CorrectXAML();
        await LoadXAML();
    }

    private static async Task LoadXAML()
    {
       if(filePathXAML == "") return;


       var obj = AvaloniaRuntimeXamlLoader.Load(ExternalXAML, typeof(ProgramWindow.MainWindow).Assembly) as Canvas;


        var buf = new List<JControl>();
        var canv_left = new List<double>();
        var canv_top = new List<double>();
        for (int j = obj.Children.Count - 1; j >= 0; j--)
        {
            buf.Add(obj.Children[j] as JControl);
            canv_top.Add(Canvas.GetTop(obj.Children[j]));
            canv_left.Add(Canvas.GetLeft(obj.Children[j]));
            obj.Children.RemoveAt(j);
        }

        foreach (var item in buf)
        {
           Workspace.MainCanvas.AddChild(item);
        }

        for (int i = 0; i < buf.Count; i++)
        {
            Canvas.SetTop(Workspace.MainCanvas.jChildren[i] as Control, canv_top[i]);
            Canvas.SetLeft(Workspace.MainCanvas.jChildren[i] as Control, canv_left[i]);
        }
        Workspace.MainCanvas.mTreeItem.Items.Clear();

        Broadcast.RestoreBehavior();
        Broadcast.RestoreTree();
    }

    public static void CorrectLoadedjElement(JControl element)
    {
        element.Name ??= element.Type + "_" + (Utils.NextgenIterator++);


        if (element.Name == "MainCanvas") return;
            element.mTreeItem = new mTreeViewItem(element);
            element.mTreeItem.Header =element.Name;
            var parent = ((Control)element).Parent;
            
            element.SetParent((JChildContainer)parent);

            

            //  element.SetParent(element);
            element.PointerEntered += Workspace.OnjControlPointerEntered;
            element.PointerExited += Workspace.OnjControlPointerExited;
            element.Click += Workspace.jElementClick;
            element.PointerPressed += Workspace.OnjControlPressed;
            element.PointerReleased += Workspace.OnjControlReleased;
            ElementGenerator.InitSelectionBorder(element as JSelectable);
        
        

    }

    private static async Task OpenXAML(Window window)
    {
        filePathXAML = "";
        ExternalXAML = "";


        OpenFileDialog dialog = new OpenFileDialog();
        dialog.Title = "Выберите XAML";
        dialog.AllowMultiple = false;
        dialog.Filters.Add(new FileDialogFilter
        {
            Name = "Файл разметки XAML",
            Extensions = { "xaml", "axaml" }
        });
        Task<string[]> task = dialog.ShowAsync(window);

        // Дожидаемся завершения задачи (await)
        string[] result = await task;

        // Обрабатываем результат
        if (result != null && result.Length > 0)
        {
            filePathXAML = result[0];
            ExternalXAML = File.ReadAllText(filePathXAML);
            // Удаляем элементы из MainCanvas
            for (int i = Workspace.MainCanvas.jChildren.Count - 1; i >= 0; i--)
            {
                Workspace.MainCanvas.RemoveChild(Workspace.MainCanvas.jChildren[i]);
            }

            //Радикальная операция
            //УЧЁТНЫЙ НОМЕР: 1_KILLALL
            Broadcast.KillAll();
        }
        
    }

    public static async Task CorrectXAML()
    {
        if (filePathXAML == "") return;
        ExternalXAML = ReplaceElements(ExternalXAML, elementsToReplace);
        ExternalXAML = RemoveWindowTags(ExternalXAML);
        ExternalXAML = ReplaceMainCanvasTag(ExternalXAML);
        File.WriteAllText(filePathXAML + "1", ExternalXAML);
    }

    static string ReplaceElements(string input, string[] elements)
    {
        // <Имя_элемента>
        string openTagPattern = @"<({0})\b([^>]*)>";
        //</Имя_элемента>
        string closeTagPattern = @"<\/({0})>";

        int index = 0;

        foreach (var element in elements)
        {
            // Строим паттерны с учетом текущего элемента
            string formattedOpenTagPattern = string.Format(openTagPattern, element);
            string formattedCloseTagPattern = string.Format(closeTagPattern, element);

            // Заменяем открывающие теги
            input = Regex.Replace(input, formattedOpenTagPattern, match =>
            {
                var matchValue = match.Value;

                // Заменяем <Имя_элемента на <xamlade:jИмя элемента
                var replacement = matchValue.Replace("<" + element, "<jClasses:j" + element);

                index++;

                return replacement;
            });

            // Заменяем закрывающие теги
            input = Regex.Replace(input, formattedCloseTagPattern, match =>
            {
                var matchValue = match.Value;
                // Заменяем </Имя_элемента на </xamlade:jИмя элемента
                var replacement = matchValue.Replace("</" + element, "</jClasses:j" + element);

                index++;

                return replacement;
            });
        }

        return input;
    }

    static string RemoveWindowTags(string input)
    {
        // Паттерн для поиска открывающего и закрывающего тегов <Window ...> и </Window>
        string pattern = @"<Window\b[^>]*>(.*?)</Window>";

        // Заменяем совпадение на содержимое между тегами
        return Regex.Replace(input, pattern, m => m.Groups[1].Value, RegexOptions.Singleline);
    }

    static string ReplaceMainCanvasTag(string input)
    {
        var match = Regex.Match(input, @"<jClasses:jCanvas", RegexOptions.Singleline);

        // Если найдено, заменяем его
        if (match.Success)
        {
            // Заменяем только первое вхождение
            int index = match.Index;
            int length = match.Length;
            input = input.Substring(0, index) +
                    "<Canvas xmlns='https://github.com/avaloniaui' xmlns:jClasses='clr-namespace:Xamlade.jClasses'" +
                    input.Substring(index + length);

            // Находим последнее вхождение </xamlade:jCanvas>
            Match lastMatch = Regex.Match(input, @"</jClasses:jCanvas>",
                RegexOptions.Singleline | RegexOptions.RightToLeft);

            // Если найдено, заменяем его
            if (lastMatch.Success)
            {
                index = lastMatch.Index;
                length = lastMatch.Length;
                input = input.Substring(0, index) + "</Canvas>" + input.Substring(index + length);
            }
        }

        return input;
    }

    public static void CorrectTree(JControl element)
    {
        if (element is JChildContainer container)
        {
            foreach (var child in container.jChildren)
                if (!element.mTreeItem.Items.Contains(child.mTreeItem))
                    element.mTreeItem.Items.Add(child.mTreeItem);
        }

        HierarchyControl.Selected = Workspace.MainCanvas.mTreeItem;
        Workspace.MainCanvas.mTreeItem.IsExpanded = true;
    }
    public static async void DEXAMLIZE(object? sender, RoutedEventArgs e)
    {
        await RunDeXAMLIZE(ProgramWindow.MainWindow._MainWindow);
    }
}