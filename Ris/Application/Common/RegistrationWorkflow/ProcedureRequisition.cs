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

using System;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.RegistrationWorkflow
{
    [DataContract]
    public class ProcedureRequisition : DataContractBase
    {
        /// <summary>
        /// Constructor for use by service to return requisition back to client for editing.
        /// </summary>
        public ProcedureRequisition(
            ProcedureTypeSummary procedureType,
            string procedureIndex,
            DateTime? scheduledTime,
            FacilitySummary performingFacility,
            EnumValueInfo laterality,
            bool portableModality,
            bool checkedIn,
            EnumValueInfo status,
            bool canModify,
			bool cancelled)
        {
            this.ProcedureType = procedureType;
            this.ProcedureIndex = procedureIndex;
            this.ScheduledTime = scheduledTime;
            this.PerformingFacility = performingFacility;
            this.Laterality = laterality;
            this.PortableModality = portableModality;
            this.CheckedIn = checkedIn;
            this.Status = status;
            this.CanModify = canModify;
        	this.Cancelled = cancelled;
        }

        /// <summary>
        /// Constructor for use by client when initially creating a requisition.
        /// </summary>
        public ProcedureRequisition(ProcedureTypeSummary procedureType, FacilitySummary facility)
        {
            this.ProcedureType = procedureType;
            this.PerformingFacility = facility;
            this.CanModify = true;  // can modify a new requisition
        }


        /// <summary>
        /// The procedure type. Required.
        /// </summary>
        [DataMember]
        public ProcedureTypeSummary ProcedureType;

        /// <summary>
        /// Procedure index.  Will be set by the server if this requisition represents an existing procedure.
        /// Clients should not set or modify this field.
        /// </summary>
        [DataMember]
        public string ProcedureIndex;

        /// <summary>
        /// Time at which this procedure is scheduled to occur. May be null, indicating that
        /// the procedure is not yet scheduled for a specific time.
        /// </summary>
        [DataMember]
        public DateTime? ScheduledTime;

        /// <summary>
        /// Status of this procedure, set by the server.
        /// </summary>
        [DataMember]
        public EnumValueInfo Status;

        /// <summary>
        /// Facility at which this procedure will be performed. Required.
        /// </summary>
        [DataMember]
        public FacilitySummary PerformingFacility;

        /// <summary>
        /// Indicates whether this procedure is to be performed on a portable modality.
        /// </summary>
        [DataMember]
        public bool PortableModality;

        /// <summary>
        /// Laterality for this procedure.
        /// </summary>
        [DataMember]
        public EnumValueInfo Laterality;

        /// <summary>
        /// Set by the server to indicate whether this requested procedure can be modified
        /// during an order modification (e.g. it cannot be modified if it is already in-progress).
        /// </summary>
        [DataMember]
        public bool CanModify;

        /// <summary>
        /// Indicates if an existing procedure is checked in or not, and if a new procedure should be checked in upon creation.
        /// </summary>
        [DataMember]
        public bool CheckedIn;

		/// <summary>
		/// Set by the server if this procedure is cancelled, or by the client to indicate that the procedure should be cancelled.
		/// </summary>
		[DataMember]
		public bool Cancelled;
	}
}
