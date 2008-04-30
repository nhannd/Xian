using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Hibernate.Ddl
{
    public interface IDdlPreProcessor
    {
        void Process(PersistentStore store);
    }
}
