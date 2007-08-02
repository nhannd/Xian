using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Dialect;

namespace ClearCanvas.Enterprise.Hibernate.Ddl
{
    /// <summary>
    /// Generates scripts to create/drop the table schemas.
    /// </summary>
    class TableSchemaGenerator : IDdlScriptGenerator
    {
        #region IDdlScriptGenerator Members

        public string[] GenerateCreateScripts(PersistentStore store, NHibernate.Dialect.Dialect dialect)
        {
            return store.Configuration.GenerateSchemaCreationScript(dialect);
        }

        public string[] GenerateDropScripts(PersistentStore store, NHibernate.Dialect.Dialect dialect)
        {
            return store.Configuration.GenerateDropSchemaScript(dialect);
        }

        #endregion
    }
}
