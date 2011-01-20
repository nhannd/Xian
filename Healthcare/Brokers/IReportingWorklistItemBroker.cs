#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using ClearCanvas.Healthcare.Workflow.Reporting;

namespace ClearCanvas.Healthcare.Brokers
{
    /// <summary>
    /// Defines an interface to a worklist item broker for registration worklist items.
    /// </summary>
    public interface IReportingWorklistItemBroker : IWorklistItemBroker
    {
        /// <summary>
        /// Maps the specified set of reporting steps to a corresponding set of reporting worklist items.
        /// </summary>
        /// <param name="reportingSteps"></param>
        /// <returns></returns>
		IList<ReportingWorklistItem> GetWorklistItems(IEnumerable<ReportingProcedureStep> reportingSteps, WorklistItemField timeField);

        /// <summary>
        /// Obtains a set of interpretation steps that are candidates for linked reporting to the specified interpretation step.
        /// </summary>
        /// <param name="step"></param>
        /// <param name="interpreter"></param>
        /// <returns></returns>
        IList<InterpretationStep> GetLinkedInterpretationCandidates(InterpretationStep step, Staff interpreter);
    }
}
