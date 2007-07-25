using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.ImageServer.Database;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.ImageServer.Database.SqlServer2005
{
    public class Broker : IPersistenceBroker
    {
        protected PersistenceContext _context;

        #region IPersistenceBroker Members

        void IPersistenceBroker.SetContext(IPersistenceContext context)
        {
            this._context = (PersistenceContext)context;
        }

        #endregion
    }
}
