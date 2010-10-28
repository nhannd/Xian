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
	class DropPrimaryKeyChange : RelationalModelChange
	{
		private readonly ConstraintInfo _pk;

		public DropPrimaryKeyChange(TableInfo table, ConstraintInfo pk)
			:base(table)
		{
			_pk = pk;
		}

		public ConstraintInfo PrimaryKey
		{
			get { return _pk; }
		}

		public override Statement[] GetStatements(IRenderer renderer)
		{
			return renderer.Render(this);
		}
	}
}
