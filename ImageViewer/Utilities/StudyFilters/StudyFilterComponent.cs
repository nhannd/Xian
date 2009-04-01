using System;
using System.Collections.Generic;
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.Common;
using ClearCanvas.ImageViewer.Utilities.StudyFilters.FilterNodes;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters
{
	[ExtensionPoint]
	public sealed class StudyFilterComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView> {}

	[AssociateView(typeof (StudyFilterComponentViewExtensionPoint))]
	public class StudyFilterComponent : ApplicationComponent, IStudyFilterColumnCollectionCallbacks
	{
		private readonly Table<StudyItem> _table;
		private readonly StudyFilterColumnCollection _columns;
		private readonly StudyItemSelection _selection;
		private readonly StudyFilterSettings _settings;
		private readonly FilteredGroups<StudyItem> _root;
		private readonly ObservableList<StudyItem> _items;
		private readonly RootPredicate _filterPredicate;
		private event EventHandler _filteredChanged;
		private FilteredGroup<StudyItem> _filter;
		private ToolSet _toolset;
		private IActionSet _actions;
		private bool _filtered;

		public StudyFilterComponent()
		{
			_items = new ObservableList<StudyItem>();
			_items.ItemAdded += OnMasterListItemAdded;
			_items.ItemChanged += OnMasterListItemAdded;
			_items.ItemChanging += OnMasterListItemRemoved;
			_items.ItemRemoved += OnMasterListItemRemoved;
			_items.EnableEvents = true;

			_selection = new StudyItemSelection(_items);
			_table = new Table<StudyItem>();
			_columns = new StudyFilterColumnCollection(this);
			_settings = StudyFilterSettings.Default;
			_filterPredicate = new RootPredicate();
			_root = new FilteredGroups<StudyItem>();
			_root.ChildGroups.Add(_filter = new FilteredGroup<StudyItem>("User", "User", _filterPredicate.Evaluate));
		}

		public StudyFilterComponent(string path) : this()
		{
			this.Load(path, true);
			this.Refresh();
		}

		public StudyFilterComponent(IEnumerable<string> paths) : this()
		{
			this.Load(paths, true);
			this.Refresh();
		}

		#region Properties

		public bool Filtered
		{
			get { return _filtered; }
			set
			{
				if (_filtered != value)
				{
					_filtered = value;
					this.OnFilteredChanged();
				}
			}
		}

		public event EventHandler FilteredChanged
		{
			add { _filteredChanged += value; }
			remove { _filteredChanged -= value; }
		}

		public Table<StudyItem> Table
		{
			get { return _table; }
		}

		public StudyFilterColumnCollection Columns
		{
			get { return _columns; }
		}

		public StudyItemSelection Selection
		{
			get { return _selection; }
		}

		public IList<FilterNodeBase> FilterPredicates
		{
			get { return _filterPredicate.Operands; }
		}

		#endregion

		#region Overrides

		public override IActionSet ExportedActions
		{
			get { return _actions; }
		}

		public override void Start()
		{
			base.Start();

			_toolset = new ToolSet(new StudyFilterToolExtensionPoint(), new StudyFilterToolContext(this));
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

			base.Stop();
		}

		protected virtual void OnFilteredChanged()
		{
			this.Refresh();
			EventsHelper.Fire(_filteredChanged, this, new EventArgs());
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
			int count = 0;
			try
			{
				if (File.Exists(path))
				{
					_items.Add(new StudyItem(path));
					count++;
				}
				else if (Directory.Exists(path))
				{
					if (recursive)
						count += this.Load(Directory.GetDirectories(path), true);
					count += this.Load(Directory.GetFiles(path), false);
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

		#region Other Public Methods

		public void Refresh()
		{
			_table.Items.Clear();

			if (_filtered)
			{
				_root.ChildGroups.Remove(_filter);
				_root.ChildGroups.Add(_filter = new FilteredGroup<StudyItem>("User", "User", _filterPredicate.Evaluate));
				_table.Items.AddRange(_filter.Items);
			}
			else
			{
				_table.Items.AddRange(_items);
			}
		}

		#endregion

		#region Private Methods

		private void OnMasterListItemRemoved(object sender, ListEventArgs<StudyItem> e)
		{
			_root.Remove(e.Item);
		}

		private void OnMasterListItemAdded(object sender, ListEventArgs<StudyItem> e)
		{
			_root.Add(e.Item);
		}

		#endregion

		#region IStudyFilterColumnCollectionCallbacks Members

		void IStudyFilterColumnCollectionCallbacks.ColumnInserted(int index, StudyFilterColumn newColumn)
		{
			_table.Columns.Insert(index, newColumn.CreateColumn());
		}

		void IStudyFilterColumnCollectionCallbacks.ColumnRemoved(int index, StudyFilterColumn oldColumn)
		{
			_table.Columns.RemoveAt(index);
		}

		void IStudyFilterColumnCollectionCallbacks.ColumnChanged(int index, StudyFilterColumn oldColumn, StudyFilterColumn newColumn)
		{
			_table.Columns[index] = newColumn.CreateColumn();
		}

		void IStudyFilterColumnCollectionCallbacks.ColumnsChanged(IEnumerable<StudyFilterColumn> newColumns)
		{
			_table.Columns.Clear();
			foreach (StudyFilterColumn column in newColumns)
				_table.Columns.Add(column.CreateColumn());
		}

		#endregion

		#region Tool Context Class

		private class StudyFilterToolContext : IStudyFilterToolContext
		{
			private readonly StudyFilterComponent _component;

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
		}

		#endregion

		#region Filter Root Predicate Class

		public sealed class RootPredicate : FilterNodeBase
		{
			private readonly IList<FilterNodeBase> _operands;

			public RootPredicate()
			{
				_operands = new List<FilterNodeBase>();
			}

			public IList<FilterNodeBase> Operands
			{
				get { return _operands; }
			}

			public override bool Evaluate(StudyItem item)
			{
				if (_operands.Count == 0)
					return true;

				foreach (FilterNodeBase operand in _operands)
				{
					if (!operand.Evaluate(item))
						return false;
				}
				return true;
			}
		}

		#endregion
	}
}