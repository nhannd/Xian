using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Core
{
    public abstract class ValueObjectSearchCriteria : SearchCriteria
    {
        public ValueObjectSearchCriteria(string key)
            :base(key)
        {
        }

        public ValueObjectSearchCriteria()
        {
        }
    }
}
