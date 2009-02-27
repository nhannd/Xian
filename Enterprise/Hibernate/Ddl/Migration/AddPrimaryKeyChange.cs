using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Hibernate.Ddl.Migration
{
	class AddPrimaryKeyChange : RelationalModelChange
	{
		private readonly ConstraintInfo _pk;

		public AddPrimaryKeyChange(TableInfo table, ConstraintInfo pk)
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
