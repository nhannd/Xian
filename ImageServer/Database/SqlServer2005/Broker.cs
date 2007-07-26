using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.ImageServer.Database;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.ImageServer.Database.SqlServer2005
{
    public abstract class Broker : IPersistenceBroker
    {
        private PersistenceContext _context;

        /// <summary>
        /// Returns the persistence context associated with this broker instance.
        /// </summary>
        protected PersistenceContext Context
        {
            get { return _context; }
        }

        public void SetContext(IPersistenceContext context)
        {
            this._context = (PersistenceContext)context;
        }

    }
}
