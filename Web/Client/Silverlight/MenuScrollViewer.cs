#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Markup;

namespace ClearCanvas.Web.Client.Silverlight
{
    public interface IMenuItemsContainer
    {
        IMenu Parent { get; set; }
    }

    public class MenuScrollViewer : CustomScrollViewer, IMenuItemsContainer
    {
        private IMenu _parent;

        public MenuScrollViewer()
        {
        }

        protected override void OnScrolling()
        {
            if (_parent == null)
                return;

            foreach (var item in _parent.Items.OfType<IMenuItem>())
                item.IsExpanded = false;
        }

        #region IMenuItemsContainer Members

        IMenu IMenuItemsContainer.Parent
        {
            get { return _parent; }
            set { _parent = value; }
        }

        #endregion
    }
}
