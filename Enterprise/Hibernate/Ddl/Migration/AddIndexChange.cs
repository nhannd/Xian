#region License

// Copyright (c) 2010, ClearCanvas Inc.
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
    class AddIndexChange : RelationalModelChange
    {
    	private readonly IndexInfo _index;

        public AddIndexChange(TableInfo table, IndexInfo index)
			: base(table)
        {
        	_index = index;
        }

    	public IndexInfo Index
    	{
			get { return _index; }
    	}

		public override Statement[] GetStatements(IRenderer renderer)
		{
			return renderer.Render(this);
		}
    }
}
