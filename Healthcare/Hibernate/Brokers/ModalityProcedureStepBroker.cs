#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using ClearCanvas.Enterprise.Hibernate;
using ClearCanvas.Enterprise.Hibernate.Hql;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Common;

namespace ClearCanvas.Healthcare.Hibernate.Brokers
{
	[ExtensionOf(typeof(BrokerExtensionPoint))]
	public class ModalityProcedureStepBroker : EntityBroker<ModalityProcedureStep, ModalityProcedureStepSearchCriteria>, IModalityProcedureStepBroker
	{
		public IList<ModalityProcedureStep> Find(ModalityProcedureStepSearchCriteria mpsCriteria, ProcedureSearchCriteria procedureCriteria)
		{
			var hqlFrom = new HqlFrom(typeof(ModalityProcedureStep).Name, "mps");
			hqlFrom.Joins.Add(new HqlJoin("mps.Procedure", "rp", HqlJoinMode.Inner, true));

			var query = new HqlProjectionQuery(hqlFrom);
			query.Conditions.AddRange(HqlCondition.FromSearchCriteria("mps", mpsCriteria));
			query.Conditions.AddRange(HqlCondition.FromSearchCriteria("rp", procedureCriteria));

			return ExecuteHql<ModalityProcedureStep>(query);
		}
	}
}
