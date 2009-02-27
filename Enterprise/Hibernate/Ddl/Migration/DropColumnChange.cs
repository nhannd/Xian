using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Hibernate.Ddl.Migration
{
    class DropColumnChange : RelationalModelChange
    {
    	private readonly ColumnInfo _column;

        public DropColumnChange(TableInfo table, ColumnInfo c)
			: base(table)
		{
        	_column = c;
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
