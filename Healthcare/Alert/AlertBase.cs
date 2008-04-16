using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Healthcare.Alert
{
    public abstract class AlertBase<TEntity> : IAlert<TEntity>
    {
        #region IAlert<TEntity> Members

        public abstract AlertNotification Test(TEntity entity, IPersistenceContext context);

        #endregion
    }
}
