#region License

// Copyright (c) 2011, ClearCanvas Inc.
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
	public partial class StudyIntegrityQueueSelectCriteria
	{
		/// <summary>
		/// Used for EXISTS or NOT EXISTS subselects against the StudyIntegrityQueueUID table.
		/// </summary>
		/// <remarks>
		/// A <see cref="StudyIntegrityQueueUidSelectCriteria"/> instance is created with the subselect parameters, 
		/// and assigned to this Sub-Criteria.  Note that the link between the <see cref="StudyIntegrityQueue"/>
		/// and <see cref="StudyIntegrityQueueUid"/> tables is automatically added into the <see cref="StudyIntegrityQueueSelectCriteria"/>
		/// instance by the broker.
		/// </remarks>
		public IRelatedEntityCondition<EntitySelectCriteria> StudyIntegrityQueueUidRelatedEntityCondition
		{
			get
			{
				if (!SubCriteria.ContainsKey("StudyIntegrityQueueUidRelatedEntityCondition"))
				{
					SubCriteria["StudyIntegrityQueueUidRelatedEntityCondition"] = new RelatedEntityCondition<EntitySelectCriteria>("StudyIntegrityQueueUidRelatedEntityCondition",
						"Key", "StudyIntegrityQueueKey");
				}
				return (IRelatedEntityCondition<EntitySelectCriteria>)SubCriteria["StudyIntegrityQueueUidRelatedEntityCondition"];
			}
		}

        /// <summary>
        /// Used for EXISTS or NOT EXISTS subselects against the StudyDataAccess table.
        /// </summary>
        /// <remarks>
        /// A <see cref="StudyDataAccessSelectCriteria"/> instance is created with the subselect parameters, 
        /// and assigned to this Sub-Criteria.  Note that the link between the <see cref="Study"/>
        /// and <see cref="StudyDataAccess"/> tables is automatically added into the <see cref="StudyDataAccessSelectCriteria"/>
        /// instance by the broker.
        /// </remarks>
        public IRelatedEntityCondition<EntitySelectCriteria> StudyDataAccessRelatedEntityCondition
        {
            get
            {
                if (!SubCriteria.ContainsKey("StudyDataAccessRelatedEntityCondition"))
                {
                    SubCriteria["StudyDataAccessRelatedEntityCondition"] = new RelatedEntityCondition<EntitySelectCriteria>("StudyDataAccessRelatedEntityCondition", "StudyStorageKey", "StudyStorageKey");
                }
                return (IRelatedEntityCondition<EntitySelectCriteria>)SubCriteria["StudyDataAccessRelatedEntityCondition"];
            }
        }
	}
}
