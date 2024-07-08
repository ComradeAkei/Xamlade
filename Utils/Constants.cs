using System.Collections.Generic;

namespace Xamlade;

public static class Constants
{
    public static readonly List<string> ExcludedWords = new()
    {
        "jParent", "mTreeItem", "Presenter", "Template", "IsLoaded",
        "DesiredSize", "IsMeasureValid", "IsArrangeValid", "RenderTransform",
        "DataContext", "IsInitialized", "Parent", "ActualThemeVariant",
        "Transitions", "Item", "Type", "IsPressed", "ClickMode", "IsDefault", "IsCancel",
        "DataTemplates", "Focusable", "IsEnabled", "IsKeyboardFocusWithin",
        "IsFocused", "IsHitTestVisible", "IsPointerOver", "IsTabStop",
        "IsEffectivelyEnabled", "TabIndex", "KeyBindings", "GestureRecognizers",
        "UseLayoutRounding", "ClipToBounds", "IsEffectivelyVisible",
        "HasMirrorTransform", "RenderTransformOrigin", "ZIndex", "Classes",
        "Styles", "StyleKey", "Resources", "Command", "HotKey",
        "CommandParameter", "Flyout", "Theme", "Clip", "TemplatedParent", "Effect",
        "OpacityMask", "Bounds", "Cursor", "Tag", "ContextFlyout", "ContextMenu", "FocusAdorner", "IsItemsHost",
        "Children", "jChildren", "FontFamily", "TextDecoration", "ContentTemplate", "FlowDirection", "Inlines",
        "TextLayout",
        "XAMLRating", "XAMLPiece", "CanPaste", "CanUndo","jImageSource", "selectionBorder"
    };
}