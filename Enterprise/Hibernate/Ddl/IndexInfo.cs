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
using NHibernate.Mapping;
using ClearCanvas.Common.Utilities;

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

		internal IndexInfo()
        {

        }

		internal IndexInfo(Index index)
        {
            this.Name = index.Name;
            this.Columns = CollectionUtils.Map<Column, string>(
                index.ColumnCollection,
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
