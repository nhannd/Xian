#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Desktop;

namespace ClearCanvas.Desktop.Tables
{
	/// <summary>
	/// Defines an extension point for views onto a <see cref="ComboBoxCellEditor"/>.
	/// </summary>
	[ExtensionPoint]
	public class ComboBoxCellEditorViewExtensionPoint : ExtensionPoint<ITableCellEditorView>
	{
	}

	/// <summary>
	/// Implements a <see cref="ITableCellEditor"/> to show a list of choices in a combox box.
	/// </summary>
	[AssociateView(typeof(ComboBoxCellEditorViewExtensionPoint))]
	public class ComboBoxCellEditor : TableCellEditor
	{
		/// <summary>
		/// Delegate to return a list of items to the user-interface.
		/// </summary>
		public delegate IList GetChoicesDelegate();

		/// <summary>
		/// Delegate to formats an item.
		/// </summary>
		public delegate string FormatItemDelegate(object item);

		private readonly GetChoicesDelegate _getChoicesCallback;
		private readonly FormatItemDelegate _formatItemCallback;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="getChoicesCallback"></param>
		/// <param name="formatItemCallback"></param>
		public ComboBoxCellEditor(GetChoicesDelegate getChoicesCallback, FormatItemDelegate formatItemCallback)
		{
			_getChoicesCallback = getChoicesCallback;
			_formatItemCallback = formatItemCallback;
		}

		/// <summary>
		/// Gets the call back to get a list of choices.
		/// </summary>
		public GetChoicesDelegate GetChoices
		{
			get { return _getChoicesCallback; }
		}

		/// <summary>
		/// Gets the call back to format an item.
		/// </summary>
		public FormatItemDelegate FormatItem
		{
			get { return _formatItemCallback; }
		}
	}
}
