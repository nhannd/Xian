using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Hibernate.Ddl.Model;

namespace ClearCanvas.Enterprise.Hibernate.Ddl.Migration
{
    abstract class Change
    {
    	private readonly TableInfo _table;

		protected Change(TableInfo table)
        {
        	_table = table;
        }

    	public TableInfo Table
    	{
			get { return _table; }
    	}

		public abstract Statement[] GetStatements(IRenderer renderer);
	}
}
