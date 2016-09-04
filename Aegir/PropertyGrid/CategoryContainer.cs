﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Aegir.PropertyGrid
{
    public class CategoryContainer : Expander
    {
        private StackPanel internalPanel;

        public CategoryContainer()
        {
            internalPanel = new StackPanel();
            this.Content = internalPanel;
        }

        public void AddProperty(PropertyContainer property)
        {
            this.internalPanel.Children.Add(property);
        }
    }
}
