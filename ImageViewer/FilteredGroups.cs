using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using ClearCanvas.Common;
using ClearCanvas.Common.Specifications;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer
{
	#region Simple Specifications

	internal class SimpleSpecification<T> : ISpecification where T : class
	{
		private readonly Predicate<T> _test;

		public SimpleSpecification(Predicate<T> test)
		{
			Platform.CheckForNullReference(test, "test");
			_test = test;
		}

		#region ISpecification Members

		public TestResult Test(object obj)
		{
			if (obj is T && _test(obj as T))
				return new TestResult(true);
			else
				return new TestResult(false);
		}

		#endregion
	}

	internal class SimpleSpecification : SimpleSpecification<object>
	{
		public SimpleSpecification(Predicate<object> test)
			: base(test)
		{
		}
	}

	#endregion

	#region List of Groups

	public class FilteredGroupList<T> : ObservableList<FilteredGroup<T>> where T : class
	{
		public FilteredGroupList()
		{
		}
	}

	#endregion
	
	//TODO: write more unit tests.
	#region Root Group Class

	public class FilteredGroups<T> : FilteredGroup<T> where T : class
	{
		public FilteredGroups()
			: base("Root", "All Items", new SimpleSpecification(delegate { return true; }))
		{
		}

		public FilteredGroups(string name, string label, ISpecification specification)
			: base(name, label, specification)
		{
		}

		public void Add(T item)
		{
			base.AddItem(item);
		}

		public void Add(IEnumerable<T> items)
		{
			base.AddItems(items);
		}

		public void Remove(T item)
		{
			base.RemoveItem(item);
		}

		public new void Clear()
		{
			base.Clear();
		}
	}

	#endregion

	public class FilteredGroup<T> where T : class 
	{
		private FilteredGroup<T> _parentGroup;
		private readonly string _name;
		private readonly string _label;
		private readonly ISpecification _specification;
		private readonly ObservableList<T> _items;
		private readonly ReadOnlyCollection<T> _readOnlyItems;
		private readonly FilteredGroupList<T> _childGroups;

		//purposely not ListEventArgs
		private event EventHandler<ItemEventArgs<T>> _itemAdded;
		private event EventHandler<ItemEventArgs<T>> _itemRemoved;

		public FilteredGroup(string name, string label, Predicate<T> test)
			: this(name, label, new SimpleSpecification<T>(test))
		{
		}

		public FilteredGroup(string name, string label, ISpecification specification)
		{
			Platform.CheckForNullReference(specification, "specification");

			_name = name;
			_label = label;
			_specification = specification;

			_childGroups = new FilteredGroupList<T>();
			_items = new ObservableList<T>();
			_readOnlyItems = new ReadOnlyCollection<T>(_items);

			_childGroups.ItemAdded += OnChildGroupAdded;
			_childGroups.ItemChanging += OnChildGroupChanging;
			_childGroups.ItemChanged += OnChildGroupChanged;
			_childGroups.ItemRemoved += OnChildGroupRemoved;

			_items.ItemAdded += OnItemAdded;
			_items.ItemRemoved += OnItemRemoved;
		}

		#region Public Events

		public event EventHandler<ItemEventArgs<T>> ItemAdded
		{
			add { _itemAdded += value; }
			remove { _itemAdded -= value; }
		}

		public event EventHandler<ItemEventArgs<T>> ItemRemoved
		{
			add { _itemRemoved += value; }
			remove { _itemRemoved -= value; }
		}

		#endregion

		#region Private Methods

		private void OnChildGroupAdded(object sender, ListEventArgs<FilteredGroup<T>> e)
		{
			e.Item.ParentGroup = this;
		}

		private void OnChildGroupChanging(object sender, ListEventArgs<FilteredGroup<T>> e)
		{
			e.Item.ParentGroup = null;
		}

		private void OnChildGroupChanged(object sender, ListEventArgs<FilteredGroup<T>> e)
		{
			e.Item.ParentGroup = this;
		}

		private void OnChildGroupRemoved(object sender, ListEventArgs<FilteredGroup<T>> e)
		{
			e.Item.ParentGroup = null;
		}

		private void OnEmpty()
		{
			if (ParentGroup != null)
				ParentGroup.OnChildGroupEmpty(this);
		}

		private void OnChildGroupEmpty(FilteredGroup<T> childGroup)
		{
			bool remove;
			OnChildGroupEmpty(childGroup, out remove);
			if (remove)
				ChildGroups.Remove(childGroup);
		}

		private void OnItemAdded(object sender, ListEventArgs<T> e)
		{
			OnItemAdded(e.Item);

			if (ParentGroup != null)
				ParentGroup.OnChildItemAdded(e.Item);
		}

		private void OnItemRemoved(object sender, ListEventArgs<T> e)
		{
			OnItemRemoved(e.Item);
			
			if (ParentGroup != null)
				ParentGroup.OnChildItemRemoved(e.Item);
		}

		private void OnChildItemAdded(T item)
		{
			_items.Remove(item);
		}

		private void OnChildItemRemoved(T item)
		{
			bool found = false;
			foreach (T childItem in GetAllChildItems())
			{
				if (childItem.Equals(item))
				{
					found = true;
					break;
				}
			}

			//when it no longer exists in any children, add it back to our list.
			if (!found)
				_items.Add(item);
		}

		protected virtual void OnChildGroupEmpty(FilteredGroup<T> childGroup, out bool remove)
		{
			remove = false;
		}

		#endregion

		#region Public Properties

		public FilteredGroup<T> ParentGroup
		{
			get { return _parentGroup; }
			set
			{
				Clear();
				_parentGroup = value;
				Refresh();
			}
		}

		public string Name
		{
			get { return _name; }
		}

		public string Label
		{
			get { return _label; }
		}

		public bool HasItems
		{
			get { return _items.Count > 0; }	
		}

		public ReadOnlyCollection<T> Items
		{
			get { return _readOnlyItems; }	
		}

		public FilteredGroupList<T> ChildGroups
		{
			get { return _childGroups; }
		}

		#endregion

		#region Public Methods

		public List<T> GetItems()
		{
			return new List<T>(_items);
		}

		public List<T> GetAllItems()
		{
			List<T> items = new List<T>();
			foreach (T item in Items)
			{
				if (!items.Contains(item))
					items.Add(item);
			}

			foreach (T item in GetAllChildItems())
			{
				if (!items.Contains(item))
					items.Add(item);
			}

			return items;
		}

		public List<T> GetAllChildItems()
		{
			List<T> items = new List<T>();
			foreach (FilteredGroup<T> child in ChildGroups)
			{
				foreach (T item in child.GetAllItems())
				{
					if (!items.Contains(item))
						items.Add(item);
				}
			}
			
			return items;
		}

		public override string ToString()
		{
			return this.Label;
		}

		#endregion

		#region Protected Methods

		#region Overridable

		protected virtual void OnItemAdded(T item)
		{
			EventsHelper.Fire(_itemAdded, this, new ItemEventArgs<T>(item));
		}

		protected virtual void OnItemRemoved(T item)
		{
			EventsHelper.Fire(_itemRemoved, this, new ItemEventArgs<T>(item));
		}

		#endregion

		protected virtual void Clear()
		{
			foreach (T item in GetItems())
				RemoveItem(item);
		}

		protected virtual void Refresh()
		{
			Clear();
			if (ParentGroup != null)
				AddItems(ParentGroup.GetItems());
		}

		protected virtual void AddItems(IEnumerable<T> items)
		{
			foreach (T item in items)
				AddItem(item);
		}

		protected virtual bool AddItem(T item)
		{
			if (!_specification.Test(item).Success)
				return false;

			if (!AddItemToChildren(item))
			{
				if (!_items.Contains(item))
					_items.Add(item);

				return true;
			}

			return false;
		}

		protected virtual bool AddItemToChildren(T item)
		{
			if (!_specification.Test(item).Success)
				return false;

			bool addedToChild = false;
			foreach (FilteredGroup<T> childGroup in _childGroups)
			{
				if (childGroup.AddItem(item))
					addedToChild = true;
			}

			if (!addedToChild)
			{
				FilteredGroup<T> newGroup = CreateNewGroup(item);
				if (newGroup != null)
				{
					ChildGroups.Add(newGroup);
					addedToChild = AddItemToChild(item, newGroup);
					if (!addedToChild)
						Debug.Assert(false, "Item should be guaranteed to have been inserted.");
				}
			}

			return addedToChild;
		}

		protected virtual bool AddItemToChild(T item, FilteredGroup<T> child)
		{
			Platform.CheckTrue(ChildGroups.Contains(child), "Group is not a child of this group.");

			if (!_specification.Test(item).Success)
				return false;

			return child.AddItem(item);
		}

		protected virtual FilteredGroup<T> CreateNewGroup(T item)
		{
			return null;
		}

		protected virtual void RemoveItem(T item)
		{
			foreach (FilteredGroup<T> group in ChildGroups)
				group.RemoveItem(item);

			_items.Remove(item);
			if (GetAllItems().Count == 0)
				OnEmpty();
		}

		#endregion
	}
}