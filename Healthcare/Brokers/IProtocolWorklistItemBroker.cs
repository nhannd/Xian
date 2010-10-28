#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare.Workflow.Reporting;

namespace ClearCanvas.Healthcare.Brokers
{
    /// <summary>
    /// Defines an interface to a worklist item broker for protocol worklist items.
    /// </summary>
    public interface IProtocolWorklistItemBroker : IWorklistItemBroker
    {
		/// <summary>
		/// Maps the specified set of protocolling steps to a corresponding set of reporting worklist items.
		/// </summary>
		/// <param name="protocollingSteps"></param>
		/// <returns></returns>
		IList<ReportingWorklistItem> GetWorklistItems(IEnumerable<ProtocolProcedureStep> protocollingSteps, WorklistItemField timeField);

		/// <summary>
		/// Obtains a set of protocol steps that are candidates for linked protocolling to the specified assignment step.
		/// </summary>
		/// <param name="step"></param>
		/// <param name="author"></param>
		/// <returns></returns>
		IList<ProtocolAssignmentStep> GetLinkedProtocolCandidates(ProtocolAssignmentStep step, Staff author);
	}
}
