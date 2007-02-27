using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise
{
    public abstract class EntitySearchCriteria : SearchCriteria
    {
        public EntitySearchCriteria(string key)
            :base(key)
        {
        }

        public EntitySearchCriteria()
        {
        }
   }
}
