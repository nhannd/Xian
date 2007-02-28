using System;
using System.Data;

using NHibernate;
using NHibernate.SqlCommand;
using NHibernate.Dialect;

namespace ClearCanvas.Enterprise.Hibernate
{
    public class CustomSqlDialect : NHibernate.Dialect.MsSql2000Dialect
    {
        public CustomSqlDialect()
        {
            // override the NHibernate mapping of the string type to use VARCHAR instead of NVARCHAR
            // TODO: for some reason this doesn't work, so I've commented it out for now
            //RegisterColumnType(DbType.String, "VARCHAR(255)");
            //RegisterColumnType(DbType.String, 4000, "VARCHAR($1)");
        }
    }
}
