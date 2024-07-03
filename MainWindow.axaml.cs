using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Gif;
using Avalonia.Media.Imaging;
using AvaloniaColorPicker;
using Avalonia.Markup;
using Microsoft.Diagnostics.Runtime;

namespace Xamlade;



public partial class MainWindow : Window
{
    public static MainWindow _MainWindow;
    public string SelectedGif => @"avares://Xamlade/RES/loading.gif";


    #region Globals

    //Отладочный итератор
    private static int i = 0;

    //Случайное число
    private static Random random;

    private PixelPoint _initialPosition;
    

    


    //Оригинальный фон выбранного элемента
    private static IBrush? selectedOriginalBackground;

    private static TreeView _MainHierarchyTree;

    #endregion

    
    
    public MainWindow()
    {
        //Дырку закрыть
        _MainWindow = this;
        
        InitializeComponent();
        WindowInit();
        DataContext = this;
        
        
        Workspace.Init(MainCanvas);
        HierarchyControl.Init(MainHierarchyTree);
        ButtonEventsInit();
        
        
        
        WindowState = WindowState.Maximized;
        selectedOriginalBackground = MainCanvas.Background;
        MainHierarchyTree.Items.Add(new mTreeViewItem(MainCanvas));
        HierarchyControl.selectedTreeItem = MainCanvas.mTreeItem;
        MainHierarchyTree.SelectedItem = HierarchyControl.selectedTreeItem;
        random = new Random();
        PropListItems = constructor.Invoke(new object[] { }) as ItemCollection;
        var listener = new GlobalKeyListener(this);
        listener.KeyPressed += GlobalKeyPressed;
        listener.KeyReleased += GlobalKeyReleased;

    }

    private void WindowInit()
    {
        this.Icon = new WindowIcon(@"assets/Icon.png");
        var screen = Screens.Primary.WorkingArea;
        this.WindowState = WindowState.Maximized;
        // Установить размеры окна равными размерам экрана
        this.Width = screen.Width;
        this.Height = screen.Height;

        // Установить минимальные и максимальные размеры окна
        this.MinWidth = screen.Width;
        this.MinHeight = screen.Height;
        this.MaxWidth = screen.Width;
        this.MaxHeight = screen.Height;
        

        // Запретить изменение размеров окна
        this.CanResize = false;
        
        //Привязать окно к начальному положению
        this.WindowStartupLocation = WindowStartupLocation.Manual;
        _initialPosition = new PixelPoint(screen.X, screen.Y);
        this.Position = _initialPosition;

        // Подписка на событие изменения положения окна
        this.PositionChanged += MainWindow_PositionChanged;
    }

    //Инициализация событий кнопок управления
    private void ButtonEventsInit()
    {
        RemoveButton.Click+= Workspace.RemoveSelectedjElement;
        XamlizeButton.Click += XAMLGenerator.XAMLIZE;
        MainCanvas.PointerPressed += Workspace.OnjControlPressed;
    }
    private void MainWindow_PositionChanged(object sender, PixelPointEventArgs e)
    {
        // Возвращаем окно на исходное положение
        this.Position = _initialPosition;
    }

    private void GlobalKeyPressed(KeyEventArgs e)
    {
        if (e.Key == Key.LeftCtrl)
            LCtrlPressed = true;
    }
    private void GlobalKeyReleased(KeyEventArgs e)
    {
        if (e.Key == Key.LeftCtrl)
            LCtrlPressed = false;
        else if(e.Key == Key.Delete)
            //ИСПРАВИТЬ
            Workspace.RemoveSelectedjElement(null,null);
    }

    
    



    


    


    

    private void ShowProperties()
    {
        try
        {
            PropListItems.Clear();
        }
        catch
        {
            return;
        }


        if (HierarchyControl.selectedTreeItem.element == MainCanvas) return;
        //  Type type = selectedTreeItem.element.GetType();
        //   = type.GetProperties();

        Type type = HierarchyControl.selectedTreeItem.element.GetType();
        var props = type.GetProperties();
    
        foreach (var prop in props)
        {
            if (!Constants.ExcludedWords.Contains(prop.Name))
            {
                var prop_type = type.GetProperty(prop.Name).PropertyType;
                AddPropItem(prop.Name, prop.GetValue(HierarchyControl.selectedTreeItem.element), prop_type);
                //KeyValueList.Add(new KeyValue { Key = prop.Name, Value = prop.GetValue(selectedTreeItem.element)?.ToString() });
            }
        }

        FieldInfo privateField =
            typeof(ItemsControl).GetField("_items", BindingFlags.NonPublic | BindingFlags.Instance);
        privateField.SetValue(PropListBox, PropListItems);
    }

