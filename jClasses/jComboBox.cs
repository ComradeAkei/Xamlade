using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Xamlade.XAMLWorkers;

namespace Xamlade.jClasses;

    public class jComboBox : ComboBox, JControl, JBroadcastHandler<JControl>, JSelectable
    {
        public int ID = 0;
        protected override Type StyleKeyOverride => typeof(ComboBox);
        public mBorder selectionBorder { get; set; }

        [field: NonSerialized]
        public JChildContainer? jParent { get; set; }
        private string controlType => jElementType.ComboBox.ToString();
        public string Type => controlType;
        [JsonIgnore]
        public mTreeViewItem? mTreeItem { get; set; }
        public int XAMLRating { get; set; }
        [field: NonSerialized]
        public List<string> XAMLPiece { get; set; }

        public new bool IsDropDownOpen
        {
            get => base.IsDropDownOpen;
            set => SetValue(IsDropDownOpenProperty, value);
        }

        public new string? Name
        {
            get => base.Name;
            set => SetValue(NameProperty, value);
        }

        public bool IsPressed { get; set; }
        public event EventHandler<RoutedEventArgs>? Click;

        public jComboBox()
        {
            Broadcast.OnBroadcast += (this as JBroadcastHandler<JControl>).HandleBroadcast;
            XAMLPiece = new List<string>();
            mTreeItem = new mTreeViewItem(this);
        }
        
    }
