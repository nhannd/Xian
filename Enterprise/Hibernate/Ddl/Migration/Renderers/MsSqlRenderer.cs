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
using ClearCanvas.Enterprise.Hibernate.Ddl.Migration;
using NHibernate.Dialect;
using ClearCanvas.Common.Utilities;
using NHibernate.Cfg;

namespace ClearCanvas.Enterprise.Hibernate.Ddl.Migration.Renderers
{
	/// <summary>
	/// Implementation of <see cref="IRenderer"/> for MS-SQL 2000 and greater.
	/// </summary>
    class MsSqlRenderer : Renderer
    {
		public MsSqlRenderer(Configuration config)
			:base(config)
		{
		}

        public override IEnumerable<RelationalModelChange> PreFilter(IEnumerable<RelationalModelChange> changes)
        {
            List<RelationalModelChange> filtered = new List<RelationalModelChange>(changes);
            foreach (RelationalModelChange change in changes)
            {
                // if a primary key is being added
                if (change is AddPrimaryKeyChange)
                {
                    // check if the table is being added, in which case
                    // the primary key will be embedded in the CREATE TABLE statement
                    if (IsTableAdded(changes, change.Table))
                        filtered.Remove(change);
                }

                // if a primary key is being dropped
                if (change is DropPrimaryKeyChange)
                {
                    // check if the table is being dropped, in which case
                    // the primary key will be dropped automatically
                    if (IsTableDropped(changes, change.Table))
                        filtered.Remove(change);
                }
            }

			return base.PreFilter(filtered);
        }

        private bool IsTableAdded(IEnumerable<RelationalModelChange> changes, TableInfo table)
        {
            return CollectionUtils.Contains(changes,
                delegate(RelationalModelChange c)
                {
                    return c is AddTableChange && Equals(c.Table, table);
                });
        }

        private bool IsTableDropped(IEnumerable<RelationalModelChange> changes, TableInfo table)
        {
            return CollectionUtils.Contains(changes,
                delegate(RelationalModelChange c)
                {
                    return c is DropTableChange && Equals(c.Table, table);
                });
        }
    }
}
