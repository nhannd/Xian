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
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Common.Utilities;
using Iesi.Collections;
using NHibernate.Cfg;
using NHibernate.Mapping;
using NHibernate.Dialect;

namespace ClearCanvas.Enterprise.Hibernate.Ddl
{
	/// <summary>
	/// Describes a relational database model.
	/// </summary>
    [DataContract]
    public class RelationalModelInfo : ElementInfo
    {
    	private List<TableInfo> _tables;
    	private List<EnumerationInfo> _enumerations;

		/// <summary>
		/// Constructor that creates an empty model.
		/// </summary>
        public RelationalModelInfo()
        {
			_tables = new List<TableInfo>();
			_enumerations = new List<EnumerationInfo>();
        }

		/// <summary>
		/// Constructor that creates a model from all NHibernate mappings and embedded enumeration information
		/// in the set of installed plugins.
		/// </summary>
		/// <param name="config"></param>
		public RelationalModelInfo(Configuration config)
		{
			_tables = CollectionUtils.Map<Table, TableInfo>(GetTables(config),
							delegate(Table table) { return BuildTableInfo(table, config); });

			_enumerations = new EnumMetadataReader().GetEnums(config);
		}

		/// <summary>
		/// Gets the set of tables.
		/// </summary>
    	[DataMember]
    	public List<TableInfo> Tables
    	{
			get { return _tables; }

			// for de-serialization
			private set { _tables = value; }
    	}

		/// <summary>
		/// Gets the set of enumerations.
		/// </summary>
		[DataMember]
		public List<EnumerationInfo> Enumerations
    	{
			get { return _enumerations; }
			private set { _enumerations = value; }
    	}

		/// <summary>
		/// Gets the table that matches the specified (unqualified) name, or null if no match.
		/// </summary>
		/// <param name="table"></param>
		/// <returns></returns>
		public TableInfo GetTable(string table)
		{
			return CollectionUtils.SelectFirst(_tables, delegate(TableInfo t) { return t.Name == table; });
		}

		/// <summary>
		/// Gets the unique identity of the element.
		/// </summary>
		/// <remarks>
		/// The identity string must uniquely identify the element within a given set of elements, but need not be globally unique.
		/// </remarks>
		public override string Identity
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

		#region Helpers

		/// <summary>
		/// Gets the set of NHibernate <see cref="Table"/> objects known to the specified configuration.
		/// </summary>
		/// <param name="cfg"></param>
		/// <returns></returns>
		private static List<Table> GetTables(Configuration cfg)
		{
			// build set of all tables
			HybridSet tables = new HybridSet();
			foreach (PersistentClass pc in cfg.ClassMappings)
			{
				foreach (Table table in pc.TableClosureIterator)
				{
					tables.Add(table);
				}
			}

			foreach (Collection collection in cfg.CollectionMappings)
			{
				tables.Add(collection.CollectionTable);
			}

			return CollectionUtils.Sort<Table>(tables,
				delegate(Table x, Table y)
				{
					return x.Name.CompareTo(y.Name);
				});
		}

		/// <summary>
		/// Converts an NHibernate <see cref="Table"/> object to a <see cref="TableInfo"/> object.
		/// </summary>
		/// <param name="table"></param>
		/// <param name="config"></param>
		/// <returns></returns>
		private static TableInfo BuildTableInfo(Table table, Configuration config)
		{
			Dialect dialect = Dialect.GetDialect(config.Properties);

			// map the set of additional unique constraints (not including individual unique columns)
			List<ConstraintInfo> uniqueKeys = CollectionUtils.Map<UniqueKey, ConstraintInfo>(
				table.UniqueKeyIterator, delegate(UniqueKey uk) { return new ConstraintInfo(table, uk); });

			// explicitly model any unique columns as unique constraints
			foreach (Column col in table.ColumnIterator)
			{
				if(col.Unique)
				{
					uniqueKeys.Add(new ConstraintInfo(table, col));
				}
			}

			return new TableInfo(
				table.Name,
				table.Schema,
				CollectionUtils.Map<Column, ColumnInfo>(table.ColumnIterator, delegate(Column column) { return new ColumnInfo(column, config, dialect); }),
				new ConstraintInfo(table, table.PrimaryKey),
				CollectionUtils.Map<Index, IndexInfo>(table.IndexIterator, delegate(Index index) { return new IndexInfo(table, index); }),
				CollectionUtils.Map<ForeignKey, ForeignKeyInfo>(table.ForeignKeyIterator, delegate(ForeignKey fk) { return new ForeignKeyInfo(table, fk, config); }),
				uniqueKeys
				);
		}

		#endregion
	}
}
