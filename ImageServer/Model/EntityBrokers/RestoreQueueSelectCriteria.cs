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
	public partial class RestoreQueueSelectCriteria
	{
		/// <summary>
		/// Used for EXISTS or NOT EXISTS subselects against the ArchiveStudyStorage table.
		/// </summary>
		/// <remarks>
		/// A <see cref="ArchiveStudyStorageSelectCriteria"/> instance is created with the subselect parameters, 
		/// and assigned to this Sub-Criteria.  Note that the link between the <see cref="ArchiveStudyStorage"/>
		/// and <see cref="RestoreQueue"/> tables is automatically added into the <see cref="RestoreQueueSelectCriteria"/>
		/// instance by the broker.
		/// </remarks>
		public IRelatedEntityCondition<EntitySelectCriteria> ArchiveStudyStorageRelatedEntityCondition
		{
			get
			{
				if (!SubCriteria.ContainsKey("ArchiveStudyStorageRelatedEntityCondition"))
				{
					SubCriteria["ArchiveStudyStorageRelatedEntityCondition"] = new RelatedEntityCondition<EntitySelectCriteria>("ArchiveStudyStorageRelatedEntityCondition", "ArchiveStudyStorageKey", "Key");
				}
				return (IRelatedEntityCondition<EntitySelectCriteria>)SubCriteria["ArchiveStudyStorageRelatedEntityCondition"];
			}
		}
	}
}
