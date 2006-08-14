using System;
using System.Collections.Generic;
using System.Diagnostics;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// Summary description for UIEventHandler.
	/// </summary>
	internal class UIEventHandler<TItem> : IUIEventHandler
	{
		private IEnumerable<TItem> _collection;

		public UIEventHandler(IEnumerable<TItem> collection)
		{
			Platform.CheckForNullReference(collection, "collection");

			_collection = collection;
		}

		#region IUIEventHandler Members

		public bool OnMouseDown(XMouseEventArgs e)
		{
			Platform.CheckForNullReference(e, "e");

			IUIEventHandler handler;
			IClientArea clientArea;

			foreach (TItem item in _collection)
			{
				if (item != null)
				{
					clientArea = item as IClientArea;
					handler = item as IUIEventHandler;

					if (clientArea != null)
					{
						if (clientArea.DrawableClientRectangle.Contains(e.X, e.Y))
							return handler.OnMouseDown(e);
					}
					else
						return handler.OnMouseDown(e);
				}
			}

			return false;
		}

		public bool OnMouseMove(XMouseEventArgs e)
		{
			Platform.CheckForNullReference(e, "e");

			IUIEventHandler handler;
			IClientArea clientArea;

			foreach (TItem item in _collection)
			{
				if (item != null)
				{
					clientArea = item as IClientArea;
					handler = item as IUIEventHandler;

					if (clientArea != null)
					{
						if (clientArea.DrawableClientRectangle.Contains(e.X, e.Y))
							return handler.OnMouseMove(e);
					}
					else
						return handler.OnMouseMove(e);
				}
			}

			return false;
		}

		public bool OnMouseUp(XMouseEventArgs e)
		{
			Platform.CheckForNullReference(e, "e");

			IUIEventHandler handler;
			IClientArea clientArea;

			foreach (TItem item in _collection)
			{
				if (item != null)
				{
					clientArea = item as IClientArea;
					handler = item as IUIEventHandler;

					if (clientArea != null)
					{
						if (clientArea.DrawableClientRectangle.Contains(e.X, e.Y))
							return handler.OnMouseUp(e);
					}
					else
						return handler.OnMouseUp(e);
				}
			}

			return false;
		}

		public bool OnMouseWheel(XMouseEventArgs e)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public bool OnKeyDown(XKeyEventArgs e)
		{
			Platform.CheckForNullReference(e, "e");

			// TODO:  Add PhysicalUIEventHandlerNode.OnKeyDown implementation
			return false;
		}

		public bool OnKeyUp(XKeyEventArgs e)
		{
			Platform.CheckForNullReference(e, "e");

			// TODO:  Add PhysicalUIEventHandlerNode.OnKeyUp implementation
			return false;
		}

		#endregion
	}
}
