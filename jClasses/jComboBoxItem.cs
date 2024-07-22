using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Xamlade.XAMLWorkers;

namespace Xamlade.jClasses;

    public class jComboBoxItem : ComboBoxItem, JControl, JSelectable
    {
        private string controlType => jElementType.ComboBoxItem.ToString();
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
        }

        // Вариант, если вы хотите реализовать логику добавления и удаления дочерних элементов
        public void AddChild(JControl child)
        {
            if (jChildren.Any())
                return;
            jChildren.Add(child);
            this.Content = child;
        }

        public void RemoveChild(JControl? child = null)
        {
            jChildren.Clear();
            this.Content = null;
        }

        public mBorder selectionBorder { get; set; }
    }