    // Выбрать редактируемый элемент
    private void SelectjElement(JControl element)
    {
        if ((element is null) || (element.Name == null)) return;
        HierarchyControl.selectedTreeItem = element.mTreeItem;
        MainHierarchyTree.SelectedItem = HierarchyControl.selectedTreeItem;
        selectedOriginalBackground = HierarchyControl.selectedTreeItem.element.Background;
        Workspace.InitMovable(HierarchyControl.selectedTreeItem.element);
        HierarchyControl.selectedTreeItem.element.Focus();
        ShowProperties();
    }


    private void MainHierarchyTree_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        var item = ((TreeView)sender).SelectedItem as mTreeViewItem;
        //     if(item == selectedTreeItem) return;
        if (item != null)
            SelectjElement(item.element);
        else
            SelectjElement(MainCanvas);
        e.Handled = true;
    }

    

    
    
    

    private async void OnChooseImageClick(object? sender, RoutedEventArgs e)
    {
        OpenFileDialog dialog = new OpenFileDialog();
        dialog.Title = "Выберите изображение";
        dialog.AllowMultiple = false;
        dialog.Filters.Add(new FileDialogFilter
        {
            Name = "Изображения",
            Extensions = { "png", "jpg", "jpeg", "gif", "bmp" }
        });
        Task<string[]> task = dialog.ShowAsync(this);
        
        // Дожидаемся завершения задачи (await)
        string[] result = await task;

        // Обрабатываем результат
        if (result != null && result.Length > 0)
        {
            string fileName = Path.GetFileName(result[0]);
            string targetFilePath = Path.Combine("assets", fileName);
            File.Copy(result[0], targetFilePath, true);
            ((jImage)HierarchyControl.selectedTreeItem.element).jImageSource = @"assets/" + fileName;
            ((jImage)HierarchyControl.selectedTreeItem.element).Source = new Bitmap(((jImage)HierarchyControl.selectedTreeItem.element).jImageSource);
        }
        else
        {
            return;
        }
    }
    private void OnEnumPropertyChanged(object? sender, SelectionChangedEventArgs e)
    {
        var comboBox = sender as ComboBox;
        var parentPanel = comboBox.Parent as DockPanel;
        var txt_blc = parentPanel.Children[0] as TextBlock;
        var prop_name = txt_blc.Text;
        Type jElement_type = HierarchyControl.selectedTreeItem.element.GetType();
        var prop_type = jElement_type.GetProperty(prop_name).PropertyType;
        var prop = jElement_type.GetProperty(prop_name);
        object enumValue = Enum.Parse(prop_type, comboBox.SelectedItem.ToString());
        
        object prevalue = prop.GetValue(HierarchyControl.selectedTreeItem.element);
        AddHistoryItem(new Change(HierarchyControl.selectedTreeItem.element,prop_name,prevalue));
        
        prop.SetValue(HierarchyControl.selectedTreeItem.element, enumValue);
    }

    private void OnColorChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property == AvaloniaColorPicker.ColorButton.ColorProperty)
        {
            var colorButton = sender as ColorButton;
            var parentPanel = colorButton.Parent.Parent as DockPanel;
            var txt_blc = parentPanel.Children[0] as TextBlock;
            var prop_name = txt_blc.Text;
           
            Type jElement_type = HierarchyControl.selectedTreeItem.element.GetType();
            var prop_type = jElement_type.GetProperty(prop_name).PropertyType;
            var prop = jElement_type.GetProperty(prop_name);
           
            object prevalue = prop.GetValue(HierarchyControl.selectedTreeItem.element);
            AddHistoryItem(new Change(HierarchyControl.selectedTreeItem.element,prop_name,prevalue));
           
            prop.SetValue(HierarchyControl.selectedTreeItem.element, new SolidColorBrush(colorButton.Color));
            var textBlock = ((StackPanel)(colorButton.Parent)).Children[0] as TextBlock;
            textBlock.Text = colorButton.Color.ToString();
        }
    }

    private void OnBoolPropertyChanged(object? sender, RoutedEventArgs e)
    {
        var checkBox = sender as CheckBox;
        var parentPanel = checkBox.Parent as DockPanel;
        var txt_blc = parentPanel.Children[0] as TextBlock;
        var prop_name = txt_blc.Text;
        Type jElement_type = HierarchyControl.selectedTreeItem.element.GetType();
        var prop = jElement_type.GetProperty(prop_name);
       
        object prevalue = prop.GetValue(HierarchyControl.selectedTreeItem.element);
        AddHistoryItem(new Change(HierarchyControl.selectedTreeItem.element,prop_name,prevalue));
        
        prop.SetValue(HierarchyControl.selectedTreeItem.element, checkBox.IsChecked);
    }

    private void OnPropertyChanged(object? sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter) return;
        var textBox = sender as TextBox;

        var parentPanel = textBox.Parent as DockPanel;
        var txt_blc = parentPanel.Children[0] as TextBlock;
        var prop_name = txt_blc.Text;
        Type jElement_type = HierarchyControl.selectedTreeItem.element.GetType();
        var prop_type = jElement_type.GetProperty(prop_name).PropertyType;
        var prop = jElement_type.GetProperty(prop_name);
        textBox.Foreground = new SolidColorBrush(Color.Parse("#88F1FF"));
        
        object prevalue = prop.GetValue(HierarchyControl.selectedTreeItem.element);
        AddHistoryItem(new Change(HierarchyControl.selectedTreeItem.element,prop_name,prevalue));

        if (textBox.Text == "не число") return;
        try
        {
            if (prop.Name == "Name")
            {
                FieldInfo privateField =
                    typeof(StyledElement).GetField("_name", BindingFlags.NonPublic | BindingFlags.Instance);
                privateField.SetValue(HierarchyControl.selectedTreeItem.element, textBox.Text);
                HierarchyControl.selectedTreeItem.Header = textBox.Text;
            }
            else if (prop.Name == "Content")
                prop.SetValue(HierarchyControl.selectedTreeItem.element, textBox.Text);
            else if (prop_type == typeof(string))
                prop.SetValue(HierarchyControl.selectedTreeItem.element, textBox.Text);
            else if (prop_type == typeof(int))
                prop.SetValue(HierarchyControl.selectedTreeItem.element, Convert.ToInt32(textBox.Text));
            else if (prop_type == typeof(double))
            {
                textBox.Text = textBox.Text.Replace('.', ',');
                prop.SetValue(HierarchyControl.selectedTreeItem.element, Convert.ToDouble(textBox.Text));
            }
            else if (prop_type == typeof(IBrush))
            {
                var brush = new SolidColorBrush(Color.Parse(textBox.Text));
                prop.SetValue(HierarchyControl.selectedTreeItem.element, brush);
                textBox.Foreground = new SolidColorBrush(Color.Parse(textBox.Text));
            }
            else if (prop_type == typeof(Thickness))
            {
                var values = textBox.Text.Split(',');
                var rect = new Thickness(Convert.ToInt32(values[0]), Convert.ToInt32(values[1]),
                    Convert.ToInt32(values[2]), Convert.ToInt32(values[3]));
                prop.SetValue(HierarchyControl.selectedTreeItem.element, rect);
            }
            else if (prop_type == typeof(CornerRadius))
            {
                var values = textBox.Text.Split(',');
                var rect = new CornerRadius(Convert.ToInt32(values[0]), Convert.ToInt32(values[1]),
                    Convert.ToInt32(values[2]), Convert.ToInt32(values[3]));
                prop.SetValue(HierarchyControl.selectedTreeItem.element, rect);
            }
            else if (prop_type == typeof(Rect))
            {
                var values = textBox.Text.Split(',');
                var rect = new Rect(Convert.ToInt32(values[0]), Convert.ToInt32(values[1]),
                    Convert.ToInt32(values[2]), Convert.ToInt32(values[3]));
                prop.SetValue(HierarchyControl.selectedTreeItem.element, rect);
            }
        }
        catch
        {
            textBox.Text = "Некорректное значение";
            textBox.Foreground = Brushes.Red;
        }
    }


    private async void RUN_WINDOW(object? sender, RoutedEventArgs e)
    {
        CopyAssets();
        LoadingGif.IsVisible = true;
        
        await ExecuteLinuxCommandAsync(@"XamladeDemo/BUILD.sh");
        LoadingGif.IsVisible = false;
        await ExecuteLinuxCommandAsync(@"XamladeDemo/RUN.sh");
    }


    public static void CopyAssets()
    {
        string[] files = Directory.GetFiles(@"assets");
        string targetDirectory = @"XamladeDemo/assets";
        if (!Directory.Exists(targetDirectory))
        {
            Directory.CreateDirectory(targetDirectory);
        }
        foreach (string file in files)
        {
            string fileName = Path.GetFileName(file);
            string destFile = Path.Combine(targetDirectory, fileName);
            File.Copy(file, destFile, true);
        }
    }
    public static async Task<string> ExecuteLinuxCommandAsync(string command)
    {
        using (Process process = new Process())
        {
            process.StartInfo.FileName = "/bin/bash";
            process.StartInfo.Arguments = $"-c \"{command}\"";
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;

            process.Start();

            string result = await process.StandardOutput.ReadToEndAsync();
            string error = await process.StandardError.ReadToEndAsync();

            if (!string.IsNullOrEmpty(error))
            {
                throw new Exception($"Error: {error}");
            }

            return result;
        }
    }
 

    private async void DEXAMLIZE(object? sender, RoutedEventArgs e)
    {


        await RunDeXAMLIZE(this);

    }

    public System.String ass;

    private void DEBUG(object? sender, RoutedEventArgs e)
    {
       
    }
}