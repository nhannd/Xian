using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise
{
    public class EntityNotFoundException : PersistenceException
    {
        public EntityNotFoundException(Exception inner)
            : base("The requested entity was not found", inner)
        {
        }
    }
}
