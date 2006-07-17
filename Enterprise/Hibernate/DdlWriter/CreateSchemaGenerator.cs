using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Hibernate.DdlWriter
{
    public class CreateSchemaGenerator : Generator
    {
        public override string[] Run(PersistentStore store)
        {
            return store.Configuration.GenerateSchemaCreationScript(new CustomSqlDialect());
        }
    }
}
