using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Data
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
