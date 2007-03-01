using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Enterprise.Core
{
    public abstract class CoreServiceLayer : ICoreServiceLayer
    {
        protected IPersistenceContext CurrentContext
        {
            get { return PersistenceScope.Current; }
        }
    }
}
