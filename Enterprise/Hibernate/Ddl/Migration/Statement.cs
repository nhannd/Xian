using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Hibernate.Ddl.Migration
{
	/// <summary>
	/// Represents a SQL statement.
	/// </summary>
    class Statement
    {
        private readonly string _sql;

        internal Statement(string sql)
        {
            _sql = sql;
        }

		/// <summary>
		/// Gets the SQL string.
		/// </summary>
    	public string Sql
    	{
			get { return _sql; }
    	}
    }
}
