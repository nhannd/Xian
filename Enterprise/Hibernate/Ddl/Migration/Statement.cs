#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

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
