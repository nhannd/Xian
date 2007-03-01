using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Dialect;

namespace ClearCanvas.Enterprise.Data.Hibernate.Ddl
{
    /// <summary>
    /// Generates scripts to create/drop the table schemas.
    /// </summary>
    class TableSchemaGenerator : IDdlScriptGenerator
    {
        #region IDdlScriptGenerator Members

        public string[] GenerateCreateScripts(PersistentStore store, NHibernate.Dialect.Dialect dialect)
        {
            string[] scripts = store.Configuration.GenerateSchemaCreationScript(dialect);
            if (dialect is SQLiteDialect)
            {
                AppendSemicolon(scripts);
            }
            return scripts;
        }

        public string[] GenerateDropScripts(PersistentStore store, NHibernate.Dialect.Dialect dialect)
        {
            string[] scripts = store.Configuration.GenerateDropSchemaScript(dialect);
            if (dialect is SQLiteDialect)
            {
                AppendSemicolon(scripts);
            }
            return scripts;
        }

        private void AppendSemicolon(string[] scripts)
        {
            for (int i = 0; i < scripts.Length; i++)
                scripts[i] = scripts[i] + ";";
        }

        #endregion
    }
}
