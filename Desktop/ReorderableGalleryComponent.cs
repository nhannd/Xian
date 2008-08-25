using System.ComponentModel;

namespace ClearCanvas.Desktop
{
	public class ReorderableGalleryComponent : GalleryComponent
	{
		private IGalleryItem _draggedItem;

		public ReorderableGalleryComponent() : base()
		{
		}

		public ReorderableGalleryComponent(IBindingList dataSource) : base(dataSource)
		{
		}

		public ReorderableGalleryComponent(string toolbarSite, string contextMenuSite) : base(toolbarSite, contextMenuSite)
		{
		}

		public ReorderableGalleryComponent(IBindingList dataSource, string toolbarSite, string contextMenuSite)
			: base(dataSource, toolbarSite, contextMenuSite)
		{
		}

		protected IGalleryItem DraggedItem
		{
			get { return _draggedItem; }
			set { _draggedItem = value; }
		}

		public override bool AllowsDropAtIndex
		{
			get { return true; }
		}

		protected IGalleryItem ExtractGalleryItem(IDragDropObject dataObject)
		{
			if (dataObject.HasData<IGalleryItem>())
				return dataObject.GetData<IGalleryItem>();
			foreach (string format in dataObject.GetFormats())
			{
				object obj = dataObject.GetData(format);
				if (obj is IGalleryItem)
					return obj as IGalleryItem;
			}
			return null;
		}

		/// <summary>
		/// Signals the component that a drag &amp; drop operation involving the specified
		/// <see cref="IGalleryItem"/> has started on the associated view.
		/// </summary>
		/// <remarks>
		/// </remarks>
		/// <param name="draggedItem">The <see cref="IGalleryItem"/> being dragged.</param>
		public override sealed DragDropOption BeginDrag(IGalleryItem draggedItem)
		{
			_draggedItem = draggedItem;
			return DragDropOption.Move | DragDropOption.Copy;
		}

		/// <summary>
		/// Signals the component that a drag &amp; drop operation involving the specified
		/// <see cref="IGalleryItem"/> has ended with the given action being taken on the item by the drop target.
		/// </summary>
		/// <param name="draggedItem">The <see cref="IGalleryItem"/> that was dragged.</param>
		/// <param name="action">The <see cref="DragDropOption"/> action that was taken on the item by the drop target.</param>
		public override sealed void EndDrag(IGalleryItem draggedItem, DragDropOption action)
		{
			if (_draggedItem != null)
			{
				if ((action & DragDropOption.Move) == DragDropOption.Move)
					base.DataSource.Remove(draggedItem);
				_draggedItem = null;
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
			IGalleryItem droppingItem = ExtractGalleryItem(droppingData);
			if (droppingItem != null)
			{
				if (_draggedItem == droppingItem)
				{
					allowedActions = CheckDropLocalItem(droppingItem, targetIndex, actions, modifiers);
				}
				else
				{
					allowedActions = CheckDropForeignItem(droppingItem, targetIndex, actions, modifiers);
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
			IGalleryItem droppingItem = ExtractGalleryItem(droppingData);
			if (droppingItem != null)
			{
				if (_draggedItem == droppingItem)
				{
					allowedActions = CheckDropLocalItem(droppingItem, targetItem, actions, modifiers);
				}
				else
				{
					allowedActions = CheckDropForeignItem(droppingItem, targetItem, actions, modifiers);
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
			IGalleryItem droppedItem = ExtractGalleryItem(droppedData);
			if (droppedItem != null)
			{
				if (_draggedItem == droppedItem)
				{
					performedAction = PerformDropLocalItem(droppedItem, targetIndex, action, modifiers);
				}
				else
				{
					performedAction = PerformDropForeignItem(droppedItem, targetIndex, action, modifiers);
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
			IGalleryItem droppedItem = ExtractGalleryItem(droppedData);
			if (droppedItem != null)
			{
				if (_draggedItem == droppedItem)
				{
					performedAction = PerformDropLocalItem(droppedItem, targetItem, action, modifiers);
				}
				else
				{
					performedAction = PerformDropForeignItem(droppedItem, targetItem, action, modifiers);
				}
			}
			else
			{
				performedAction = PerformDropForeignObject(droppedData, targetItem, action, modifiers);
			}
			return performedAction;
		}

		#region Virtual Members

		protected virtual DragDropOption CheckDropLocalItem(IGalleryItem droppingItem, int targetIndex, DragDropOption actions, ModifierFlags modifiers)
		{
			DragDropOption allowedActions = DragDropOption.None;
			if (modifiers == ModifierFlags.None)
			{
				// we are dragging an item, and the item we want to drop is the same as that which we are dragging
				// then this is a reordering operation
				allowedActions = actions & DragDropOption.Move;
			}
			return allowedActions;
		}

		protected virtual DragDropOption CheckDropForeignItem(IGalleryItem droppingItem, int targetIndex, DragDropOption actions, ModifierFlags modifiers)
		{
			return DragDropOption.None;
		}

		protected virtual DragDropOption CheckDropForeignObject(IDragDropObject droppingData, int targetIndex, DragDropOption actions, ModifierFlags modifiers)
		{
			return DragDropOption.None;
		}

		protected virtual DragDropOption CheckDropLocalItem(IGalleryItem droppingItem, IGalleryItem targetItem, DragDropOption actions, ModifierFlags modifiers)
		{
			return DragDropOption.None;
		}

		protected virtual DragDropOption CheckDropForeignItem(IGalleryItem droppingItem, IGalleryItem targetItem, DragDropOption actions, ModifierFlags modifiers)
		{
			return DragDropOption.None;
		}

		protected virtual DragDropOption CheckDropForeignObject(IDragDropObject droppingData, IGalleryItem targetItem, DragDropOption actions, ModifierFlags modifiers)
		{
			return DragDropOption.None;
		}

		protected virtual DragDropOption PerformDropLocalItem(IGalleryItem droppingItem, int targetIndex, DragDropOption actions, ModifierFlags modifiers)
		{
			DragDropOption performedAction = DragDropOption.None;
			if (modifiers == ModifierFlags.None)
			{
				// we are dragging something, and the item we want to drop is the same as that which we are dragging
				// then this is a reordering operation
				int draggedIndex = base.DataSource.IndexOf(droppingItem);
				if (draggedIndex < targetIndex)
					targetIndex--;
				base.DataSource.Remove(droppingItem);
				base.DataSource.Insert(targetIndex, droppingItem);
				this.DraggedItem = null;

				performedAction = DragDropOption.Move;
			}
			return performedAction;
		}

		protected virtual DragDropOption PerformDropForeignItem(IGalleryItem droppingItem, int targetIndex, DragDropOption actions, ModifierFlags modifiers)
		{
			return DragDropOption.None;
		}

		protected virtual DragDropOption PerformDropForeignObject(IDragDropObject droppingData, int targetIndex, DragDropOption actions, ModifierFlags modifiers)
		{
			return DragDropOption.None;
		}

		protected virtual DragDropOption PerformDropLocalItem(IGalleryItem droppingItem, IGalleryItem targetItem, DragDropOption actions, ModifierFlags modifiers)
		{
			return DragDropOption.None;
		}

		protected virtual DragDropOption PerformDropForeignItem(IGalleryItem droppingItem, IGalleryItem targetItem, DragDropOption actions, ModifierFlags modifiers)
		{
			return DragDropOption.None;
		}

		protected virtual DragDropOption PerformDropForeignObject(IDragDropObject droppingData, IGalleryItem targetItem, DragDropOption actions, ModifierFlags modifiers)
		{
			return DragDropOption.None;
		}

		#endregion
	}
}