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

using System.Collections.Generic;
using System.Runtime.Serialization;
using ClearCanvas.Common.Utilities;
using NHibernate.Mapping;
using System.Collections;

namespace ClearCanvas.Enterprise.Hibernate.Ddl
{
	/// <summary>
	/// Describes a constraint in a relational database model.
	/// </summary>
	[DataContract]
	public class ConstraintInfo : ElementInfo
	{
		private string _name;
		private List<string> _columns;


		public ConstraintInfo()
		{

		}

		/// <summary>
		/// Constructor for creating a constraint from a Hibnerate PrimaryKey object.
		/// </summary>
		/// <param name="table"></param>
		/// <param name="constraint"></param>
		internal ConstraintInfo(Table table, PrimaryKey constraint)
			: this("PK_", table.Name, constraint.ColumnIterator, null)
		{
		}

		/// <summary>
		/// Constructor for creating a constraint from a Hibnerate UniqueKey object.
		/// </summary>
		/// <param name="table"></param>
		/// <param name="constraint"></param>
		internal ConstraintInfo(Table table, UniqueKey constraint)
			: this("UQ_", table.Name, constraint.ColumnIterator, constraint.Name)
		{
		}

		/// <summary>
		/// Constructor for creating a unique constraint on a single column that is marked unique.
		/// </summary>
		/// <param name="table"></param>
		/// <param name="column"></param>
		internal ConstraintInfo(Table table, Column column)
			: this("UQ_", table.Name, new Column[] { column }, null)
		{
		}

		/// <summary>
		/// Constructor for creating a unique constraint on a single column that is marked unique.
		/// </summary>
		/// <param name="table"></param>
		/// <param name="column"></param>
		internal ConstraintInfo(TableInfo table, ColumnInfo column)
			: this("UQ_", table.Name, new [] { column })
		{
		}

		/// <summary>
		/// Protected constructor.
		/// </summary>
		protected ConstraintInfo(string prefix, string table, IEnumerable<Column> columns, string constraintName)
		{
			_name = string.IsNullOrEmpty(constraintName) ? MakeName(prefix, table, columns) : MakeName(prefix, table, constraintName);
			_columns = CollectionUtils.Map(columns, (Column column) => column.Name);
		}

		/// <summary>
		/// Protected constructor.
		/// </summary>
		protected ConstraintInfo(string prefix, string table, IEnumerable<ColumnInfo> columns)
		{
			_name = MakeName(prefix, table, columns);
			_columns = CollectionUtils.Map(columns, (ColumnInfo column) => column.Name);
		}

		/// <summary>
		/// Gets the name of the constraint.
		/// </summary>
		[DataMember]
		public string Name
		{
			get { return _name; }
			protected set { _name = value; }
		}

		/// <summary>
		/// Gets the names of the columns on which the constraint is defined.
		/// </summary>
		[DataMember]
		public List<string> Columns
		{
			get { return _columns; }
			protected set { _columns = value; }
		}

		/// <summary>
		/// Returns true if this constraint matches that, property for property.
		/// </summary>
		/// <param name="that"></param>
		/// <returns></returns>
		public bool Matches(ConstraintInfo that)
		{
			return this.Name == that.Name &&
				CollectionUtils.Equal<string>(this.Columns, that.Columns, false);
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
				// note that the identity is based entirely on the column names, not the name of the constraint
				// the column names are sorted because we want the identity to be independent of column ordering
				return StringUtilities.Combine(CollectionUtils.Sort(this.Columns), "");
			}
		}
	}
}
