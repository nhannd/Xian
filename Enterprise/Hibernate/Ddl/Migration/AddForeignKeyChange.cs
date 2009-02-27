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
