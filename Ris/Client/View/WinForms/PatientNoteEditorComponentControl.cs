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
	/// Provides a Windows Forms user-interface for <see cref="PatientNoteEditorComponent"/>
	/// </summary>
	public partial class PatientNoteEditorComponentControl : ApplicationComponentUserControl
	{
		private readonly PatientNoteEditorComponent _component;

		/// <summary>
		/// Constructor
		/// </summary>
		public PatientNoteEditorComponentControl(PatientNoteEditorComponent component)
			:base(component)
		{
			InitializeComponent();
			_component = component;

			_category.DataSource = _component.CategoryChoices;
			_category.Format +=
				delegate(object sender, ListControlConvertEventArgs e)
				{
					 e.Value = _component.FormatNoteCategory(e.ListItem);
				};
			_category.DataBindings.Add("Value", _component, "Category", true, DataSourceUpdateMode.OnPropertyChanged);
			_description.DataBindings.Add("Value", _component, "CategoryDescription", true, DataSourceUpdateMode.OnPropertyChanged);
			_comment.DataBindings.Add("Value", _component, "Comment", true, DataSourceUpdateMode.OnPropertyChanged);
			_acceptButton.DataBindings.Add("Enabled", _component, "AcceptEnabled", true, DataSourceUpdateMode.OnPropertyChanged);

			_expiryDate.DataBindings.Add("Value", _component, "ExpiryDate", true, DataSourceUpdateMode.OnPropertyChanged);

			_category.Enabled = _component.IsCommentEditable;
			_comment.ReadOnly = !_component.IsCommentEditable;
			_expiryDate.Enabled = _component.IsExpiryDateEditable;
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
