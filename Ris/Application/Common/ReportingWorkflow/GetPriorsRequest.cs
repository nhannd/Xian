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

using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.ReportingWorkflow
{
    /// <summary>
    /// Request object for <see cref="IReportingWorkflowService.GetPriors"/>.
    /// </summary>
    /// <remarks>
    /// If <see cref="PatientRef"/> is supplied, all priors for the patient will be returned.
    /// The <see cref="ReportRef"/> and <see cref="OrderRef"/> values will be ignored.
    /// If <see cref="PatientRef"/> is null and the <see cref="OrderRef"/> is supplied, only priors relevant to the order for <see cref="OrderRef"/>
    /// will be returned. The <see cref="ReportRef"/> value will be ignored.
    /// If <see cref="PatientRef"/> and <see cref="OrderRef"/> are null and the <see cref="ReportRef"/> is supplied, only priors relevant to the report 
    /// for <see cref="ReportRef"/> will be returned.
    /// </remarks>
    [DataContract]
    public class GetPriorsRequest : DataContractBase
    {
        /// <summary>
        /// A report for which relevant priors are obtained.
        /// </summary>
        [DataMember]
        public EntityRef ReportRef;

        /// <summary>
        /// An order for which relevant priors are obtained.
        /// </summary>
        [DataMember]
        public EntityRef OrderRef;

        /// <summary>
        /// A patient for which all priors are obtained.
        /// </summary>
        [DataMember]
        public EntityRef PatientRef;
    }
}
