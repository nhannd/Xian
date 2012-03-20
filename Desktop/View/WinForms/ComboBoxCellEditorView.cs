#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;
using ClearCanvas.Desktop.Tables;

namespace ClearCanvas.Desktop.View.WinForms
{
	[ExtensionOf(typeof(ComboBoxCellEditorViewExtensionPoint))]
	public class ComboBoxCellEditorView : WinFormsView, ITableCellEditorView
	{
		private readonly ComboBoxCellEditorControl _control;

		public ComboBoxCellEditorView()
		{
			_control = new ComboBoxCellEditorControl();
		}

		public void SetEditor(ITableCellEditor editor)
		{
			_control.SetEditor((ComboBoxCellEditor)editor);
		}

		public override object GuiElement
		{
			get { return _control; }
		}
	}}
