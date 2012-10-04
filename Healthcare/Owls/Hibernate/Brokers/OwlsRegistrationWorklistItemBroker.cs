#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Healthcare.Owls.Hibernate.Brokers.QueryBuilders;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Hibernate;
using ClearCanvas.Healthcare.Hibernate.Brokers;

namespace ClearCanvas.Healthcare.Owls.Hibernate.Brokers
{
	/// <summary>
	/// Owls implementation of <see cref="IRegistrationWorklistItemBroker"/>.
	/// </summary>
	[ExtensionOf(typeof(BrokerExtensionPoint))]
	public class OwlsRegistrationWorklistItemBroker : RegistrationWorklistItemBroker
	{
		public OwlsRegistrationWorklistItemBroker()
			:base(
				new OwlsRegistrationWorklistItemQueryBuilder(),
				new OwlsProcedureQueryBuilder(),
				new OwlsPatientQueryBuilder()
				)
		{
		}
	}
}
