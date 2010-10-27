#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.ImageViewer.Clipboard.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="ClipboardComponent"/>.
    /// </summary>
    public partial class ClipboardComponentControl : ApplicationComponentUserControl
    {
        private readonly ClipboardComponent _component;

        /// <summary>
        /// Constructor.
        /// </summary>
        public ClipboardComponentControl(ClipboardComponent component)
            :base(component)
        {
			_component = component;

			InitializeComponent();

        	_component.DataSourceChanged += delegate { _galleryView.DataSource = _component.DataSource; };
        	_galleryView.DataSource = _component.DataSource;

        	_galleryView.ToolbarModel = _component.ToolbarModel;
        	_galleryView.ContextMenuModel = _component.ContextMenuModel;
        	_galleryView.SelectionChanged += OnSelectionChanged;
        	_galleryView.MultiSelect = true;
        	_galleryView.DragReorder = true;
			//_galleryView.DragOutside = true;
        }

		private void OnSelectionChanged(object sender, EventArgs e)
		{
			_component.SetSelection(_galleryView.Selection);
		}
    }
}
