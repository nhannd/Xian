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
	public partial interface IOrderBroker
	{
		Order FindDocumentOwner(AttachedDocument document);

		IList<Order> FindByResultRecipient(OrderSearchCriteria orderSearchCriteria, ResultRecipientSearchCriteria recipientSearchCriteria);
		IList<Order> FindByResultRecipient(OrderSearchCriteria orderSearchCriteria, ResultRecipientSearchCriteria recipientSearchCriteria, SearchResultPage page);
		long CountByResultRecipient(OrderSearchCriteria orderSearchCriteria, ResultRecipientSearchCriteria recipientSearchCriteria);
	}
}
