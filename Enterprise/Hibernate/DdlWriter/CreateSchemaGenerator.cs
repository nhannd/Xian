using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Hibernate.DdlWriter
{
    public class CreateSchemaGenerator : Generator
    {
        public CreateSchemaGenerator(string databaseType) 
            : base(databaseType)
        {
        }

        public override string[] Run(PersistentStore store)
        {
            return store.Configuration.GenerateSchemaCreationScript(_dialect);
        }
    }
}
