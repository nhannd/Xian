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
using System.Text;

namespace ClearCanvas.Desktop.Tables
{
	/// <summary>
	/// Defines an interface to a view onto a <see cref="ITableCellEditor"/>.
	/// </summary>
	public interface ITableCellEditorView : IView
	{
		/// <summary>
		/// Sets the editor with which this view is associated.
		/// </summary>
		/// <param name="editor"></param>
		void SetEditor(ITableCellEditor editor);
	}
}
