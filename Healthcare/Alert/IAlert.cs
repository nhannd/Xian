using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Healthcare.Alert
{
    public interface IAlert<TEntity>
    {
        string Name { get; }
        IAlertNotification Test(TEntity entity, IPersistenceContext context);
    }
}
