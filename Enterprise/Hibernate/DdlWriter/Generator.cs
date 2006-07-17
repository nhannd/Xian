using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Hibernate.DdlWriter
{
    public abstract class Generator
    {
        public abstract string[] Run(PersistentStore store);
    }
}
