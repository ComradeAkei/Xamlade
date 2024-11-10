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
using Xamlade.ProgramWindow;

namespace Xamlade.XAMLWorkers
{
    public static class ImportXAML
    {
        private static string filePathXAML = "";
        private static string externalXAML = "";

        private static readonly string[] elementsToReplace =
        {
            "Border", "Canvas", "DockPanel", "Grid", "Panel", "ScrollViewer", "StackPanel",
            "TabControl", "TabItem", "Button", "CheckBox", "ComboBox", "DatePicker",
            "ListBox", "ListView", "Menu", "MenuItem", "ProgressBar", "RadioButton",
            "Slider", "TextBox", "ToggleButton", "TextBlock", "Image", "ComboBoxItem"
        };

        public static async Task RunDeXAMLIZEAsync()
        {
            await OpenXAML();
        }

        private static void LoadXAML()
        {
            if (string.IsNullOrEmpty(filePathXAML))
            {
                return;
            }

            var obj = AvaloniaRuntimeXamlLoader.Load(externalXAML, typeof(ProgramWindow.MainWindow).Assembly) as Canvas;
            if (obj == null) return;

            var buf = new List<JControl>();
            var canvLeft = new List<double>();
            var canvTop = new List<double>();

            for (int j = obj.Children.Count - 1; j >= 0; j--)
            {
                if (obj.Children[j] is JControl control)
                {
                    buf.Add(control);
                    canvTop.Add(Canvas.GetTop(control as Control));
                    canvLeft.Add(Canvas.GetLeft(control as Control));
                }
                obj.Children.RemoveAt(j);
            }

            foreach (var item in buf)
            {
                Beholder.NewBeholder(item);
                Workspace.MainCanvas.AddChild(item);
            }

            for (int i = 0; i < buf.Count; i++)
            {
                Canvas.SetTop(Workspace.MainCanvas.jChildren[i] as Control, canvTop[i]);
                Canvas.SetLeft(Workspace.MainCanvas.jChildren[i] as Control, canvLeft[i]);
            }

            Workspace.MainCanvas.Beholder.mTreeItem.Items.Clear();
            Broadcast.RestoreBehavior();
            Broadcast.RestoreTree();
        }

        public static void CorrectLoadedjElement(JControl element)
        {
            if (element.Name == "MainCanvas")
            {
                return;
            }

            element.Beholder ??= Beholder.NewBeholder(element);
            element.SetParent((JChildContainer)((Control)element).Parent);
        }

        private static async Task OpenXAML()
        {
            filePathXAML = "";
            externalXAML = "";

            var dialog = new OpenFileDialog
            {
                Title = "Выберите XAML",
                AllowMultiple = false,
                Filters = { new FileDialogFilter { Name = "Файл разметки XAML", Extensions = { "xaml", "axaml" } } }
            };

            string[] result = await dialog.ShowAsync(MainWindow._MainWindow);
            if (result != null && result.Length > 0)
            {
                filePathXAML = result[0];
                externalXAML = File.ReadAllText(filePathXAML);

                Workspace.MainCanvas.jChildren.Clear();
                Broadcast.KillAll();
            }
        }

        public static void CorrectXAML()
        {
            if (string.IsNullOrEmpty(filePathXAML))
            {
                return;
            }

            externalXAML = ReplaceElements(externalXAML, elementsToReplace);
            externalXAML = RemoveWindowTags(externalXAML);
            externalXAML = ReplaceMainCanvasTag(externalXAML);

            File.WriteAllText(filePathXAML + "1", externalXAML);
        }

        private static string ReplaceElements(string input, string[] elements)
        {
            foreach (var element in elements)
            {
                input = Regex.Replace(input, $@"<({element})\b([^>]*)>",
                    match => match.Value.Replace($"<{element}", $"<jClasses:j{element}"));
                input = Regex.Replace(input, $@"<\/({element})>",
                    match => match.Value.Replace($"</{element}", $"</jClasses:j{element}"));
            }

            return input;
        }

        private static string RemoveWindowTags(string input)
        {
            return Regex.Replace(input, @"<Window\b[^>]*>(.*?)</Window>",
                m => m.Groups[1].Value, RegexOptions.Singleline);
        }

        private static string ReplaceMainCanvasTag(string input)
        {
            var match = Regex.Match(input, @"<jClasses:jCanvas", RegexOptions.Singleline);
            if (match.Success)
            {
                int index = match.Index;
                input = input.Substring(0, index) +
                        "<Canvas xmlns='https://github.com/avaloniaui' xmlns:jClasses='clr-namespace:Xamlade.jClasses'" +
                        input.Substring(index + match.Length);

                var lastMatch = Regex.Match(input, @"</jClasses:jCanvas>",
                    RegexOptions.Singleline | RegexOptions.RightToLeft);
                if (lastMatch.Success)
                {
                    index = lastMatch.Index;
                    input = input.Substring(0, index) + "</Canvas>" + input.Substring(index + lastMatch.Length);
                }
            }

            return input;
        }

        public static void CorrectTree(JControl element)
        {
            element.Beholder ??= new Beholder(element);

            if (element is JChildContainer container)
            {
                foreach (var child in ((Panel)element).Children)
                {
                    if (child is MControl) continue;
                    if (!container.jChildren.Contains(child as JControl))
                        container.jChildren.Add(child as JControl);
                }

                foreach (var child in container.jChildren)
                {
                    if (child is null) continue;
                    JBaseStatic.Jctor(child);
                    child.Beholder ??= new Beholder(child);
                    child.jParent ??= container;

                    Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
                    {
                        if (child.Beholder?.mTreeItem != null && !element.Equals(child))
                        {
                            if (child.Beholder.mTreeItem.Parent is StackPanel ch)
                                ch.Children.Remove(child as Control);
                            else if (child.Beholder.mTreeItem.Parent is mTreeViewItem parentItem)
                                parentItem.Items.Remove(child);
                            if (!element.Beholder.mTreeItem.Items.Contains(child.Beholder.mTreeItem))
                                element.Beholder.mTreeItem.Items.Add(child.Beholder.mTreeItem);
                        }
                    });
                }
            }

            HierarchyControl.Selected = Workspace.MainCanvas.Beholder.mTreeItem;
            Workspace.MainCanvas.Beholder.mTreeItem.IsExpanded = true;
        }

        public static void DEXAMLIZE(object? sender, RoutedEventArgs e)
        {
            DEXAMLIZEAsync();
        }

        public static async Task DEXAMLIZEAsync()
        {
            await Task.Run(async () =>
            {
                await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(async () =>
                {
                    await RunDeXAMLIZEAsync();
                });
            });

            CorrectXAML();
            LoadXAML();
        }
    }
}