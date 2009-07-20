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
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.ImageViewer.Utilities.StudyFilters.Utilities;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters
{
	public abstract partial class StudyFilterColumn : IComparer<StudyItem>
	{
		private IStudyFilter _owner;

		protected StudyFilterColumn() {}

		public IStudyFilter Owner
		{
			get { return _owner; }
			internal set
			{
				if (_owner != value)
				{
					if (_owner != null)
					{
						_owner.ItemAdded -= Owner_ItemAdded;
						_owner.ItemRemoved -= Owner_ItemRemoved;

						// dispose filter root after disposing tools, since some tools might hold references to the filter root
						this.DisposeTools();
						this.Owner.FilterPredicates.Remove(_columnFilterRoot);
						_columnFilterRoot = null;
					}

					_owner = value;

					if (_owner != null)
					{
						_columnFilterRoot = new ColumnRootFilterPredicate();
						this.Owner.FilterPredicates.Add(_columnFilterRoot);

						_owner.ItemAdded += Owner_ItemAdded;
						_owner.ItemRemoved += Owner_ItemRemoved;
					}

					this.OnOwnerChanged();
				}
			}
		}

		public abstract string Name { get; }

		public abstract string Key { get; }

		public virtual string GetText(StudyItem item)
		{
			object value = this.GetValue(item);
			if (value == null)
				return string.Empty;
			return value.ToString();
		}

		public abstract object GetValue(StudyItem item);

		public abstract Type GetValueType();

		public virtual bool Parse(string input, out object output)
		{
			output = null;
			return false;
		}

		public virtual int Compare(StudyItem x, StudyItem y)
		{
			return 0;
		}

		public override sealed string ToString()
		{
			return this.Name;
		}

		internal abstract TableColumnBase<StudyItem> CreateTableColumn();

		#region Event Handling

		private void Owner_ItemAdded(object sender, EventArgs e)
		{
			this.OnOwnerItemAdded();
		}

		private void Owner_ItemRemoved(object sender, EventArgs e)
		{
			this.OnOwnerItemRemoved();
		}

		protected virtual void OnOwnerItemAdded() {}

		protected virtual void OnOwnerItemRemoved() {}

		protected virtual void OnOwnerChanged() {}

		#endregion

		#region Tools and Actions

		private ToolSet _tools;
		private ActionModelNode _actionModel;

		public ToolSet Tools
		{
			get
			{
				if (_tools == null)
					_tools = new ToolSet(new StudyFilterColumnToolExtensionPoint(), new ToolContext(this));

				return _tools;
			}
		}

		public ActionModelNode FilterMenuModel
		{
			get
			{
				if (_actionModel == null)
					_actionModel = ActionModelRoot.CreateModel(this.GetType().Namespace, "studyfilters-columnfilters", this.Tools.Actions);

				return _actionModel;
			}
		}

		private void DisposeTools()
		{
			// if column is removed from an owner, dispose any tools which are hanging on
			if (_actionModel != null)
				_actionModel = null;

			if (_tools != null)
			{
				_tools.Dispose();
				_tools = null;
			}
		}

		private class ToolContext : IStudyFilterColumnToolContext
		{
			private readonly StudyFilterColumn _column;

			public ToolContext(StudyFilterColumn column)
			{
				_column = column;
			}

			public StudyFilterColumn Column
			{
				get { return _column; }
			}
		}

		#endregion

		#region Column Filter

		private ColumnRootFilterPredicate _columnFilterRoot;

		public CompositeFilterPredicate ColumnFilterRoot
		{
			get { return _columnFilterRoot; }
		}

		public bool IsColumnFiltered
		{
			get { return _columnFilterRoot != null && _columnFilterRoot.Predicates.Count > 0; }
		}

		private void ClearColumnFilterRoot()
		{
			if(_columnFilterRoot != null && this.Owner != null)
			{
				
				_columnFilterRoot = null;
			}
		}

		private class ColumnRootFilterPredicate : CompositeFilterPredicate
		{
			public override bool Evaluate(StudyItem item)
			{
				foreach (FilterPredicate predicate in base.Predicates)
				{
					if (!predicate.Evaluate(item))
						return false;
				}
				return true;
			}
		}

		#endregion
	}
}