#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare.Hibernate.Brokers.QueryBuilders;

namespace ClearCanvas.Healthcare.Owls.Hibernate.Brokers
{
	/// <summary>
	/// Abstract intermediate class for view source brokers that return subclasses of <see cref="ProcedureStepWorklistViewItem"/>.
	/// </summary>
	/// <typeparam name="TViewItem"></typeparam>
	public abstract class ProcedureStepViewSourceBroker<TViewItem> : ViewSourceBrokerBase<TViewItem, ProcedureStep>
		where TViewItem : ProcedureStepWorklistViewItem, new()
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="queryBuilder"></param>
		/// <param name="viewSourceProjection"></param>
		protected ProcedureStepViewSourceBroker(IQueryBuilder queryBuilder, WorklistItemProjection viewSourceProjection)
			:base(queryBuilder, viewSourceProjection)
		{
		}

		/// <summary>
		/// Gets the sub-criteria corresponding to the root entity.
		/// </summary>
		/// <param name="criteria"></param>
		/// <returns></returns>
		protected override EntitySearchCriteria GetRootEntitySubCriteria(WorklistItemSearchCriteria criteria)
		{
			// the root entity is the procedure step
			return criteria.ProcedureStep;
		}

		/// <summary>
		/// Returns true if the view contains a PatientProfile and Procedure such that performing facility is 
		/// of the same information authority as the patient profile.
		/// </summary>
		/// <param name="viewItem"></param>
		/// <returns></returns>
		protected override bool IsPatientProfileAligned(TViewItem viewItem)
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
			return new TViewItem().GetTupleMapping(projection);
		}
	}
}
