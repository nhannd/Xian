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

using System.Runtime.Serialization;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Mapping;

namespace ClearCanvas.Enterprise.Hibernate.Ddl
{
	/// <summary>
	/// Describes a column in a relational database model.
	/// </summary>
	[DataContract]
	public class ColumnInfo : ElementInfo
	{
		private string _name;
		private int _length;
		private bool _nullable;
		private string _sqlType;

		/// <summary>
		/// Constructor.
		/// </summary>
		public ColumnInfo()
		{

		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="column"></param>
		/// <param name="config"></param>
		/// <param name="dialect"></param>
		internal ColumnInfo(Column column, Configuration config, Dialect dialect)
		{
			_name = column.Name;
			_length = column.Length;
			_nullable = column.IsNullable;
			_sqlType = column.GetSqlType(dialect, new Mapping(config));
		}

		/// <summary>
		/// Gets the column name.
		/// </summary>
		[DataMember]
		public string Name
		{
			get { return _name; }
			private set { _name = value; }
		}

		/// <summary>
		/// Gets the column length, or -1 if not applicable.
		/// </summary>
		[DataMember]
		public int Length
		{
			get { return _length; }
			private set { _length = value; }
		}

		/// <summary>
		/// Gets a value indicating whether the column is nullable.
		/// </summary>
		[DataMember]
		public bool Nullable
		{
			get { return _nullable; }
			private set { _nullable = value; }
		}

		/// <summary>
		/// Gets the SQL data type of the column.
		/// </summary>
		[DataMember]
		public string SqlType
		{
			get { return _sqlType; }
			private set { _sqlType = value; }
		}

		/// <summary>
		/// Returns true if this column matches that, property for property.
		/// </summary>
		/// <param name="that"></param>
		/// <returns></returns>
		public bool Matches(ColumnInfo that)
		{
			return this.Name == that.Name
				&& this.Length == that.Length
				&& this.Nullable == that.Nullable
				&& this.SqlType == that.SqlType;
		}

		/// <summary>
		/// Gets the unique identity of the element.
		/// </summary>
		/// <remarks>
		/// The identity string must uniquely identify the element within a given set of elements, but need not be globally unique.
		/// </remarks>
		public override string Identity
		{
			get { return this.Name; }
		}
	}
}
