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
using NHibernate.Cfg;
using NHibernate.Mapping;

namespace ClearCanvas.Enterprise.Hibernate.Ddl
{
	/// <summary>
	/// Describes a foreign key in a relational database model.
	/// </summary>
	[DataContract]
	public class ForeignKeyInfo : ConstraintInfo
	{
		private string _referencedTable;
		private List<string> _referencedColumns;

		public ForeignKeyInfo()
		{

		}

		internal ForeignKeyInfo(ForeignKey fk, Configuration config)
			: base(fk)
		{
			//note: the fk object has a ReferencedTable property, but it doesn't always seem to be set
			//the reference class property is always set, so we use it instead to get the referenced table 
			Table table = config.GetClassMapping(fk.ReferencedEntityName).Table;
			_referencedTable = table.Name;
			_referencedColumns = CollectionUtils.Map<Column, string>(
				table.PrimaryKey.ColumnIterator,
				delegate(Column column) { return column.Name; });
		}

		/// <summary>
		/// Gets the name of the referenced (foreign) table.
		/// </summary>
		[DataMember]
		public string ReferencedTable
		{
			get { return _referencedTable; }
			private set { _referencedTable = value; }
		}

		/// <summary>
		/// Gets the names of the referenced (foreign) columns.
		/// </summary>
		[DataMember]
		public List<string> ReferencedColumns
		{
			get { return _referencedColumns; }
			private set { _referencedColumns = value; }
		}

		/// <summary>
		/// Returns true if this constraint matches that, property for property.
		/// </summary>
		/// <param name="that"></param>
		/// <returns></returns>
		public bool Matches(ForeignKeyInfo that)
		{
			return this.ReferencedTable == that.ReferencedTable
				&& CollectionUtils.Equal<string>(this.ReferencedColumns, that.ReferencedColumns, false)
				&& base.Matches(that);
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
				return this.ReferencedTable
					+ StringUtilities.Combine(CollectionUtils.Sort(this.ReferencedColumns), "")
					+ base.Identity;
			}
		}
	}
}
