#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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

using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.ReportingWorkflow
{
    /// <summary>
    /// Request object for <see cref="IReportingWorkflowService.GetPriorReports"/>.
    /// </summary>
    /// <remarks>
    /// Only one of <see cref="ReportingProcedureStepRef"/> or <see cref="ProcedureRefs"/> needs to be supplied
    /// by the caller.  If <see cref="ReportingProcedureStepRef"/> is supplied, the priors will be obtained
    /// based on all procedures attached to the report to which the reporting step refers.  Otherwise,
    /// priors will be obtained based on the set of procedures specified in <see cref="ProcedureRefs"/>.
    /// </remarks>
    [DataContract]
    public class GetPriorReportsRequest : DataContractBase
    {
        /// <summary>
        /// Constructor to request priors based on a reporting step.
        /// </summary>
        /// <param name="reportingProcedureStepRef"></param>
        public GetPriorReportsRequest(EntityRef reportingProcedureStepRef)
        {
            this.ReportingProcedureStepRef = reportingProcedureStepRef;
        }

        /// <summary>
        /// Constructor to request priors based on a set of procedures.
        /// </summary>
        /// <param name="procedureRefs"></param>
        public GetPriorReportsRequest(List<EntityRef> procedureRefs)
        {
            this.ProcedureRefs = procedureRefs;
        }

        /// <summary>
        /// A reporting step that has an associated report for which relevant priors are obtained.
        /// </summary>
        [DataMember]
        public EntityRef ReportingProcedureStepRef;

        /// <summary>
        /// A set of procedures, for which relevant priors are obtained.
        /// </summary>
        [DataMember]
        public List<EntityRef> ProcedureRefs;
    }
}
