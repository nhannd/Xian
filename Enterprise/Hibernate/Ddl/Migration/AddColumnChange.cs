using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Hibernate.Ddl.Model;

namespace ClearCanvas.Enterprise.Hibernate.Ddl.Migration
{
    class AddColumnChange : Change
    {
    	private readonly ColumnInfo _column;

		public AddColumnChange(TableInfo table, ColumnInfo c)
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
