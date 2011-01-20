#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Desktop.Explorer.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="ExplorerConfigurationComponent"/>.
    /// </summary>
    [ExtensionOf(typeof(ExplorerConfigurationComponentViewExtensionPoint))]
    public class ExplorerConfigurationComponentView : WinFormsView, IApplicationComponentView
    {
        private ExplorerConfigurationComponent _component;
        private ExplorerConfigurationComponentControl _control;

        #region IApplicationComponentView Members

        /// <summary>
        /// Called by the host to assign this view to a component.
        /// </summary>
        public void SetComponent(IApplicationComponent component)
        {
            _component = (ExplorerConfigurationComponent)component;
        }

        #endregion

        /// <summary>
        /// Gets the underlying GUI component for this view.
        /// </summary>
        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new ExplorerConfigurationComponentControl(_component);
                }
                return _control;
            }
        }
    }
}
