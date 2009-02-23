using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Hibernate.Ddl.Model;

namespace ClearCanvas.Enterprise.Hibernate.Ddl.Migration
{
    class ColumnPropertiesChange : Change
    {
    	private readonly ColumnInfo _column;

		public ColumnPropertiesChange(TableInfo table, ColumnInfo initial, ColumnInfo desired)
			: base(table)
		{
			_column = initial;
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
