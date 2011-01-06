#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using ClearCanvas.Desktop.Tables;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters
{
	public abstract class StudyFilterColumnBase<T> : StudyFilterColumn, IValueIndexedColumn<string>
	{
		public override sealed object GetValue(IStudyItem item)
		{
			return this.GetTypedValue(item);
		}

		public override Type GetValueType()
		{
			return typeof (T);
		}

		public abstract T GetTypedValue(IStudyItem item);

		public override sealed bool Parse(string input, out object output)
		{
			T tOutput;
			bool result = this.Parse(input, out tOutput);
			output = tOutput;
			return result;
		}

		public virtual bool Parse(string input, out T output)
		{
			output = default(T);
			return false;
		}

		internal override sealed TableColumnBase<IStudyItem> CreateTableColumn()
		{
			return new TableColumn<IStudyItem, T>(this.Name, this.GetTypedValue);
		}

		#region IValueIndexedColumn<string> Members

		private List<string> _index;

		protected override void OnOwnerChanged()
		{
			_index = null;
			base.OnOwnerChanged();
		}

		protected override void OnOwnerItemAdded()
		{
			_index = null;
			base.OnOwnerItemAdded();
		}

		protected override void OnOwnerItemRemoved()
		{
			_index = null;
			base.OnOwnerItemRemoved();
		}

		private void BuildIndex()
		{
			if (_index == null)
			{
				SortedDictionary<IndexEntry, string> indexBuilder = new SortedDictionary<IndexEntry, string>();
				if (base.Owner != null)
				{
					foreach (IStudyItem item in base.Owner.Items)
					{
						IndexEntry entry = new IndexEntry(this.GetTypedValue(item), this.GetText(item));
						if (!indexBuilder.ContainsKey(entry))
							indexBuilder.Add(entry, entry.Text);

						//string v = this.GetText(item);
						//if (!_index.ContainsKey(v))
						//    _index.Add(v, 0);
					}
				}
				_index = new List<string>(indexBuilder.Values);
			}
		}

		public IEnumerable<string> UniqueValues
		{
			get
			{
				BuildIndex();
				return _index;
			}
		}

		IEnumerable IValueIndexedColumn.UniqueValues
		{
			get { return this.UniqueValues; }
		}

		private struct IndexEntry : IComparable<IndexEntry>
		{
			private readonly T Value;
			public readonly string Text;
			private readonly IComparable<T> Comparable;
			private readonly bool IsNull;

			public IndexEntry(T value, string text)
			{
				this.Value = value;
				this.Text = text;
				this.IsNull = false;
				if (!typeof (T).IsValueType)
					this.IsNull = ((object) value) == null;;
				this.Comparable = null;
				if (typeof(IComparable<T>).IsAssignableFrom(typeof(T)))
					this.Comparable = (IComparable<T>) this.Value;
			}

			public override bool Equals(object obj)
			{
				if (obj is IndexEntry)
				{
					IndexEntry other = (IndexEntry) obj;
					return this.Text.Equals(other.Text);
				}
				return false;
			}

			public override int GetHashCode()
			{
				int value = -0x0928F2C3;
				//if (!this.IsNull)
					value ^= this.Text.GetHashCode();
				return value;
			}

			public override string ToString()
			{
				return this.Text;
			}

			public int CompareTo(IndexEntry other)
			{
				if (this.Text.CompareTo(other.Text) == 0)
					return 0;

				if (this.IsNull && other.IsNull)
					return 0;
				if (this.IsNull)
					return -1;
				if (other.IsNull)
					return 1;

				if (this.Comparable == null)
					return 0;
				return this.Comparable.CompareTo(other.Value);
			}
		}

		#endregion
	}
}