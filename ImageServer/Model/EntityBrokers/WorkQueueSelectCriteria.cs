#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Enterprise;

namespace ClearCanvas.ImageServer.Model.EntityBrokers
{
    public partial class WorkQueueSelectCriteria
    {
        /// <summary>
        /// Used for EXISTS or NOT EXISTS subselects against the FilesystemStudyStorage table based on StudyStorage being related.
        /// </summary>
        /// <remarks>
        /// A <see cref="FilesystemStudyStorageSelectCriteria"/> instance is created with the subselect parameters, 
        /// and assigned to this Sub-Criteria.  Note that the link between the <see cref="WorkQueue"/>
		/// and <see cref="FilesystemStudyStorage"/> tables is automatically added into the <see cref="WorkQueueSelectCriteria"/>
        /// instance by the broker.
        /// </remarks>
        public IRelatedEntityCondition<EntitySelectCriteria> FilesystemStudyStorageRelatedEntityCondition
        {
            get
            {
                if (!SubCriteria.ContainsKey("StudyStorageRelatedEntityCondition"))
                {
                    SubCriteria["StudyStorageRelatedEntityCondition"] = new RelatedEntityCondition<EntitySelectCriteria>("StudyStorageRelatedEntityCondition",
                        "StudyStorageKey", "StudyStorageKey");
                }
                return (IRelatedEntityCondition<EntitySelectCriteria>)SubCriteria["StudyStorageRelatedEntityCondition"];
            }
        }

		/// <summary>
		/// Used for EXISTS or NOT EXISTS subselects against the WorkQueueUID table.
		/// </summary>
		/// <remarks>
		/// A <see cref="WorkQueueUidSelectCriteria"/> instance is created with the subselect parameters, 
		/// and assigned to this Sub-Criteria.  Note that the link between the <see cref="WorkQueue"/>
		/// and <see cref="WorkQueueUid"/> tables is automatically added into the <see cref="WorkQueueSelectCriteria"/>
		/// instance by the broker.
		/// </remarks>
		public IRelatedEntityCondition<EntitySelectCriteria> WorkQueueUidRelatedEntityCondition
		{
			get
			{
				if (!SubCriteria.ContainsKey("WorkQueueUidRelatedEntityCondition"))
				{
					SubCriteria["WorkQueueUidRelatedEntityCondition"] = new RelatedEntityCondition<EntitySelectCriteria>("WorkQueueUidRelatedEntityCondition",
						"Key", "WorkQueueKey");
				}
				return (IRelatedEntityCondition<EntitySelectCriteria>)SubCriteria["WorkQueueUidRelatedEntityCondition"];
			}
		}
    }
}
