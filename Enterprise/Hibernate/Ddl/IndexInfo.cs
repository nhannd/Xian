#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using System.Runtime.Serialization;
using ClearCanvas.Common.Utilities;
using NHibernate.Mapping;

namespace ClearCanvas.Enterprise.Hibernate.Ddl
{
	/// <summary>
	/// Describes an index in a relational model.
	/// </summary>
	[DataContract]
	public class IndexInfo : ElementInfo
	{
		public string _name;
		public List<string> _columns;

		public IndexInfo()
		{

		}

		internal IndexInfo(Table table, Index index)
		{
			_name = MakeName("IX_", table.Name, index.ColumnIterator);
			_columns = CollectionUtils.Map<Column, string>(
				index.ColumnIterator,
				delegate(Column column) { return column.Name; });
		}

		/// <summary>
		/// Gets the name of the index.
		/// </summary>
		[DataMember]
		public string Name
		{
			get { return _name; }
			private set { _name = value; }
		}

		/// <summary>
		/// Gets the names of the columns on which the index is based, in order.
		/// </summary>
		[DataMember]
		public List<string> Columns
		{
			get { return _columns; }
			private set { _columns = value; }
		}

		/// <summary>
		/// Returns true if this index matches that, property for property.
		/// </summary>
		/// <param name="that"></param>
		/// <returns></returns>
		public bool Matches(IndexInfo that)
		{
			return this.Name == that.Name &&
				CollectionUtils.Equal<string>(this.Columns, that.Columns, true);
		}

		/// <summary>
		/// Gets the unique identity of the element.
		/// </summary>
		/// <remarks>
		/// The identity string must uniquely identify the element within a given set of elements, but need not be globally unique.
		/// </remarks>
		public override string Identity
		{
			get
			{
				// note that the identity is based entirely on the column names, not the name of the index
				// the column names are *not* sorted because we want the identity to be dependent on column ordering,
				// because the order of columns is important in an index, and multiple indexes may exist that differ
				// only in the order of the columns
				return StringUtilities.Combine(this.Columns, "");
			}
		}
	}
}
