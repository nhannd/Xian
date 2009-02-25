using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Hibernate.Ddl.Migration
{
    class ModifyColumnChange : Change
    {
    	private readonly ColumnInfo _column;

		public ModifyColumnChange(TableInfo table, ColumnInfo initial, ColumnInfo desired)
			: base(table)
		{
			_column = desired;
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
