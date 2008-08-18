using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Enterprise;

namespace ClearCanvas.ImageServer.Model.EntityBrokers
{
    public partial class  FilesystemQueueSelectCriteria
    {
        /// <summary>
        /// Used for EXISTS or NOT EXISTS subselects against the FilesystemQueue table.
        /// </summary>
        /// <remarks>
        /// A <see cref="WorkQueueSelectCriteria"/> instance is created with the subselect parameters, 
        /// and assigned to this Sub-Criteria.  Note that the link between the <see cref="FilesystemQueue"/>
        /// and <see cref="WorkQueue"/> tables is automatically added into the <see cref="WorkQueueSelectCriteria"/>
        /// instance by the broker.
        /// </remarks>
        public IRelatedEntityCondition<EntitySelectCriteria> WorkQueue
        {
            get
            {
                if (!this.SubCriteria.ContainsKey("WorkQueueCondition"))
                {
                    this.SubCriteria["WorkQueueCondition"] = new RelatedEntityCondition<EntitySelectCriteria>("WorkQueueCondition", "StudyStorageKey", "StudyStorageKey");
                }
                return (IRelatedEntityCondition<EntitySelectCriteria>)this.SubCriteria["WorkQueueCondition"];
            }
        }
    }
}
