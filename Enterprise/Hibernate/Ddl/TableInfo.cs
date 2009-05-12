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

namespace ClearCanvas.Enterprise.Hibernate.Ddl
{
	/// <summary>
	/// Describes a table in a relational database model.
	/// </summary>
	[DataContract]
	public class TableInfo : ElementInfo
	{
		private string _name;
		private string _schema;
		private List<ColumnInfo> _columns;
		private ConstraintInfo _primaryKey;
		private List<IndexInfo> _indexes;
		private List<ForeignKeyInfo> _foreignKeys;
		private List<ConstraintInfo> _uniqueKeys;

		/// <summary>
		/// Constructor
		/// </summary>
		public TableInfo()
		{

		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="name"></param>
		/// <param name="schema"></param>
		/// <param name="columns"></param>
		/// <param name="primaryKey"></param>
		/// <param name="indexes"></param>
		/// <param name="foreignKeys"></param>
		/// <param name="uniqueKeys"></param>
		internal TableInfo(string name, string schema, List<ColumnInfo> columns, ConstraintInfo primaryKey, List<IndexInfo> indexes, List<ForeignKeyInfo> foreignKeys, List<ConstraintInfo> uniqueKeys)
		{
			Name = name;
			Schema = schema;
			Columns = columns;
			PrimaryKey = primaryKey;
			Indexes = indexes;
			ForeignKeys = foreignKeys;
			UniqueKeys = uniqueKeys;
		}

		/// <summary>
		/// Gets the name of the table.
		/// </summary>
		[DataMember]
		public string Name
		{
			get { return _name; }
			private set { _name = value; }
		}

		/// <summary>
		/// Gets the name of the schema to which the table belongs, if different from the default schema.
		/// </summary>
		[DataMember]
		public string Schema
		{
			get { return _schema; }
			private set { _schema = value; }
		}

		/// <summary>
		/// Gets the set of columns in the table.
		/// </summary>
		[DataMember]
		public List<ColumnInfo> Columns
		{
			get { return _columns; }
			private set { _columns = value; }
		}

		/// <summary>
		/// Gets the table's primary key.
		/// </summary>
		[DataMember]
		public ConstraintInfo PrimaryKey
		{
			get { return _primaryKey; }
			private set { _primaryKey = value; }
		}

		/// <summary>
		/// Gets the set of indexes defined on columns in this table.
		/// </summary>
		[DataMember]
		public List<IndexInfo> Indexes
		{
			get { return _indexes; }
			private set { _indexes = value; }
		}

		/// <summary>
		/// Gets the set of foreign key relationships defined on columns in this table.
		/// </summary>
		[DataMember]
		public List<ForeignKeyInfo> ForeignKeys
		{
			get { return _foreignKeys; }
			private set { _foreignKeys = value; }
		}

		/// <summary>
		/// Gets the set of unique keys defined on columns in this table.
		/// </summary>
		[DataMember]
		public List<ConstraintInfo> UniqueKeys
		{
			get { return _uniqueKeys; }
			private set { _uniqueKeys = value; }
		}


		/// <summary>
		/// Gets the unique identity of the element.
		/// </summary>
		/// <remarks>
		/// The identity string must uniquely identify the element within a given set of elements, but need not be globally unique.
		/// </remarks>
		public override string Identity
		{
			get { return Name; }
		}
	}
}
