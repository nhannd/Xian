#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;
using ClearCanvas.Enterprise.Hibernate;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Healthcare.Hibernate.Brokers;
using ClearCanvas.Healthcare.Owls.Hibernate.Brokers.QueryBuilders;

namespace ClearCanvas.Healthcare.Owls.Hibernate.Brokers
{
	/// <summary>
	/// Owls implementation of <see cref="IModalityWorklistItemBroker"/>.
	/// </summary>
	[ExtensionOf(typeof(BrokerExtensionPoint))]
	public class OwlsModalityWorklistItemBroker : ModalityWorklistItemBroker
	{
		public OwlsModalityWorklistItemBroker()
			:base(
				new OwlsModalityWorklistItemQueryBuilder(),
				new OwlsProcedureQueryBuilder(),
				new OwlsPatientQueryBuilder()
				)
		{
		}
	}
}
