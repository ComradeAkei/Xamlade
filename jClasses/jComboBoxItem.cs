using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Xamlade.XAMLWorkers;

namespace Xamlade.jClasses;

    public class jComboBoxItem : ComboBoxItem, JControl, JSelectable, JChildContainer
    {
        private string controlType => jElementType.ComboBoxItem.ToString();
        protected override Type StyleKeyOverride => typeof(ComboBoxItem);
        public mTreeViewItem? mTreeItem { get; set; }
        public JChildContainer? jParent { get; set; }
        public string Type => controlType;
        public int XAMLRating { get; set; }
        public List<string> XAMLPiece { get; set; }
        public bool IsPressed { get; set; }
        public event EventHandler<RoutedEventArgs>? Click;

        public List<JControl> jChildren { get; }

        public jComboBoxItem(string name)
        {
            Name = name;
            jChildren = new List<JControl>();
            XAMLPiece = new List<string>();
            mTreeItem = new mTreeViewItem(this);
            this.AddHandler(PointerPressedEvent, OnPointerPressed, handledEventsToo: true);
        }

        private void OnPointerPressed(object sender, PointerPressedEventArgs e)
        {
            IsPressed = true;
            Click?.Invoke(this, new RoutedEventArgs());
        }
       
        public void AddChild(JControl child)
        {
            RemoveChild();
            jChildren.Add(child);
            this.Content = child;
            child.jParent = this;
            (child as Control).IsHitTestVisible = false;
            IsHitTestVisible = true;
        }

        public void RemoveChild(JControl? child = null)
        {
            if(mTreeItem.Items.Any())
                this.mTreeItem.Items.Remove(this.jChildren[0].mTreeItem);
            jChildren.Clear();
            this.Content = null;
        }

        public mBorder selectionBorder { get; set; }
    }
