using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Hibernate.Ddl.Migration
{
	/// <summary>
	/// Abstract base class for classes that represent a discrete change to a relational model.
	/// </summary>
    abstract class RelationalModelChange
    {
    	private readonly TableInfo _table;

		protected RelationalModelChange(TableInfo table)
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
