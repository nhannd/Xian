using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Hibernate.DdlWriter
{
    public class DropSchemaGenerator : Generator
    {
        public DropSchemaGenerator(string databaseType) 
            : base(databaseType)
        {
        }

        public override string[] Run(PersistentStore store)
        {
            return store.Configuration.GenerateDropSchemaScript(_dialect);
        }
    }
}
