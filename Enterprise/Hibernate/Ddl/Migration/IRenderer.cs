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

namespace ClearCanvas.Enterprise.Hibernate.Ddl.Migration
{
	/// <summary>
	/// Defines an interface to an object that renders instances of <see cref="RelationalModelChange"/>
	/// to SQL statements for a given RDBMS/dialect.
	/// </summary>
	interface IRenderer
	{
		/// <summary>
		/// Allows the renderer to add or remove changes from the change set prior to rendering.
		/// This is necessary because, depending on the DDL dialect, some changes may be redundant,
		/// and can be removed, or other changes may need to be added.
		/// </summary>
		/// <param name="changes"></param>
		/// <returns></returns>
        IEnumerable<RelationalModelChange> PreFilter(IEnumerable<RelationalModelChange> changes);

		Statement[] Render(AddTableChange change);
		Statement[] Render(DropTableChange change);
		Statement[] Render(AddColumnChange change);
		Statement[] Render(DropColumnChange change);
		Statement[] Render(AddIndexChange change);
		Statement[] Render(DropIndexChange change);
		Statement[] Render(AddPrimaryKeyChange change);
		Statement[] Render(DropPrimaryKeyChange change);
		Statement[] Render(AddForeignKeyChange change);
		Statement[] Render(DropForeignKeyChange change);
		Statement[] Render(AddUniqueConstraintChange change);
		Statement[] Render(DropUniqueConstraintChange change);
		Statement[] Render(ModifyColumnChange change);
		Statement[] Render(AddEnumValueChange change);
		Statement[] Render(DropEnumValueChange change);
	}
}