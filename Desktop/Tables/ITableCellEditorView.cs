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
