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
    class AddForeignKeyChange : RelationalModelChange
    {
    	private readonly ForeignKeyInfo _fk;

        public AddForeignKeyChange(TableInfo table, ForeignKeyInfo fk)
			: base(table)
		{
        	_fk = fk;
        }

    	public ForeignKeyInfo ForeignKey
    	{
			get { return _fk; }
    	}

		public override Statement[] GetStatements(IRenderer renderer)
		{
			return renderer.Render(this);
		}
	}
}
