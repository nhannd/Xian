#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Windows.Forms;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.Desktop.View.WinForms
{
    /// <summary>
    /// Base class for user controls that are created by an Application Component View.
    /// </summary>
    public partial class ApplicationComponentUserControl : CustomUserControl
    {
        /// <summary>
        /// Constructor required for Designer support.  Do not use this constructor in application code.
        /// </summary>
        public ApplicationComponentUserControl()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="component"></param>
        public ApplicationComponentUserControl(IApplicationComponent component)
        {
            InitializeComponent();

            _errorProvider.DataSource = component;
            component.ValidationVisibleChanged += ValidationVisibleChangedEventHandler;

            if (component is ApplicationComponent)
            {
                ActionModelNode menuModel = ((ApplicationComponent)component).MetaContextMenuModel;
                if (menuModel != null)
                {
                    ToolStripBuilder.BuildMenu(_contextMenu.Items, menuModel.ChildNodes);
                }
            }
        }

        /// <summary>
        /// Gets the default <see cref="System.Windows.Forms.ErrorProvider"/> for this user control
        /// </summary>
        public ErrorProvider ErrorProvider
        {
            get { return _errorProvider; }
        }

        private void ValidationVisibleChangedEventHandler(object sender, EventArgs e)
        {
            _errorProvider.UpdateBinding();
        }
    }
}
