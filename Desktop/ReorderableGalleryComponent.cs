using System.Collections.Generic;
using System.ComponentModel;

namespace ClearCanvas.Desktop
{
	public class ReorderableGalleryComponent : GalleryComponent
	{
		private IList<IGalleryItem> _draggedItems;

		public ReorderableGalleryComponent() : base() {}

		public ReorderableGalleryComponent(IBindingList dataSource) : base(dataSource) {}

		public ReorderableGalleryComponent(string toolbarSite, string contextMenuSite) : base(toolbarSite, contextMenuSite) {}

		public ReorderableGalleryComponent(IBindingList dataSource, string toolbarSite, string contextMenuSite)
			: base(dataSource, toolbarSite, contextMenuSite) {}

		protected IList<IGalleryItem> DraggedItems
		{
			get { return _draggedItems; }
			set { _draggedItems = value; }
		}

		public override bool AllowsDropAtIndex
		{
			get { return true; }
		}

		protected IList<IGalleryItem> ExtractGalleryItemList(IDragDropObject dataObject)
		{
			IList<IGalleryItem> itemlist = null;
			if (dataObject.HasData<IList<IGalleryItem>>())
			{
				itemlist = dataObject.GetData<IList<IGalleryItem>>();
			}

			foreach (string format in dataObject.GetFormats())
			{
				object obj = dataObject.GetData(format);
				if (obj is IList<IGalleryItem>)
					itemlist = obj as IList<IGalleryItem>;
			}

			if (itemlist != null && itemlist.Count == 0)
				return null;
			return itemlist;
		}

		/// <summary>
		/// Signals the component that a drag &amp; drop operation involving the specified
		/// <see cref="IGalleryItem"/>s has started on the associated view.
		/// </summary>
		/// <remarks>
		/// </remarks>
		/// <param name="draggedItems">The <see cref="IGalleryItem"/>s being dragged.</param>
		public override sealed DragDropOption BeginDrag(IList<IGalleryItem> draggedItems)
		{
			_draggedItems = draggedItems;
			return DragDropOption.Move | DragDropOption.Copy;
		}

		/// <summary>
		/// Signals the component that a drag &amp; drop operation involving the specified
		/// <see cref="IGalleryItem"/>s has ended with the given action being taken on the items by the drop target.
		/// </summary>
		/// <param name="draggedItems">The <see cref="IGalleryItem"/>s that were dragged.</param>
		/// <param name="action">The <see cref="DragDropOption"/> action that was taken on the items by the drop target.</param>
		public override sealed void EndDrag(IList<IGalleryItem> draggedItems, DragDropOption action)
		{
			if (_draggedItems != null && draggedItems == _draggedItems)
			{
				if ((action & DragDropOption.Move) == DragDropOption.Move)
				{
					foreach (IGalleryItem draggedItem in draggedItems)
					{
						base.DataSource.Remove(draggedItem);
					}
				}
				_draggedItems = null;
			}
		}

		/// <summary>
		/// Checks for allowed drag &amp; drop actions involving the specified foreign data and the given target on this component.
		/// </summary>
		/// <param name="droppingData">The <see cref="IDragDropObject"/> object that encapsulates all forms of the foreign data.</param>
		/// <param name="targetIndex">The target index that the user is trying to drop at.</param>
		/// <param name="actions"></param>
		/// <param name="modifiers">The modifier keys that are being held by the user.</param>
		/// <returns>The allowed <see cref="DragDropKind"/> actions for this attempted drag &amp; drop operation.</returns>
		public override sealed DragDropOption CheckDrop(IDragDropObject droppingData, int targetIndex, DragDropOption actions, ModifierFlags modifiers)
		{
			DragDropOption allowedActions;
			IList<IGalleryItem> droppingItems = ExtractGalleryItemList(droppingData);
			if (droppingItems != null)
			{
				if (_draggedItems == droppingItems)
				{
					allowedActions = CheckDropLocalItems(droppingItems, targetIndex, actions, modifiers);
				}
				else
				{
					allowedActions = CheckDropForeignItems(droppingItems, targetIndex, actions, modifiers);
				}
			}
			else
			{
				allowedActions = CheckDropForeignObject(droppingData, targetIndex, actions, modifiers);
			}
			return actions & allowedActions;
		}

