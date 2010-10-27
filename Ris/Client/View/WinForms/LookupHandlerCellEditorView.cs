#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Desktop.View.WinForms;
using ClearCanvas.Desktop.Tables;

namespace ClearCanvas.Ris.Client.View.WinForms
{
	[ExtensionOf(typeof(LookupHandlerCellEditorViewExtensionPoint))]
	public class LookupHandlerCellEditorView : WinFormsView, ITableCellEditorView
	{
		private readonly LookupHandlerCellEditorControl _control;

		public LookupHandlerCellEditorView()
		{
			_control = new LookupHandlerCellEditorControl();
		}

		public void SetEditor(ITableCellEditor editor)
		{
			_control.SetEditor((LookupHandlerCellEditor)editor);
		}

		public override object GuiElement
		{
			get { return _control; }
		}
	}
}
