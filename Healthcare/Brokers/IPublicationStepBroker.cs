#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Enterprise.Core;
using System.Collections.Generic;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Healthcare.Brokers
{
	/// <summary>
	/// Defines the interface for a <see cref="PublicationStep"/> broker
	/// </summary>
	public interface IPublicationStepBroker : IEntityBroker<PublicationStep, PublicationStepSearchCriteria>
	{
		IList<PublicationStep> FindUnprocessedSteps(int failedItemRetryDelay, SearchResultPage page);
	}
}