using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.ImageServer.Database
{
    public class SelectCriteria : SearchCriteria
    {
        public SelectCriteria(string key)
            :base(key)
        {
        }

        public SelectCriteria()
        {
        }
    }
}
