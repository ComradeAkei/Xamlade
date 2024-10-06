using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Xamlade.Extensions;
using Xamlade.LinkWorkers;
using Xamlade.XAMLWorkers;

namespace Xamlade.jClasses;

    public class jComboBoxItem : ComboBoxItem, JControl, JSelectable, JChildContainer, JBroadcastHandler<JControl>
    {
        private string controlType => jElementType.ComboBoxItem.ToString();
        
        
        protected override Type StyleKeyOverride => typeof(ComboBoxItem);

        
        public Beholder Beholder { get; set; }
        public Dictionary<string, JChildContainer.ContainerSetPropertyDelegate> SpecialSetDelegates { get; set; }
        public JChildContainer? _jParent { get; set; }
     //   public JChildContainer? jParent { get; set; }
        public string Type => controlType;
        public int XAMLRating { get; set; }
        public List<string> XAMLPiece { get; set; }
        public bool IsPressed { get; set; }
        public event EventHandler<RoutedEventArgs>? Click;

        public List<(string, JChildContainer.ContainerPropertyDelegate)> ContainerProperties { get; set; }
        public static Dictionary<string, JChildContainer.ContainerSetPropertyDelegate> ContainerSetProperties
        {
            get;
            set;
        }

        public List<JControl> jChildren { get; set; }

        public jComboBoxItem()
        {
            SpecialSetDelegates = new();
            Name = $"jComboboxItem + {Utils.NextgenIterator}";
            jChildren = new List<JControl>();
            Broadcast.OnBroadcast += (this as JBroadcastHandler<JControl>).HandleBroadcast;
            XAMLPiece = new List<string>();
            this.AddHandler(PointerPressedEvent, OnPointerPressed, handledEventsToo: true);
        }
        // public jComboBoxItem(string name):this()
        // {
            // Name = name;
            // Beholder.mTreeItem.Header = name;
        // }

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
            if(Beholder.mTreeItem.Items.Any())
                this.Beholder.mTreeItem.Items.Remove(this.jChildren[0].Beholder.mTreeItem);
            jChildren.Clear();
            this.Content = null;
        }

        static jComboBoxItem()
        {
            ContainerSetProperties = new();
        }
        public void InitContainerProperties()
        {
            ContainerProperties = new();
        }

        public mBorder selectionBorder { get; set; }

        public Dictionary<string, Dictionary<string, Property>> xPropertiesGroup { get; set; }

        public void AddSpecialProperties()
        {
            return;
        }
    }
