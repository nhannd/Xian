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
    class ModifyColumnChange : RelationalModelChange
    {
    	private readonly ColumnInfo _initial;
    	private readonly ColumnInfo _column;

		public ModifyColumnChange(TableInfo table, ColumnInfo initial, ColumnInfo desired)
			: base(table)
		{
			_initial = initial;
			_column = desired;
        }

    	public ColumnInfo Initial
    	{
			get { return _initial; }
    	}

    	public ColumnInfo Column
    	{
			get { return _column; }
    	}

		public override Statement[] GetStatements(IRenderer renderer)
		{
			return renderer.Render(this);
		}
	}
}
