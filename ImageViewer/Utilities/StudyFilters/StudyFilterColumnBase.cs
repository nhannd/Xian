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
using System.Collections;
using System.Collections.Generic;
using ClearCanvas.Desktop.Tables;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters
{
	public abstract class StudyFilterColumnBase<T> : StudyFilterColumn, IValueIndexedColumn<string>
	{
		public override sealed object GetValue(StudyItem item)
		{
			return this.GetTypedValue(item);
		}

		public override Type GetValueType()
		{
			return typeof (T);
		}

		public abstract T GetTypedValue(StudyItem item);

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

		internal override sealed TableColumnBase<StudyItem> CreateTableColumn()
		{
			return new TableColumn<StudyItem, T>(this.Name, this.GetTypedValue);
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
					foreach (StudyItem item in base.Owner.Items)
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