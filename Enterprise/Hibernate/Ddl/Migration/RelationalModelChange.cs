#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Hibernate.Ddl.Migration
{
	/// <summary>
	/// Abstract base class for classes that represent a discrete change to a relational model.
	/// </summary>
    abstract class RelationalModelChange
    {
    	private readonly TableInfo _table;

		protected RelationalModelChange(TableInfo table)
        {
        	_table = table;
        }

    	public TableInfo Table
    	{
			get { return _table; }
    	}

		public abstract Statement[] GetStatements(IRenderer renderer);
	}
}
