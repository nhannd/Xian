using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop
{
	/// <summary>
	/// Applies a given <see cref="IUndoableOperation{T}"/> to a collection of items
	/// and can capture and restore the state of each item affected by the operation.
	/// </summary>
	/// <remarks>
	/// It is often desirable to apply the same operation to a collection of objects
	/// with similar properties and later undo that operation.  This class encapsulates
	/// the functionality to apply an operation to a set of objects, as well as to capture
	/// and restore the state of each object affected by the operation.
	/// </remarks>
	public class UndoableOperationApplicator<T> : IMemorable where T : class
	{
		#region Memento Classes

		private class ItemMemento
		{
			public ItemMemento(T sourceItem, IMemorable originator, object memento)
			{
				Platform.CheckForNullReference(sourceItem, "sourceItem");
				Platform.CheckForNullReference(originator, "originator");
				Platform.CheckForNullReference(memento, "memento");

				this.SourceItem = sourceItem;
				this.Originator = originator;
				this.Memento = memento;
			}

			public readonly T SourceItem;
			public readonly IMemorable Originator;
			public readonly object Memento;

			public override int GetHashCode()
			{
				return base.GetHashCode();
			}

			public override bool Equals(object obj)
			{
				if (obj == null)
					return false;

				if (obj is ItemMemento)
				{
					ItemMemento other = (ItemMemento)obj;
					return other.SourceItem.Equals(SourceItem) && 
							other.Originator.Equals(Originator) &&
							other.Memento.Equals(Memento);
				}

				return false;
			}
		}

		private class MementoList
		{
			public MementoList()
			{
				ItemMementos = new List<ItemMemento>();
			}

			public readonly List<ItemMemento> ItemMementos;

			public override int GetHashCode()
			{
				return base.GetHashCode();
			}

			public override bool Equals(object obj)
			{
				if (obj == null)
					return false;

				if (obj is MementoList)
				{
					MementoList other = (MementoList)obj;
					if (other.ItemMementos.Count != ItemMementos.Count)
						return false;

					for (int i = 0; i < other.ItemMementos.Count; ++i)
					{
						if (!other.ItemMementos[i].Equals(ItemMementos[i]))
							return false;
					}

					return true;
				}

				return false;
			}
		}

		#endregion

		#region Private Fields

		private readonly IUndoableOperation<T> _operation;
		private readonly IEnumerable<T> _sourceItems;

		private event EventHandler<ItemEventArgs<T>> _appliedOperation;
		private event EventHandler<ItemEventArgs<T>> _itemMementoSet;

		#endregion

		/// <summary>
		/// Mandatory constructor.
		/// </summary>
		/// <param name="operation">The operation to apply to the items in <paramref name="sourceItems"/>.</param>
		/// <param name="sourceItems">The items to apply the <paramref name="operation"/> to.</param>
		public UndoableOperationApplicator(IUndoableOperation<T> operation, IEnumerable<T> sourceItems)
		{
			Platform.CheckForNullReference(operation, "operation");
			Platform.CheckForNullReference(sourceItems, "sourceItems");

			_operation = operation;
			_sourceItems = sourceItems;
		}

		/// <summary>
		/// Occurs when the <see cref="IUndoableOperation{T}"/> has been applied to an item.
		/// </summary>
		public event EventHandler<ItemEventArgs<T>> AppliedOperation
		{
			add { _appliedOperation += value; }
			remove { _appliedOperation -= value; }
		}

		/// <summary>
		/// Occurs when the state an item affected by the <see cref="IUndoableOperation{T}"/> has been set via a
		/// call to <see cref="SetMemento"/>.
		/// </summary>
		/// <remarks>
		/// When <see cref="SetMemento"/> is called, the state of each item is set to a previous state.
		/// </remarks>
		public event EventHandler<ItemEventArgs<T>> ItemMementoSet
		{
			add { _itemMementoSet += value; }
			remove { _itemMementoSet -= value; }
		}

		#region Public Methods

		/// <summary>
		/// Applies the <see cref="IUndoableOperation{T}"/> to each item.
		/// </summary>
		public void Apply()
		{
			foreach (T sourceItem in _sourceItems)
			{
				if (AppliesTo(sourceItem))
				{
					_operation.Apply(sourceItem);
					OnAppliedOperation(sourceItem);
				}
			}
		}

		#region IMemorable Members

		/// <summary>
		/// Uses the memento pattern to create a single memento containing the state of each item
		/// that will be affected by the <see cref="IUndoableOperation{T}"/>.
		/// </summary>
		public virtual object CreateMemento()
		{
			MementoList memento = new MementoList();
			foreach (T sourceItem in _sourceItems)
			{
				if (AppliesTo(sourceItem))
				{
					IMemorable originator = _operation.GetOriginator(sourceItem);
					object itemMemento = originator.CreateMemento();
					if (itemMemento != null)
						memento.ItemMementos.Add(new ItemMemento(sourceItem, originator, itemMemento));
				}
			}

			return memento;
		}

		/// <summary>
		/// Uses the memento pattern to restore a previous state of each item affected by the
		/// <see cref="IUndoableOperation{T}"/>.
		/// </summary>
		public virtual void SetMemento(object memento)
		{
			MementoList restoreMemento = memento as MementoList;
			Platform.CheckForNullReference(restoreMemento, "restoreMemento");

			foreach (ItemMemento itemMemento in restoreMemento.ItemMementos)
			{
				itemMemento.Originator.SetMemento(itemMemento.Memento);
				OnSetItemMemento(itemMemento.SourceItem);
			}
		}

		#endregion
		#endregion

		#region Protected Virtual Methods

		/// <summary>
		/// Called after the <see cref="IUndoableOperation{T}"/> has been applied to <paramref name="sourceItem"/>.
		/// </summary>
		protected virtual void OnAppliedOperation(T sourceItem)
		{
			EventsHelper.Fire(_appliedOperation, this, new ItemEventArgs<T>(sourceItem));
		}

		/// <summary>
		/// Called after <paramref name="sourceItem"/>'s previous state has been restored via a call to
		/// <see cref="SetMemento"/>.
		/// </summary>
		protected virtual void OnSetItemMemento(T sourceItem)
		{
			EventsHelper.Fire(_itemMementoSet, this, new ItemEventArgs<T>(sourceItem));
		}

		#endregion

		#region Private Methods

		private bool AppliesTo(T sourceItem)
		{
			return GetOriginator(sourceItem) != null;
		}

		private IMemorable GetOriginator(T sourceItem)
		{
			if (sourceItem == null)
				return null;

			IMemorable originator = _operation.GetOriginator(sourceItem);
			bool applies = _operation.AppliesTo(sourceItem);
			return applies ? originator : null;
		}

		#endregion
	}
}
