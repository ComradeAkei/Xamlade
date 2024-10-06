using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.Markup.Xaml;
using Xamlade.Extensions;
using Xamlade.FunctionalAreas;
using Xamlade.jClasses;
using Xamlade.LinkWorkers;
using Xamlade.mClasses;

namespace Xamlade.XAMLWorkers;

public static class ImportXAML
{
    private static string filePathXAML = "";
    private static string ExternalXAML = "";
    private static readonly string[] elementsToReplace;

    static ImportXAML()
    {
        elementsToReplace = new[]
        {
            "Border", "Canvas", "DockPanel", "Grid", "Panel", "ScrollViewer", "StackPanel",
            "TabControl", "TabItem", "Button", "CheckBox", "ComboBox", "DatePicker",
            "ListBox", "ListView", "Menu", "MenuItem", "ProgressBar", "RadioButton",
            "Slider", "TextBox", "ToggleButton", "TextBlock", "Image", "ComboBoxItem"
        };
    }
//todo убрать window
    public static async Task RunDeXAMLIZEAsync(Window window)
    {
        await OpenXAML(window);
    }

    private static void LoadXAML()
    {
        if (filePathXAML == "") return;

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
            Beholder.NewBeholder(item);
            Workspace.MainCanvas.AddChild(item);
        }

        for (int i = 0; i < buf.Count; i++)
        {
            Canvas.SetTop(Workspace.MainCanvas.jChildren[i] as Control, canv_top[i]);
            Canvas.SetLeft(Workspace.MainCanvas.jChildren[i] as Control, canv_left[i]);
        }

        Workspace.MainCanvas.Beholder.mTreeItem.Items.Clear();
        Broadcast.RestoreBehavior();
        Broadcast.RestoreTree();
    }

    public static void CorrectLoadedjElement(JControl element)
    {
        if (element.Name == "MainCanvas") return;
        if (element.Beholder is null)
            Beholder.NewBeholder(element);

        var parent = ((Control)element).Parent;
        element.SetParent((JChildContainer)parent);
    }

    private static async Task OpenXAML(Window window)
    {
        filePathXAML = "";
        ExternalXAML = "";
    
        OpenFileDialog dialog = new OpenFileDialog
        {
            Title = "Выберите XAML",
            AllowMultiple = false
        };

        dialog.Filters.Add(new FileDialogFilter
        {
            Name = "Файл разметки XAML",
            Extensions = { "xaml", "axaml" }
        });

        string[] result = await dialog.ShowAsync(window);
        if (result != null && result.Length > 0)
        {
            filePathXAML = result[0];
            ExternalXAML = File.ReadAllText(filePathXAML);

            // Удаление детей из MainCanvas
            for (int i = Workspace.MainCanvas.jChildren.Count - 1; i >= 0; i--)
            {
                Workspace.MainCanvas.RemoveChild(Workspace.MainCanvas.jChildren[i]);
            }

            // Радикальная операция
            Broadcast.KillAll();
        }
    }

    public static void CorrectXAML()
    {
        if (filePathXAML == "") return;

        ExternalXAML = ReplaceElements(ExternalXAML, elementsToReplace);
        ExternalXAML = RemoveWindowTags(ExternalXAML);
        ExternalXAML = ReplaceMainCanvasTag(ExternalXAML);
        File.WriteAllText(filePathXAML + "1", ExternalXAML);
    }

    static string ReplaceElements(string input, string[] elements)
    {
        string openTagPattern = @"<({0})\b([^>]*)>";
        string closeTagPattern = @"<\/({0})>";
        int index = 0;

        foreach (var element in elements)
        {
            string formattedOpenTagPattern = string.Format(openTagPattern, element);
            string formattedCloseTagPattern = string.Format(closeTagPattern, element);

            input = Regex.Replace(input, formattedOpenTagPattern, match =>
            {
                var matchValue = match.Value;
                var replacement = matchValue.Replace("<" + element, "<jClasses:j" + element);
                index++;
                return replacement;
            });

            input = Regex.Replace(input, formattedCloseTagPattern, match =>
            {
                var matchValue = match.Value;
                var replacement = matchValue.Replace("</" + element, "</jClasses:j" + element);
                index++;
                return replacement;
            });
        }

        return input;
    }

    static string RemoveWindowTags(string input)
    {
        string pattern = @"<Window\b[^>]*>(.*?)</Window>";
        return Regex.Replace(input, pattern, m => m.Groups[1].Value, RegexOptions.Singleline);
    }

    static string ReplaceMainCanvasTag(string input)
    {
        var match = Regex.Match(input, @"<jClasses:jCanvas", RegexOptions.Singleline);
        if (match.Success)
        {
            int index = match.Index;
            int length = match.Length;
            input = input.Substring(0, index) +
                    "<Canvas xmlns='https://github.com/avaloniaui' xmlns:jClasses='clr-namespace:Xamlade.jClasses'" +
                    input.Substring(index + length);

            Match lastMatch = Regex.Match(input, @"</jClasses:jCanvas>", RegexOptions.Singleline | RegexOptions.RightToLeft);
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
        if (element.Beholder is null)
            element.Beholder = new(element);

        if (element is JChildContainer container)
        {
            foreach (var child in (element as Panel).Children)
            {
                if (child is MControl) continue;
                if (!container.jChildren.Contains(child as JControl))
                    container.jChildren.Add(child as JControl);
            }

            foreach (var child in container.jChildren)
            {
                if (child is null) continue;
                JBaseStatic.Jctor(child);
                if (child.Beholder is null)
                    child.Beholder = new(child);
                if (child.jParent is null)
                    child.jParent = container;

                Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
                {
                    if (child.Beholder != null && child.Beholder.mTreeItem != null && !element.Equals(child))
                    {
                       
                        
                        if (child.Beholder.mTreeItem.Parent is StackPanel ch)
                            (ch).Children.Remove(child as Control);
                        else if (child.Beholder.mTreeItem.Parent is not null && (child.Beholder.mTreeItem.Parent as mTreeViewItem) != null)
                        {
                            (child.Beholder.mTreeItem.Parent as mTreeViewItem).Items.Remove(child);
                        }
                        if ( (!element.Beholder.mTreeItem.Items.Contains(child.Beholder.mTreeItem)) )
                            element.Beholder.mTreeItem.Items.Add(child.Beholder.mTreeItem);
                    }
                });
            }
        }

        HierarchyControl.Selected = Workspace.MainCanvas.Beholder.mTreeItem;
        Workspace.MainCanvas.Beholder.mTreeItem.IsExpanded = true;
        return;
    }

    public static void DEXAMLIZE(object? sender, RoutedEventArgs e)
        => DEXAMLIZEAsync();
    public static async Task DEXAMLIZEAsync()
    {
        // Ждем завершения Task.Run и всех вложенных задач
        await Task.Run(async () =>
        {
            // Переходим на UI-поток для вызова окна диалога
            await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(async () =>
            {
                await RunDeXAMLIZEAsync(ProgramWindow.MainWindow._MainWindow);
            });
        });

        // Выполняем следующие синхронные команды после завершения Task.Run
        CorrectXAML();
        LoadXAML();
        Console.WriteLine("Задача завершена");
    }
}