		/// <summary>
		/// Checks for allowed drag &amp; drop actions involving the specified foreign data and the given target on this component.
		/// </summary>
		/// <param name="droppingData">The <see cref="IDragDropObject"/> object that encapsulates all forms of the foreign data.</param>
		/// <param name="targetItem">The target item that the user is trying to drop on to.</param>
		/// <param name="actions"></param>
		/// <param name="modifiers">The modifier keys that are being held by the user.</param>
		/// <returns>The allowed <see cref="DragDropKind"/> action for this attempted drag &amp; drop operation.</returns>
		public override sealed DragDropOption CheckDrop(IDragDropObject droppingData, IGalleryItem targetItem, DragDropOption actions, ModifierFlags modifiers)
		{
			DragDropOption allowedActions;
			IList<IGalleryItem> droppingItems = ExtractGalleryItemList(droppingData);
			if (droppingItems != null)
			{
				if (_draggedItems == droppingItems)
				{
					allowedActions = CheckDropLocalItems(droppingItems, targetItem, actions, modifiers);
				}
				else
				{
					allowedActions = CheckDropForeignItems(droppingItems, targetItem, actions, modifiers);
				}
			}
			else
			{
				allowedActions = CheckDropForeignObject(droppingData, targetItem, actions, modifiers);
			}
			return actions & allowedActions;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <remarks>
		/// This method or <see cref="GalleryComponent.PerformDrop(IDragDropObject,IGalleryItem,DragDropOption,ModifierFlags)"/> may be called
		/// additional times if the returned action is <see cref="DragDropOption.None"/> in order to attempt other ways to drop the item in
		/// an acceptable manner. It is thus very important that the result be set properly if the drop was accepted and no further attempts
		/// should be made.
		/// </remarks>
		/// <param name="droppedData"></param>
		/// <param name="targetIndex"></param>
		/// <param name="action"></param>
		/// <param name="modifiers"></param>
		/// <returns></returns>
		public override sealed DragDropOption PerformDrop(IDragDropObject droppedData, int targetIndex, DragDropOption action, ModifierFlags modifiers)
		{
			DragDropOption performedAction;
			IList<IGalleryItem> droppedItems = ExtractGalleryItemList(droppedData);
			if (droppedItems != null)
			{
				if (_draggedItems == droppedItems)
				{
					performedAction = PerformDropLocalItems(droppedItems, targetIndex, action, modifiers);
				}
				else
				{
					performedAction = PerformDropForeignItems(droppedItems, targetIndex, action, modifiers);
				}
			}
			else
			{
				performedAction = PerformDropForeignObject(droppedData, targetIndex, action, modifiers);
			}
			return performedAction;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="droppedData"></param>
		/// <param name="targetItem"></param>
		/// <param name="action"></param>
		/// <param name="modifiers"></param>
		/// <returns></returns>
		public override sealed DragDropOption PerformDrop(IDragDropObject droppedData, IGalleryItem targetItem, DragDropOption action, ModifierFlags modifiers)
		{
			DragDropOption performedAction;
			IList<IGalleryItem> droppedItems = ExtractGalleryItemList(droppedData);
			if (droppedItems != null)
			{
				if (_draggedItems == droppedItems)
				{
					performedAction = PerformDropLocalItems(droppedItems, targetItem, action, modifiers);
				}
				else
				{
					performedAction = PerformDropForeignItems(droppedItems, targetItem, action, modifiers);
				}
			}
			else
			{
				performedAction = PerformDropForeignObject(droppedData, targetItem, action, modifiers);
			}
			return performedAction;
		}

		#region Virtual Members

		protected virtual DragDropOption CheckDropLocalItems(IList<IGalleryItem> droppingItems, int targetIndex, DragDropOption actions, ModifierFlags modifiers)
		{
			DragDropOption allowedActions = DragDropOption.None;
			if (modifiers == ModifierFlags.None) {
				// check for null drops and drops to a point within the source data
				if (droppingItems.Count == 0)
					return DragDropOption.None;

				int draggedIndex = base.DataSource.IndexOf(droppingItems[0]);
				if (targetIndex >= draggedIndex && targetIndex < draggedIndex + droppingItems.Count)
					return DragDropOption.None;

				// we are dragging an item, and the item we want to drop is the same as that which we are dragging
				// then this is a reordering operation
				allowedActions = actions & DragDropOption.Move;
			}
			return allowedActions;
		}

		protected virtual DragDropOption CheckDropForeignItems(IList<IGalleryItem> droppingItems, int targetIndex, DragDropOption actions, ModifierFlags modifiers)
		{
			return DragDropOption.None;
		}

		protected virtual DragDropOption CheckDropForeignObject(IDragDropObject droppingData, int targetIndex, DragDropOption actions, ModifierFlags modifiers)
		{
			return DragDropOption.None;
		}

		protected virtual DragDropOption CheckDropLocalItems(IList<IGalleryItem> droppingItems, IGalleryItem targetItem, DragDropOption actions, ModifierFlags modifiers)
		{
			return DragDropOption.None;
		}

		protected virtual DragDropOption CheckDropForeignItems(IList<IGalleryItem> droppingItems, IGalleryItem targetItem, DragDropOption actions, ModifierFlags modifiers)
		{
			return DragDropOption.None;
		}

		protected virtual DragDropOption CheckDropForeignObject(IDragDropObject droppingData, IGalleryItem targetItem, DragDropOption actions, ModifierFlags modifiers)
		{
			return DragDropOption.None;
		}

		protected virtual DragDropOption PerformDropLocalItems(IList<IGalleryItem> droppedItems, int targetIndex, DragDropOption actions, ModifierFlags modifiers)
		{
			DragDropOption performedAction = DragDropOption.None;
			if (modifiers == ModifierFlags.None)
			{
				// check for null drops and drops to a point within the source data
				if (droppedItems.Count == 0)
					return DragDropOption.None;

				int draggedIndex = base.DataSource.IndexOf(droppedItems[0]);
				if (targetIndex >= draggedIndex && targetIndex < draggedIndex + droppedItems.Count)
					return DragDropOption.None;

				// we are dragging something, and the item we want to drop is the same as that which we are dragging
				// then this is a reordering operation
				if (droppedItems != this.DraggedItems)
					return DragDropOption.None;

				if (draggedIndex < targetIndex)
					targetIndex -= droppedItems.Count;

				Stack<IGalleryItem> stack = new Stack<IGalleryItem>();
				foreach (IGalleryItem droppedItem in droppedItems) {
					base.DataSource.Remove(droppedItem);
					stack.Push(droppedItem);
				}
				while (stack.Count > 0) {
					base.DataSource.Insert(targetIndex, stack.Pop());
				}
				this.DraggedItems = null;

				performedAction = DragDropOption.Move;
			}
			return performedAction;
		}

		protected virtual DragDropOption PerformDropForeignItems(IList<IGalleryItem> droppedItems, int targetIndex, DragDropOption actions, ModifierFlags modifiers)
		{
			return DragDropOption.None;
		}

		protected virtual DragDropOption PerformDropForeignObject(IDragDropObject droppedItem, int targetIndex, DragDropOption actions, ModifierFlags modifiers)
		{
			return DragDropOption.None;
		}

		protected virtual DragDropOption PerformDropLocalItems(IList<IGalleryItem> droppedItems, IGalleryItem targetItem, DragDropOption actions, ModifierFlags modifiers)
		{
			return DragDropOption.None;
		}

		protected virtual DragDropOption PerformDropForeignItems(IList<IGalleryItem> droppedItems, IGalleryItem targetItem, DragDropOption actions, ModifierFlags modifiers)
		{
			return DragDropOption.None;
		}

		protected virtual DragDropOption PerformDropForeignObject(IDragDropObject droppedItem, IGalleryItem targetItem, DragDropOption actions, ModifierFlags modifiers)
		{
			return DragDropOption.None;
		}

		#endregion
	}
}