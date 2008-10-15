using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Model.EntityBrokers;

namespace ClearCanvas.ImageServer.Model
{
    public partial class WorkQueue
    {
        /// <summary>
        /// Delete the Work Queue record from the system.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public bool Delete(IPersistenceContext context)
        {
            IWorkQueueUidEntityBroker workQueueUidBroker = context.GetBroker<IWorkQueueUidEntityBroker>();
            WorkQueueUidSelectCriteria criteria = new WorkQueueUidSelectCriteria();
            criteria.WorkQueueKey.EqualTo(this.GetKey());
            workQueueUidBroker.Delete(criteria);

            IWorkQueueEntityBroker workQueueBroker = context.GetBroker<IWorkQueueEntityBroker>();
            return workQueueBroker.Delete(this.GetKey());
        }
    }
}
