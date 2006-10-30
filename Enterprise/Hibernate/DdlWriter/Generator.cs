using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Hibernate.DdlWriter
{
    public abstract class Generator
    {
        public Generator(string databaseType)
        {
            if (databaseType == "SQLite")
            {
                _dialect = new NHibernate.Dialect.SQLiteDialect();
            }
            else
            {
                _dialect = new CustomSqlDialect();
            }
        }

        public abstract string[] Run(PersistentStore store);

        protected NHibernate.Dialect.Dialect _dialect = null;
    }
}
