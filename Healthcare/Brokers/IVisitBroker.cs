#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion


using System.Collections.Generic;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Healthcare.Brokers
{
	public partial interface IVisitBroker
	{
		IList<Visit> FindByVisitPractitioner(VisitSearchCriteria visitSearchCriteria,
											 VisitPractitionerSearchCriteria practitionerSearchCriteria);
		IList<Visit> FindByVisitPractitioner(VisitSearchCriteria visitSearchCriteria,
											 VisitPractitionerSearchCriteria practitionerSearchCriteria, SearchResultPage page);

		long CountByVisitPractitioner(VisitSearchCriteria visitSearchCriteria,
									  VisitPractitionerSearchCriteria practitionerSearchCriteria);
	}
}
