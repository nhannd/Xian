#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Windows.Forms;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.ImageViewer.Layout.Basic.View.WinForms
{
    /// <summary>
    /// WinForms implementation of the <see cref="LayoutComponentViewExtensionPoint"/> extension point.
    /// The actual user-interface is implemented by <see cref="LayoutControl"/>.
    /// </summary>
    [ClearCanvas.Common.ExtensionOf(typeof(LayoutComponentViewExtensionPoint))]
    public class LayoutComponentView : WinFormsView, IApplicationComponentView
    {
        private Control _control;
        private LayoutComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public LayoutComponentView()
        {
        }

        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (LayoutComponent)component;
        }

        #endregion

        /// <summary>
        /// Overridden to return an instance of <see cref="LayoutControl"/>
        /// </summary>
        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new LayoutControl(_component);
                }
                return _control;
            }
        }
    }
}
