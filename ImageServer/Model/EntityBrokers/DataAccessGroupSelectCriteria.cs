#region License

// Copyright (c) 2012, ClearCanvas Inc.
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
    public partial class DataAccessGroupSelectCriteria
    {
        /// <summary>
        /// Used for EXISTS or NOT EXISTS subselects against the <see cref="ServerPartitionDataAccess"/> table.
        /// </summary>
        /// <remarks>
        /// A <see cref="ServerPartitionDataAccessSelectCriteria"/> instance is created with the subselect parameters, 
        /// and assigned to this Sub-Criteria.  Note that the link between the <see cref="ServerPartitionDataAccess"/>
        /// and <see cref="DataAccessGroup"/> tables is automatically added into the <see cref="ServerPartitionDataAccessSelectCriteria"/>
        /// instance by the broker.
        /// </remarks>
        public IRelatedEntityCondition<EntitySelectCriteria> ServerPartitionDataAccessRelatedEntityCondition
        {
            get
            {
                if (!SubCriteria.ContainsKey("ServerPartitionDataAccessRelatedEntityCondition"))
                {
                    SubCriteria["ServerPartitionDataAccessRelatedEntityCondition"] = new RelatedEntityCondition<EntitySelectCriteria>("ServerPartitionDataAccessRelatedEntityCondition", "Key", "DataAccessGroupKey");
                }
                return (IRelatedEntityCondition<EntitySelectCriteria>)SubCriteria["ServerPartitionDataAccessRelatedEntityCondition"];
            }
        }
    }
}
