#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Healthcare.Hibernate.Brokers
{
	public partial class VisitBroker
	{
		public IList<Visit> FindByPractitioner(ExternalPractitioner practitioner)
		{
			var q = this.GetNamedHqlQuery("visitsForPractitioner");
			q.SetParameter(0, practitioner);
			return CollectionUtils.Unique(q.List<Visit>());
		}
	}
}
