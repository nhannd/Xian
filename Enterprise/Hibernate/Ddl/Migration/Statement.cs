using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Hibernate.Ddl.Migration
{
    class Statement
    {
        private readonly string _sql;

        public Statement(string sql)
        {
            _sql = sql;
        }

    	public string Sql
    	{
			get { return _sql; }
    	}
    }
}
