#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Desktop.Tables;
using ClearCanvas.Common;
using ClearCanvas.Desktop;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Defines an extension point for views onto a <see cref="LookupHandlerCellEditor"/>.
	/// </summary>
	[ExtensionPoint]
	public class LookupHandlerCellEditorViewExtensionPoint : ExtensionPoint<ITableCellEditorView>
	{
	}

	/// <summary>
	/// Implements a <see cref="ITableCellEditor"/> in terms of a <see cref="ILookupHandler"/>.
	/// </summary>
	[AssociateView(typeof(LookupHandlerCellEditorViewExtensionPoint))]
	public class LookupHandlerCellEditor : TableCellEditor
	{
		private readonly ILookupHandler _lookupHandler;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="lookupHandler"></param>
		public LookupHandlerCellEditor(ILookupHandler lookupHandler)
		{
			_lookupHandler = lookupHandler;
		}

		/// <summary>
		/// Gets the lookup handler associated with this cell editor.
		/// </summary>
		public ILookupHandler LookupHandler
		{
			get { return _lookupHandler; }
		}
	}
}
