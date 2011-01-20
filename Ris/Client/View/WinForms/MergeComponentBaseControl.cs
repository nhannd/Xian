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

using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.View.WinForms
{
	/// <summary>
	/// Provides a Windows Forms user-interface for <see cref="MergeComponentBase"/>
	/// </summary>
	public partial class MergeComponentBaseControl : ApplicationComponentUserControl
	{
		private readonly MergeComponentBase _component;

		/// <summary>
		/// Constructor
		/// </summary>
		public MergeComponentBaseControl(MergeComponentBase component)
			:base(component)
		{
			InitializeComponent();
			_component = component;

			_sourceItem.DataSource = _component.SourceItems;
			_sourceItem.DataBindings.Add("Value", _component, "SelectedDuplicate", true, DataSourceUpdateMode.OnPropertyChanged);
			_sourceItem.Format += delegate(object sender, ListControlConvertEventArgs e) { e.Value = _component.FormatItem(e.ListItem); };

			_targetItem.DataSource = _component.TargetItems;
			_targetItem.DataBindings.Add("Value", _component, "SelectedOriginal", true, DataSourceUpdateMode.OnPropertyChanged);
			_targetItem.Format += delegate(object sender, ListControlConvertEventArgs e) { e.Value = _component.FormatItem(e.ListItem); };

			_report.DataBindings.Add("Value", _component, "MergeReport", true, DataSourceUpdateMode.OnPropertyChanged);
		}

		private void _acceptButton_Click(object sender, EventArgs e)
		{
			_component.Accept();
		}

		private void _cancelButton_Click(object sender, EventArgs e)
		{
			_component.Cancel();
		}

		private void _switchButton_Click(object sender, EventArgs e)
		{
			_component.Switch();
		}
	}
}
