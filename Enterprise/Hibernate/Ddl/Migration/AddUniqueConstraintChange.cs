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
    class AddUniqueConstraintChange : RelationalModelChange
    {
    	private readonly ConstraintInfo _constraint;

		public AddUniqueConstraintChange(TableInfo table, ConstraintInfo constraint)
			: base(table)
		{
        	_constraint = constraint;
        }

    	public ConstraintInfo Constraint
    	{
			get { return _constraint; }
    	}

		public override Statement[] GetStatements(IRenderer renderer)
		{
			return renderer.Render(this);
		}
	}
}
