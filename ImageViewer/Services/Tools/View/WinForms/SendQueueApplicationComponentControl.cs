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
	/// <summary>
	/// Provides a Windows Forms user-interface for <see cref="SendQueueApplicationComponent"/>
	/// </summary>
	public partial class SendQueueApplicationComponentControl : ApplicationComponentUserControl
	{
		private SendQueueApplicationComponent _component;
		private bool _updating;
		private int _failedItemsCount;

		/// <summary>
		/// Constructor
		/// </summary>
		public SendQueueApplicationComponentControl(SendQueueApplicationComponent component)
			: base(component)
		{
			InitializeComponent();
			FailedItemsCount = 0;

			_component = component;
			_component.SelectionUpdated += OnComponentSelectionUpdated;

			_sendTable.Table = _component.SendTable;

			_sendTable.ToolbarModel = _component.ToolbarModel;
			_sendTable.MenuModel = _component.ContextMenuModel;

			_sendTable.SelectionChanged += new EventHandler(OnSelectionChanged);

			BindingSource bindingSource = new BindingSource();
			bindingSource.DataSource = _component;

			_titleBar.DataBindings.Add("Text", _component, "Title", true, DataSourceUpdateMode.OnPropertyChanged);
			DataBindings.Add("FailedItemsCount", _component, "FailedItemsCount", false, DataSourceUpdateMode.Never);
		}

		private void DoDispose(bool disposing)
		{
			if (disposing)
			{
				if (_component != null)
				{
					_component.SelectionUpdated -= OnComponentSelectionUpdated;
					_component = null;
				}
			}
		}

		private void OnComponentSelectionUpdated(object sender, EventArgs e)
		{
			if (!_updating)
			{
				_sendTable.Selection = _component.Selection;
			}
		}

		private void OnSelectionChanged(object sender, EventArgs e)
		{
			_updating = true;
			try
			{
				_component.SetSelection(_sendTable.Selection);
			}
			finally
			{
				_updating = false;
			}
		}

		public int FailedItemsCount
		{
			get { return _failedItemsCount; }
			set
			{
				if (_failedItemsCount != value)
				{
					var oldCount = _failedItemsCount;
					_failedItemsCount = value;
					_failuresStatusItem.Text = string.Format(SR.FormatFailedItemsStatusMessage, _failedItemsCount);

					if (_failedItemsCount > oldCount)
					{
						_toolTip.Show(SR.MessageOneOrMoreFailedSendTasks, _statusBar, _failuresStatusItem.Bounds.Location, 5000);
					}
				}
			}
		}
	}
}
