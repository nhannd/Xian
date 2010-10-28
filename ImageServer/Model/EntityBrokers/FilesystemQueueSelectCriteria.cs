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
                if (!SubCriteria.ContainsKey("WorkQueueCondition"))
                {
                    SubCriteria["WorkQueueCondition"] = new RelatedEntityCondition<EntitySelectCriteria>("WorkQueueCondition", "StudyStorageKey", "StudyStorageKey");
                }
				return (IRelatedEntityCondition<EntitySelectCriteria>)SubCriteria["WorkQueueCondition"];
            }
        }

		/// <summary>
		/// Used for EXISTS or NOT EXISTS subselects against the FilesystemQueue table
		/// </summary>
		public IRelatedEntityCondition<EntitySelectCriteria> StudyStorage
		{
			get
			{
				if (!SubCriteria.ContainsKey("StudyStorageCondition"))
				{
					SubCriteria["StudyStorageCondition"] = new RelatedEntityCondition<EntitySelectCriteria>("WorkQueueCondition", "StudyStorageKey", "StudyStorageKey");
				}
				return (IRelatedEntityCondition<EntitySelectCriteria>)SubCriteria["StudyStorageCondition"];
			}
		}

    }
}
