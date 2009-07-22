#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.Utilities.StudyFilters.Utilities;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters
{
	[ExtensionPoint]
	public sealed class StudyFilterComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView> {}

	[AssociateView(typeof (StudyFilterComponentViewExtensionPoint))]
	public partial class StudyFilterComponent : ApplicationComponent, IStudyFilter
	{
		private event EventHandler _itemAdded;
		private event EventHandler _itemRemoved;
		private event EventHandler _filterPredicatesChanged;
		private event EventHandler _sortPredicatesChanged;
		private event EventHandler _isStaleChanged;

		private readonly Table<StudyItem> _table;
		private readonly StudyFilterColumnCollection _columns;
		private readonly StudyItemSelection _selection;
		private readonly StudyFilterSettings _settings;
		private readonly ObservableList<StudyItem> _masterList;
		private readonly SortPredicateRoot _sortPredicate;
		private readonly FilterPredicateRoot _filterPredicate;
		private StudyFilterToolContext _toolContext;
		private ToolSet _toolset;
		private IActionSet _actions;
		private bool _isStale;
		private bool _bulkOperationsMode;

		public StudyFilterComponent()
		{
			_masterList = new ObservableList<StudyItem>();
			_masterList.ItemAdded += OnMasterListItemAdded;
			_masterList.ItemChanged += OnMasterListItemAdded;
			_masterList.ItemChanging += OnMasterListItemRemoved;
			_masterList.ItemRemoved += OnMasterListItemRemoved;
			_masterList.EnableEvents = true;

			_selection = new StudyItemSelection(_masterList);
			_table = new Table<StudyItem>();
			_columns = new StudyFilterColumnCollection(this);
			_settings = StudyFilterSettings.Default;
			_sortPredicate = new SortPredicateRoot(this);
			_filterPredicate = new FilterPredicateRoot(this);
		}

		public StudyFilterComponent(string path) : this()
		{
			this.Load(path, true);
			this.Refresh(true);
		}

		public StudyFilterComponent(IEnumerable<string> paths) : this()
		{
			this.Load(paths, true);
			this.Refresh(true);
		}

		#region Misc

		/// <summary>
		/// Gets or sets a value indicating whether or not the component should operate in bulk operations mode.
		/// </summary>
		/// <remarks>
		/// The component runs special code to speed up operations involving small numbers of changing items.
		/// These optimizations can be detrimental when large numbers of items are changed in one large operation.
		/// Setting the component to run in bulk operations mode will temporarily disable these optimizations.
		/// During and after bulk operations, the displayed table will only update if <see cref="Refresh()"/>
		/// is explicitly called. If <see cref="Refresh()"/> is not called after ending bulk operations, the table
		/// will remain stale.
		/// </remarks>
		public bool BulkOperationsMode
		{
			get { return _bulkOperationsMode; }
			set { _bulkOperationsMode = value; }
		}

		public Table<StudyItem> Table
		{
			get { return _table; }
		}

		public override IActionSet ExportedActions
		{
			get { return _actions; }
		}

		public override void Start()
		{
			base.Start();

			_toolContext = new StudyFilterToolContext(this);
			_toolset = new ToolSet(new StudyFilterToolExtensionPoint(), _toolContext);
			_actions = _toolset.Actions;

			// restore columns from settings
			_columns.Deserialize(_settings.Columns);
		}

		public override void Stop()
		{
			// save columns to settings
			_settings.Columns = _columns.Serialize();

			_actions = null;
			_toolset.Dispose();
			_toolset = null;
			_toolContext = null;

			_table.Filter();

			base.Stop();
		}

		#endregion

		#region Items

		public IList<StudyItem> Items
		{
			get { return _masterList; }
		}

		public StudyItemSelection Selection
		{
			get { return _selection; }
		}

		public event EventHandler ItemAdded
		{
			add { _itemAdded += value; }
			remove { _itemAdded -= value; }
		}

		public event EventHandler ItemRemoved
		{
			add { _itemRemoved += value; }
			remove { _itemRemoved -= value; }
		}

		private void OnMasterListItemRemoved(object sender, ListEventArgs<StudyItem> e)
		{
			if (!_bulkOperationsMode)
			{
				// if the item passes the filter, then it's probably being shown in the table right now - remove it!
				if (_filterPredicate.Test(e.Item))
				{
					_table.Items.Remove(e.Item);

					// and now we don't have to flag the filtered table as stale
				}
			}
			else
			{
				this.IsStale = true;
			}

			EventsHelper.Fire(_itemRemoved, this, EventArgs.Empty);
		}

		private void OnMasterListItemAdded(object sender, ListEventArgs<StudyItem> e)
		{
			if (!_bulkOperationsMode)
			{
				// if the item passes the filter, then immediately add it to the table
				if (_filterPredicate.Test(e.Item))
				{
					_sortPredicate.Insert(_table.Items, e.Item);

					// and now we don't have to flag the filtered table as stale
				}
			}
			else
			{
				this.IsStale = true;
			}

			EventsHelper.Fire(_itemAdded, this, EventArgs.Empty);
		}

		#endregion

		#region Load Methods

		public int Load(IEnumerable<string> paths)
		{
			return this.Load(paths, true);
		}

		public int Load(IEnumerable<string> paths, bool recursive)
		{
			int count = 0;
			foreach (string path in paths)
				count += this.Load(path, recursive);
			return count;
		}

		public int Load(string path)
		{
			return this.Load(path, true);
		}

		public int Load(string path, bool recursive)
		{
			return LoadCore(path, recursive);
		}

		private int LoadCore(string path, bool recursive)
		{
			int count = 0;
			try
			{
				if (File.Exists(path))
				{
					_masterList.Add(new StudyItem(path));
					count++;
				}
				else if (Directory.Exists(path))
				{
					if (recursive)
					{
						foreach (string directory in Directory.GetDirectories(path))
							count += this.LoadCore(directory, true);
					}
					foreach (string filename in Directory.GetFiles(path))
						count += this.LoadCore(filename, false);
				}
			}
			catch (DicomException)
			{
				Platform.Log(LogLevel.Info, string.Format(SR.MessageDicomException, path));
			}
			catch (IOException)
			{
				Platform.Log(LogLevel.Info, string.Format(SR.MessageInvalidPath, path));
			}
			return count;
		}

		#endregion

		#region Columns

		public IStudyFilterColumnCollection Columns
		{
			get { return _columns; }
		}

		private void ColumnInserted(int index, StudyFilterColumn newColumn)
		{
			_table.Columns.Insert(index, newColumn.CreateTableColumn());
		}

		private void ColumnRemoved(int index, StudyFilterColumn oldColumn)
		{
			_table.Columns.RemoveAt(index);
		}

		private void ColumnChanged(int index, StudyFilterColumn oldColumn, StudyFilterColumn newColumn)
		{
			_table.Columns[index] = newColumn.CreateTableColumn();
		}

		private void ColumnsChanged(IEnumerable<StudyFilterColumn> newColumns)
		{
			_table.Columns.Clear();
			foreach (StudyFilterColumn column in newColumns)
				_table.Columns.Add(column.CreateTableColumn());
		}

		#endregion

		#region Tool Context Class

		private class StudyFilterToolContext : IStudyFilterToolContext
		{
			private readonly StudyFilterComponent _component;

			private event EventHandler _activeChanged;

			private StudyItem _activeItem = null;
			private StudyFilterColumn _activeColumn = null;

			public StudyFilterToolContext(StudyFilterComponent component)
			{
				_component = component;
			}

			public StudyFilterComponent Component
			{
				get { return _component; }
			}

			public IDesktopWindow DesktopWindow
			{
				get { return _component.Host.DesktopWindow; }
			}

			public StudyItem ActiveItem
			{
				get { return _activeItem; }
			}

			public StudyFilterColumn ActiveColumn
			{
				get { return _activeColumn; }
			}

			public event EventHandler ActiveChanged
			{
				add { _activeChanged += value; }
				remove { _activeChanged -= value; }
			}

			internal void SetActiveCell(StudyItem item, StudyFilterColumn column)
			{
				_activeItem = item;
				_activeColumn = column;
				EventsHelper.Fire(_activeChanged, this, EventArgs.Empty);
			}
		}

		#endregion

		#region Context Menu Support

		public ActionModelNode GetContextMenuActionModel(StudyItem item, StudyFilterColumn column)
		{
			_toolContext.SetActiveCell(item, column);
			return ActionModelRoot.CreateModel(this.GetType().FullName, StudyFilterTool.DefaultContextMenuActionSite, _actions);
		}

		#endregion

		#region Staleness/Caching Support

		public bool IsStale
		{
			get { return _isStale; }
			private set
			{
				if (_isStale != value)
				{
					_isStale = value;
					this.OnIsStaleChanged();
				}
			}
		}

		public event EventHandler IsStaleChanged
		{
			add { _isStaleChanged += value; }
			remove { _isStaleChanged -= value; }
		}

		protected virtual void OnIsStaleChanged()
		{
			EventsHelper.Fire(_isStaleChanged, this, new EventArgs());
		}

		public void Invalidate()
		{
			this.IsStale = true;
		}

		/// <summary>
		/// If the displayed data is stale, reapplies the predicates to the dataset and updates the display.
		/// </summary>
		public void Refresh()
		{
			this.Refresh(false);
		}

		/// <summary>
		/// Reapplies the predicates to the dataset and updates the display.
		/// </summary>
		/// <param name="force">A value indicating whether or not to perform the refresh even if the data is not stale.</param>
		public void Refresh(bool force)
		{
			if (force || this.IsStale)
			{
				try
				{
					_table.Items.Clear();

					IList<StudyItem> result = _filterPredicate.Filter(_masterList);
					_sortPredicate.Sort(result);

					_table.Items.AddRange(result);

					this.IsStale = false;
				}
				catch (Exception ex)
				{
					Platform.Log(LogLevel.Error, ex, "An unexpected error occured evaluating the table's sort and filter predicates.");
				}
			}
		}

		#endregion

		#region Filtering Support

		public IList<FilterPredicate> FilterPredicates
		{
			get { return _filterPredicate.Predicates; }
		}

		public event EventHandler FilterPredicatesChanged
		{
			add { _filterPredicatesChanged += value; }	
			remove { _filterPredicatesChanged -= value; }
		}

		private class FilterPredicateRoot
		{
			private readonly AndFilterPredicate _predicate = new AndFilterPredicate();
			private readonly StudyFilterComponent _owner;

			public FilterPredicateRoot(StudyFilterComponent owner)
			{
				_owner = owner;
				_predicate.Changed += Predicates_Changed;
			}

			public IList<FilterPredicate> Predicates
			{
				get { return _predicate.Predicates; }
			}

			/// <summary>
			/// Filters a list. O{n}
			/// </summary>
			public IList<StudyItem> Filter(IList<StudyItem> list)
			{
				IList<StudyItem> filtered = new List<StudyItem>();
				foreach (StudyItem item in list)
				{
					if (_predicate.Evaluate(item))
						filtered.Add(item);
				}
				return filtered;
			}

			/// <summary>
			/// Tests the filter on the specified item. O{1}
			/// </summary>
			public bool Test(StudyItem item)
			{
				return _predicate.Evaluate(item);
			}

			private void Predicates_Changed(object sender, EventArgs e)
			{
				_owner.IsStale = true;
				EventsHelper.Fire(_owner._filterPredicatesChanged, _owner, EventArgs.Empty);
			}
		}

		#endregion

		#region Sorting Support

		public IList<SortPredicate> SortPredicates
		{
			get { return _sortPredicate.Predicates; }
		}

		public event EventHandler SortPredicatesChanged
		{
			add { _sortPredicatesChanged += value; }
			remove { _sortPredicatesChanged -= value; }
		}

		private class SortPredicateRoot : IComparer<StudyItem>
		{
			public readonly ObservableList<SortPredicate> _predicates = new ObservableList<SortPredicate>();
			private readonly StudyFilterComponent _owner;

			public SortPredicateRoot(StudyFilterComponent owner)
			{
				_owner = owner;
				_predicates.ItemAdded += Predicates_Changed;
				_predicates.ItemChanged += Predicates_Changed;
				_predicates.ItemRemoved += Predicates_Changed;
				_predicates.EnableEvents = true;
			}

			public IList<SortPredicate> Predicates
			{
				get { return _predicates; }
			}

			/// <summary>
			/// Sorts a list in place. O{n*log(n)}
			/// </summary>
			public void Sort(IList<StudyItem> list)
			{
				MergeSort(this, list, 0, list.Count);
			}

			/// <summary>
			/// Inserts the specified item into a list. O{log(n)}
			/// </summary>
			public void Insert(IList<StudyItem> list, StudyItem item)
			{
				list.Insert(BinarySearch(this, list, item, 0, list.Count), item);
			}

			private void Predicates_Changed(object sender, ListEventArgs<SortPredicate> e)
			{
				_owner.IsStale = true;
				EventsHelper.Fire(_owner._sortPredicatesChanged, _owner, EventArgs.Empty);
			}

			int IComparer<StudyItem>.Compare(StudyItem x, StudyItem y)
			{
				if (x == y)
					return 0;
				if (x == null)
					return 1;
				if (y == null)
					return -1;

				foreach (SortPredicate predicate in this.Predicates)
				{
					int result = predicate.Compare(x, y);
					if (result != 0)
						return result;
				}
				return 0;
			}

			#region Stable Sort Implementation

			/// <summary>
			/// Performs a stable merge sort on the given <paramref name="list"/> using the given <paramref name="comparer"/>.
			/// The range of items sorted is [<paramref name="rangeStart"/>, <paramref name="rangeStop"/>).
			/// </summary>
			private static void MergeSort(IComparer<StudyItem> comparer, IList<StudyItem> list, int rangeStart, int rangeStop)
			{
				int rangeLength = rangeStop - rangeStart;
				if (rangeLength > 1)
				{
					// sort halves
					int rangeMid = rangeStart + rangeLength/2;
					MergeSort(comparer, list, rangeStart, rangeMid);
					MergeSort(comparer, list, rangeMid, rangeStop);

					// merge halves
					List<StudyItem> merged = new List<StudyItem>(rangeLength);
					int j = rangeStart;
					int k = rangeMid;

					for (int n = 0; n < rangeLength; n++)
					{
						// if left half has run out of items, add item from right half
						// else if right half has run out of items, add item from left half
						// else if the left item is before or equal to the right item, add left half item
						// else add right half item
						if (k >= rangeStop || (j < rangeMid && comparer.Compare(list[j], list[k]) <= 0))
							merged.Add(list[j++]);
						else
							merged.Add(list[k++]);
					}

					// copy merged to list
					k = rangeStart;
					foreach (StudyItem item in merged)
						list[k++] = item;
				}
			}

			#endregion

			#region Search Implementation

			/// <summary>
			/// Performs a binary search on the given <paramref name="list"/> using the given <paramref name="comparer"/>
			/// for the expected index of <paramref name="item"/>.
			/// The range of items searched is [<paramref name="rangeStart"/>, <paramref name="rangeStop"/>).
			/// </summary>
			private static int BinarySearch(IComparer<StudyItem> comparer, IList<StudyItem> list, StudyItem item, int rangeStart, int rangeStop)
			{
				int rangeLength = rangeStop - rangeStart;

				int result;
				if (rangeLength == 1)
				{
					if (comparer.Compare(item, list[rangeStart]) >= 0)
						result = rangeStop;
					else
						result = rangeStart;
				}
				else if (rangeLength == 0)
				{
					result = rangeStop;
				}
				else
				{
					rangeLength = rangeStart + rangeLength/2;
					if (comparer.Compare(item, list[rangeLength]) >= 0)
						result = BinarySearch(comparer, list, item, rangeLength, rangeStop);
					else
						result = BinarySearch(comparer, list, item, rangeStart, rangeLength);
				}
				return result;
			}

			#endregion
		}

		#endregion
	}
}