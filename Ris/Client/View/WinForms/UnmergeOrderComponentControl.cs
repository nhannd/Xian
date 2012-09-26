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
	/// Provides a Windows Forms user-interface for <see cref="UnmergeOrderComponent"/>
	/// </summary>
	public partial class UnmergeOrderComponentControl : ApplicationComponentUserControl
	{
		private readonly UnmergeOrderComponent _component;

		/// <summary>
		/// Constructor
		/// </summary>
		public UnmergeOrderComponentControl(UnmergeOrderComponent component)
			: base(component)
		{
			InitializeComponent();
			_component = component;

			_cancelReason.DataSource = _component.ReasonChoices;
			_cancelReason.DataBindings.Add("Value", _component, "SelectedReason", true, DataSourceUpdateMode.OnPropertyChanged);
			_okButton.DataBindings.Add("Enabled", _component, "AcceptEnabled", false, DataSourceUpdateMode.OnPropertyChanged);
		}

		private void _okButton_Click(object sender, EventArgs e)
		{
			_component.Accept();
		}

		private void _cancelButton_Click(object sender, EventArgs e)
		{
			_component.Unmerge();
		}
	}
}
