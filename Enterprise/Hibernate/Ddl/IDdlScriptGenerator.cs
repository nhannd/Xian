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
using NHibernate.Cfg;
using NHibernate.Dialect;

namespace ClearCanvas.Enterprise.Hibernate.Ddl
{
    /// <summary>
    /// Defines an interface for generating DDL scripts to create, upgrade or drop a relational database.
    /// </summary>
    public interface IDdlScriptGenerator
    {
        /// <summary>
        /// Returns a set of scripts that will be executed as part of creating the database.  The scripts
        /// will be executed in the order they are returned.
        /// </summary>
        /// <param name="config">The persistent store (database) that DDL should be generated for</param>
        /// <returns>A set of scripts</returns>
        string[] GenerateCreateScripts(Configuration config);

		/// <summary>
		/// Returns a set of scripts that will be executed to upgrade the database from a previous version.  The scripts
		/// will be executed in the order they are returned.
		/// </summary>
		/// <param name="config"></param>
		/// <param name="baselineModel"></param>
		/// <returns></returns>
    	string[] GenerateUpgradeScripts(Configuration config, RelationalModelInfo baselineModel);

        /// <summary>
        /// Returns a set of scripts that will be executed as part of dropping the database.  The scripts
        /// will be executed in the order they are returned.
        /// </summary>
        /// <param name="config">The persistent store (database) that DDL should be generated for</param>
        /// <returns>A set of scripts</returns>
        string[] GenerateDropScripts(Configuration config);
    }
}
