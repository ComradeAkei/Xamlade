using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Xamlade.Extensions;
using Xamlade.FunctionalAreas;
using Xamlade.XAMLWorkers;

namespace Xamlade.ProgramWindow;



public partial class MainWindow : Window
{
    public static MainWindow _MainWindow = null!;

    public MainWindow()
    {
        _MainWindow = this;
        DataContext = this;
        InitializeComponent();
        WindowInit();

        
        Utils.Init(DebugPanel);
        Workspace.Init(MainCanvas);
        HierarchyControl.Init(MainHierarchyTree);
        PropertiesControl.Init(PropListBox);
        TestWindow.Init(LoadingGif);
        
        ButtonEventsInit();
        
        var listener = new GlobalKeyListener(this);
        listener.KeyPressed += GlobalKeyPressed;
        listener.KeyReleased += GlobalKeyReleased;
    }
    
   
    private void WindowInit()
    {
        this.Icon = new WindowIcon(@"assets/Icon.png");
        var screen = Screens.Primary!.WorkingArea;
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
        var _initialPosition = new PixelPoint(screen.X, screen.Y);
        this.Position = _initialPosition;
        // Вернуть положение окна
        this.PositionChanged += (_, _) => this.Position = _initialPosition;
    }


    //Инициализация событий кнопок управления
    private void ButtonEventsInit()
    {
        RemoveButton.Click+= Workspace.RemoveSelectedjElementHandler;
        XamlizeButton.Click += XAMLGenerator.XAMLIZE;
        RedoButton.Click += History.REDO;
        UndoButton.Click += History.UNDO;
        DeXamLizeButton.Click += ImportXAML.DEXAMLIZE;
        RunWindowButton.Click += TestWindow.RUN_WINDOW;
        DebugButton.Click += Utils.DEBUG;
        MainCanvas.PointerPressed += Workspace.OnjControlPressed;
        foreach (var child in GeneratorPanel.Children)
            ((Button)child).Click += ElementGenerator.GenerateElement;

    }
    

    private void GlobalKeyPressed(KeyEventArgs e)
    {
        if (e.Key == Key.LeftCtrl)
            State.LCtrlPressed = true;
        else if (e.Key == Key.LeftShift)
            State.LShiftPressed = true;
        
    }
    private void GlobalKeyReleased(KeyEventArgs e)
    {
        if (e.Key == Key.LeftCtrl)
            State.LCtrlPressed = false;
        else if(e.Key == Key.Delete)
            Workspace.RemoveSelectedjElement();
        else if (e.Key == Key.LeftShift)
        {
            State.LShiftPressed = false;
            State.RectangleFlag = true;
        }
    }
    

   
}