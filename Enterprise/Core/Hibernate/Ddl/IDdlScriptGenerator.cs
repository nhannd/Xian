using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Dialect;

namespace ClearCanvas.Enterprise.Data.Hibernate.Ddl
{
    /// <summary>
    /// Defines an interface for generating DDL scripts to create or drop database objects
    /// </summary>
    public interface IDdlScriptGenerator
    {
        /// <summary>
        /// Returns a set of scripts that will be executed as part of creating the database.  The scripts
        /// will be executed in the order they are returned.
        /// </summary>
        /// <param name="store">The persistent store (database) that DDL should be generated for</param>
        /// <param name="dialect">The database dialect</param>
        /// <returns>A set of scripts</returns>
        string[] GenerateCreateScripts(PersistentStore store, Dialect dialect);

        /// <summary>
        /// Returns a set of scripts that will be executed as part of dropping the database.  The scripts
        /// will be executed in the order they are returned.
        /// </summary>
        /// <param name="store">The persistent store (database) that DDL should be generated for</param>
        /// <param name="dialect">The database dialect</param>
        /// <returns>A set of scripts</returns>
        string[] GenerateDropScripts(PersistentStore store, Dialect dialect);
    }
}
