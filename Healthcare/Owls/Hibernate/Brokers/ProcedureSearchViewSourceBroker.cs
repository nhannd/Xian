#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Hibernate;
using ClearCanvas.Healthcare.Hibernate.Brokers.QueryBuilders;
using ClearCanvas.Healthcare.Owls.Brokers;

namespace ClearCanvas.Healthcare.Owls.Hibernate.Brokers
{
	/// <summary>
	/// Implementation of <see cref="IProcedureSearchViewSourceBroker"/>.
	/// </summary>
	[ExtensionOf(typeof(BrokerExtensionPoint))]
	public class ProcedureSearchViewSourceBroker : ViewSourceBrokerBase<ProcedureSearchViewItem, Procedure>, IProcedureSearchViewSourceBroker
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public ProcedureSearchViewSourceBroker()
			:base(new ProcedureQueryBuilder(), ViewSourceProjection.ProcedureBase)
		{
			
		}

		/// <summary>
		/// Gets the sub-criteria corresponding to the root entity.
		/// </summary>
		/// <param name="criteria"></param>
		/// <returns></returns>
		protected override EntitySearchCriteria GetRootEntitySubCriteria(WorklistItemSearchCriteria criteria)
		{
			return criteria.Procedure;
		}

		/// <summary>
		/// Returns true if the view contains a PatientProfile and Procedure such that performing facility is 
		/// of the same information authority as the patient profile.
		/// </summary>
		/// <param name="viewItem"></param>
		/// <returns></returns>
		protected override bool IsPatientProfileAligned(ProcedureSearchViewItem viewItem)
		{
			return viewItem.IsPatientProfileAligned();
		}

		/// <summary>
		/// Gets the tuple mapping function for the specified projection.
		/// </summary>
		/// <param name="projection"></param>
		/// <returns></returns>
		protected override TupleMappingDelegate GetTupleMapping(WorklistItemProjection projection)
		{
			return new ProcedureSearchViewItem().GetTupleMapping(projection);
		}
	}
}
