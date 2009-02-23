using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Hibernate.Ddl.Model;

namespace ClearCanvas.Enterprise.Hibernate.Ddl.Migration
{
    class DropForeignKeyChange : Change
    {
    	private readonly ForeignKeyInfo _fk;

		public DropForeignKeyChange(TableInfo table, ForeignKeyInfo fk)
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
