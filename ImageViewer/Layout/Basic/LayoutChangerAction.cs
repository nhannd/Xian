using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.ImageViewer.Layout.Basic
{
	[ExtensionPoint]
	public class LayoutChangerActionViewExtensionPoint : ExtensionPoint<IActionView> {}

	[AssociateView(typeof (LayoutChangerActionViewExtensionPoint))]
	public class LayoutChangerAction : Action
	{
		private readonly SetLayoutCallback _setLayoutCallback;
		private readonly int _maxRows;
		private readonly int _maxColumns;

		public LayoutChangerAction(string actionID, int maxRows, int maxColumns, SetLayoutCallback callback, ActionPath path, IResourceResolver resourceResolver)
			: base(actionID, path, resourceResolver)
		{
			Platform.CheckForNullReference(callback, "callback");
			_setLayoutCallback = callback;
			_maxRows = maxRows;
			_maxColumns = maxColumns;
		}

		public int MaxRows
		{
			get { return _maxRows; }
		}

		public int MaxColumns
		{
			get { return _maxColumns; }
		}

		public void SetLayout(int rows, int columns)
		{
			_setLayoutCallback(rows, columns);
		}
	}

	public delegate void SetLayoutCallback(int rows, int columns);
}