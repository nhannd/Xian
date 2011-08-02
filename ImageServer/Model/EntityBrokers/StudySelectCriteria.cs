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
    public partial class StudySelectCriteria
    {
        /// <summary>
        /// Used for EXISTS or NOT EXISTS subselects against the Series table.
        /// </summary>
        /// <remarks>
        /// A <see cref="SeriesSelectCriteria"/> instance is created with the subselect parameters, 
        /// and assigned to this Sub-Criteria.  Note that the link between the <see cref="Study"/>
        /// and <see cref="Series"/> tables is automatically added into the <see cref="SeriesSelectCriteria"/>
        /// instance by the broker.
        /// </remarks>
        public IRelatedEntityCondition<EntitySelectCriteria> SeriesRelatedEntityCondition
        {
            get
            {
                if (!SubCriteria.ContainsKey("SeriesRelatedEntityCondition"))
                {
                    SubCriteria["SeriesRelatedEntityCondition"] = new RelatedEntityCondition<EntitySelectCriteria>("SeriesRelatedEntityCondition", "Key", "StudyKey");
                }
                return (IRelatedEntityCondition<EntitySelectCriteria>)SubCriteria["SeriesRelatedEntityCondition"];
            }
        }

		/// <summary>
		/// Used for EXISTS or NOT EXISTS subselects against the StudyStorage table.
		/// </summary>
		/// <remarks>
		/// A <see cref="StudyStorageSelectCriteria"/> instance is created with the subselect parameters, 
		/// and assigned to this Sub-Criteria.  Note that the link between the <see cref="Study"/>
		/// and <see cref="StudyStorage"/> tables is automatically added into the <see cref="StudyStorageSelectCriteria"/>
		/// instance by the broker.
		/// </remarks>
		public IRelatedEntityCondition<EntitySelectCriteria> StudyStorageRelatedEntityCondition
		{
			get
			{
				if (!SubCriteria.ContainsKey("StudyStorageRelatedEntityCondition"))
				{
					SubCriteria["StudyStorageRelatedEntityCondition"] = new RelatedEntityCondition<EntitySelectCriteria>("StudyStorageRelatedEntityCondition", "StudyStorageKey", "Key");
				}
				return (IRelatedEntityCondition<EntitySelectCriteria>)SubCriteria["StudyStorageRelatedEntityCondition"];
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
