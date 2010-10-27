#region License

// Copyright (c) 2010, ClearCanvas Inc.
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
	/// Provides a Windows Forms user-interface for <see cref="CannedTextCategoryEditorComponent"/>.
	/// </summary>
	public partial class CannedTextCategoryEditorComponentControl : ApplicationComponentUserControl
	{
		private readonly CannedTextCategoryEditorComponent _component;

		/// <summary>
		/// Constructor.
		/// </summary>
		public CannedTextCategoryEditorComponentControl(CannedTextCategoryEditorComponent component)
			: base(component)
		{
			_component = component;
			InitializeComponent();

			_category.DataSource = _component.CategoryChoices;
			_category.DataBindings.Add("Text", _component, "Category", true, DataSourceUpdateMode.OnPropertyChanged);
		}

		private void _acceptButton_Click(object sender, EventArgs e)
		{
			_component.Accept();
		}

		private void _cancelButton_Click(object sender, EventArgs e)
		{
			_component.Cancel();
		}
	}
}
