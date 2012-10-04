#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

namespace ClearCanvas.Healthcare.Owls.Brokers
{
	/// <summary>
	/// Source broker for the procedure search view.
	/// </summary>
	public interface IProcedureSearchViewSourceBroker : IViewSourceBroker<ProcedureSearchViewItem, Procedure>
	{
	}

	/// <summary>
	/// Source broker for the registration worklist view.
	/// </summary>
	public interface IRegistrationWorklistViewSourceBroker : IViewSourceBroker<RegistrationWorklistViewItem, ProcedureStep>
	{
	}

	/// <summary>
	/// Source broker for the modality worklist view.
	/// </summary>
	public interface IModalityWorklistViewSourceBroker : IViewSourceBroker<ModalityWorklistViewItem, ProcedureStep>
	{
	}

	/// <summary>
	/// Source broker for the protocol worklist view.
	/// </summary>
	public interface IProtocolWorklistViewSourceBroker : IViewSourceBroker<ProtocolWorklistViewItem, ProcedureStep>
	{
	}

	/// <summary>
	/// Source broker for the reporting worklist view.
	/// </summary>
	public interface IReportingWorklistViewSourceBroker : IViewSourceBroker<ReportingWorklistViewItem, ProcedureStep>
	{
	}
}
