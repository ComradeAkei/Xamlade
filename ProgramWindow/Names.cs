using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Xamlade.ProgramWindow
{
    partial class MainWindow
    {
        internal global::Avalonia.Controls.StackPanel Toolbar;
        internal global::Avalonia.Controls.Button XamlizeButton;
        internal global::Avalonia.Controls.Button RunWindowButton;
        internal global::Avalonia.Controls.Button DeXamLizeButton;
        internal global::Avalonia.Controls.Button UndoButton;
        internal global::Avalonia.Controls.Button RedoButton;
        internal global::Avalonia.Controls.Button RemoveButton;
        internal global::Avalonia.Controls.Button SettingsButton;
        internal global::Avalonia.Controls.Button DebugButton;
        internal global::Avalonia.Gif.GifImage LoadingGif;
        internal global::Avalonia.Controls.Panel DebugPanel;
        internal global::Avalonia.Controls.TextBlock DebugTextBlock;
        internal global::Avalonia.Controls.StackPanel HierarchyPanel;
        internal global::Avalonia.Controls.TreeView MainHierarchyTree;
        internal global::Avalonia.Controls.TabControl MainTabControl;
        internal global::Avalonia.Controls.StackPanel GeneratorPanel;
        internal global::Avalonia.Controls.TabItem PropertiesPanel;
        internal global::Avalonia.Controls.StackPanel PropertyPanel;
        internal global::Avalonia.Controls.ListBox PropListBox;
        internal global::Xamlade.jClasses.jCanvas MainCanvas;

        public void InitializeComponent(bool loadXaml = true, bool attachDevTools = true)
        {
            if (loadXaml)
            {
                AvaloniaXamlLoader.Load(this);
            }
#if DEBUG
            if (attachDevTools)
            {
                this.AttachDevTools();
            }
#endif
            var __thisNameScope__ = this.FindNameScope();
            Toolbar = __thisNameScope__?.Find<global::Avalonia.Controls.StackPanel>("Toolbar");
            XamlizeButton = __thisNameScope__?.Find<global::Avalonia.Controls.Button>("XamlizeButton");
            RunWindowButton = __thisNameScope__?.Find<global::Avalonia.Controls.Button>("RunWindowButton");
            DeXamLizeButton = __thisNameScope__?.Find<global::Avalonia.Controls.Button>("DeXamLizeButton");
            UndoButton = __thisNameScope__?.Find<global::Avalonia.Controls.Button>("UndoButton");
            RedoButton = __thisNameScope__?.Find<global::Avalonia.Controls.Button>("RedoButton");
            RemoveButton = __thisNameScope__?.Find<global::Avalonia.Controls.Button>("RemoveButton");
            SettingsButton = __thisNameScope__?.Find<global::Avalonia.Controls.Button>("SettingsButton");
            DebugButton = __thisNameScope__?.Find<global::Avalonia.Controls.Button>("DebugButton");
            LoadingGif = __thisNameScope__?.Find<global::Avalonia.Gif.GifImage>("LoadingGif");
            DebugPanel = __thisNameScope__?.Find<global::Avalonia.Controls.Panel>("DebugPanel");
            DebugTextBlock = __thisNameScope__?.Find<global::Avalonia.Controls.TextBlock>("DebugTextBlock");
            HierarchyPanel = __thisNameScope__?.Find<global::Avalonia.Controls.StackPanel>("HierarchyPanel");
            MainHierarchyTree = __thisNameScope__?.Find<global::Avalonia.Controls.TreeView>("MainHierarchyTree");
            MainTabControl = __thisNameScope__?.Find<global::Avalonia.Controls.TabControl>("MainTabControl");
            GeneratorPanel = __thisNameScope__?.Find<global::Avalonia.Controls.StackPanel>("GeneratorPanel");
            PropertiesPanel = __thisNameScope__?.Find<global::Avalonia.Controls.TabItem>("PropertiesPanel");
            PropertyPanel = __thisNameScope__?.Find<global::Avalonia.Controls.StackPanel>("PropertyPanel");
            PropListBox = __thisNameScope__?.Find<global::Avalonia.Controls.ListBox>("PropListBox");
            MainCanvas = __thisNameScope__?.Find<global::Xamlade.jClasses.jCanvas>("MainCanvas");
        }
    }
}
