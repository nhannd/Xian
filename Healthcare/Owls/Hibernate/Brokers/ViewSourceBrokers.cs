#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Healthcare.Owls.Brokers;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Hibernate;
using ClearCanvas.Healthcare.Hibernate.Brokers.QueryBuilders;

namespace ClearCanvas.Healthcare.Owls.Hibernate.Brokers
{
	/// <summary>
	/// Implementation of <see cref="IRegistrationWorklistViewSourceBroker"/>.
	/// </summary>
	[ExtensionOf(typeof(BrokerExtensionPoint))]
	public class RegistrationWorklistViewSourceBroker : ProcedureStepViewSourceBroker<RegistrationWorklistViewItem>, IRegistrationWorklistViewSourceBroker
	{
		public RegistrationWorklistViewSourceBroker()
			: base(new WorklistItemQueryBuilder(), ViewSourceProjection.ProcedureStepBase)
		{
		}
	}

	/// <summary>
	/// Implementation of <see cref="IModalityWorklistViewSourceBroker"/>.
	/// </summary>
	[ExtensionOf(typeof(BrokerExtensionPoint))]
	public class ModalityWorklistViewSourceBroker : ProcedureStepViewSourceBroker<ModalityWorklistViewItem>, IModalityWorklistViewSourceBroker
	{
		public ModalityWorklistViewSourceBroker()
			: base(new WorklistItemQueryBuilder(), ViewSourceProjection.ProcedureStepBase)
		{
		}
	}

	/// <summary>
	/// Implementation of <see cref="IProtocolWorklistViewSourceBroker"/>.
	/// </summary>
	[ExtensionOf(typeof(BrokerExtensionPoint))]
	public class ProtocolWorklistViewSourceBroker : ProcedureStepViewSourceBroker<ProtocolWorklistViewItem>, IProtocolWorklistViewSourceBroker
	{
		public ProtocolWorklistViewSourceBroker()
			: base(new ProtocolWorklistItemQueryBuilder(), ViewSourceProjection.Protocol)
		{
		}
	}

	/// <summary>
	/// Implementation of <see cref="IReportingWorklistViewSourceBroker"/>.
	/// </summary>
	[ExtensionOf(typeof(BrokerExtensionPoint))]
	public class ReportingWorklistViewSourceBroker : ProcedureStepViewSourceBroker<ReportingWorklistViewItem>, IReportingWorklistViewSourceBroker
	{
		public ReportingWorklistViewSourceBroker()
			: base(new ReportingWorklistItemQueryBuilder(), ViewSourceProjection.Reporting)
		{
		}
	}
}
