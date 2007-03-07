using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Ris.Application.Services
{
    public abstract class ApplicationServiceBase : IApplicationServiceLayer
    {
        protected IPersistenceContext PersistenceContext
        {
            get { return PersistenceScope.Current; }
        }
    }
}
