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
	public partial class PatientSelectCriteria
	{
		/// <summary>
		/// Used for EXISTS or NOT EXISTS subselects against the Study table.
		/// </summary>
		/// <remarks>
		/// A <see cref="StudySelectCriteria"/> instance is created with the subselect parameters, 
		/// and assigned to this Sub-Criteria.  Note that the link between the <see cref="Patient"/>
		/// and <see cref="Study"/> tables is automatically added into the <see cref="StudySelectCriteria"/>
		/// instance by the broker.
		/// </remarks>
		public IRelatedEntityCondition<EntitySelectCriteria> StudyRelatedEntityCondition
		{
			get
			{
				if (!SubCriteria.ContainsKey("StudyRelatedEntityCondition"))
				{
					SubCriteria["StudyRelatedEntityCondition"] = new RelatedEntityCondition<EntitySelectCriteria>("StudyRelatedEntityCondition", "Key", "PatientKey");
				}
				return (IRelatedEntityCondition<EntitySelectCriteria>)SubCriteria["StudyRelatedEntityCondition"];
			}
		}
	}
}
