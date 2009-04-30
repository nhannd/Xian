#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System.Collections.Generic;
using ClearCanvas.Healthcare.Workflow.Reporting;

namespace ClearCanvas.Healthcare.Brokers
{
    /// <summary>
    /// Defines an interface to a worklist item broker for registration worklist items.
    /// </summary>
    public interface IReportingWorklistItemBroker : IWorklistItemBroker<WorklistItem>
    {
        /// <summary>
        /// Maps the specified set of reporting steps to a corresponding set of reporting worklist items.
        /// </summary>
        /// <param name="reportingSteps"></param>
        /// <returns></returns>
        IList<WorklistItem> GetWorklistItems(IEnumerable<ReportingProcedureStep> reportingSteps);

        /// <summary>
        /// Maps the specified set of protocolling steps to a corresponding set of reporting worklist items.
        /// </summary>
        /// <param name="protocollingSteps"></param>
        /// <returns></returns>
        IList<WorklistItem> GetWorklistItems(IEnumerable<ProtocolProcedureStep> protocollingSteps);

        /// <summary>
        /// Obtains a set of interpretation steps that are candidates for linked reporting to the specified interpretation step.
        /// </summary>
        /// <param name="step"></param>
        /// <param name="interpreter"></param>
        /// <returns></returns>
        IList<InterpretationStep> GetLinkedInterpretationCandidates(InterpretationStep step, Staff interpreter);

        /// <summary>
        /// Obtains a set of protocol steps that are candidates for linked protocolling to the specified assignment step.
        /// </summary>
        /// <param name="step"></param>
        /// <param name="author"></param>
        /// <returns></returns>
        IList<ProtocolAssignmentStep> GetLinkedProtocolCandidates(ProtocolAssignmentStep step, Staff author);
    }
}
