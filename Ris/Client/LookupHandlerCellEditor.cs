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
