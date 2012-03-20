#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Windows.Forms;
using ClearCanvas.Desktop.Tables;

namespace ClearCanvas.Desktop.View.WinForms
{
	public partial class ComboBoxCellEditorControl : UserControl
	{
		private ComboBoxCellEditor _editor;

		public ComboBoxCellEditorControl()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Sets the editor on which this control is operating - the control is re-used by the <see cref="TableView"/>.
		/// </summary>
		/// <param name="editor"></param>
		public void SetEditor(ComboBoxCellEditor editor)
		{
			_editor = editor;

			// update value
			_comboBox.DataSource = _editor.GetChoices();
			_comboBox.SelectedItem = _editor.Value;
		}

		private void _comboBox_SelectionChangeCommitted(object sender, System.EventArgs e)
		{
			_editor.Value = _comboBox.SelectedItem;
		}

		private void ComboBoxCellEditorControl_Load(object sender, System.EventArgs e)
		{
			_comboBox.DataSource = _editor.GetChoices();
			_comboBox.SelectedItem = _editor.Value;
		}

		private void _comboBox_Format(object sender, ListControlConvertEventArgs e)
		{
			e.Value = _editor.FormatItem(e.ListItem);
		}
	}
}
