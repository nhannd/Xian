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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.ImageViewer.Services.Tools.View.WinForms
{
	public partial class ReceiveQueueApplicationComponentControl : ApplicationComponentUserControl
    {
        private ReceiveQueueApplicationComponent _component;

        public ReceiveQueueApplicationComponentControl(ReceiveQueueApplicationComponent component)
        {
            InitializeComponent();

            _component = component;

			_receiveTable.Table = _component.ReceiveTable;

			_receiveTable.ToolbarModel = _component.ToolbarModel;
			_receiveTable.MenuModel = _component.ContextMenuModel;

			_receiveTable.SelectionChanged += new EventHandler(OnSelectionChanged);
			_receiveTable.ItemDoubleClicked += new EventHandler(OnItemDoubleClicked);
			BindingSource bindingSource = new BindingSource();
			bindingSource.DataSource = _component;

			_titleBar.DataBindings.Add("Text", _component, "Title", true, DataSourceUpdateMode.OnPropertyChanged);
        }

		void OnItemDoubleClicked(object sender, EventArgs e)
		{
			_component.ItemDoubleClick();
		}

		void OnSelectionChanged(object sender, EventArgs e)
		{
			_component.SetSelection(_receiveTable.Selection);
		}
    }
}
