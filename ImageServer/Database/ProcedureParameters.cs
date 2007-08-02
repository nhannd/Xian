using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.ImageServer.Database
{
    public class ProcedureParameters : SearchCriteria
    {
        public ProcedureParameters(string key)
            : base(key)
        {
        }
    }
}
