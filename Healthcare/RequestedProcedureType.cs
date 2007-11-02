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
using System.Collections;
using System.Text;

using Iesi.Collections;
using ClearCanvas.Enterprise.Core;
using Iesi.Collections.Generic;


namespace ClearCanvas.Healthcare {


    /// <summary>
    /// RequestedProcedureType entity
    /// </summary>
	public partial class RequestedProcedureType
	{
        public RequestedProcedureType(string id, string name)
            :this(id, name, new HashedSet<ModalityProcedureStepType>())
        {
        }

        /// <summary>
        /// Creates a new <see cref="RequestedProcedure"/> of this type, scheduled for the specified time.
        /// </summary>
        /// <returns></returns>
        public virtual RequestedProcedure CreateProcedure(DateTime? scheduledStartTime)
        {
            RequestedProcedure rp = new RequestedProcedure(this);

            // Create a check-in step - each RP only ever has one check-in step
            rp.AddProcedureStep(new CheckInProcedureStep());

            // add modality procedure steps
            foreach (ModalityProcedureStepType spt in this.ModalityProcedureStepTypes)
            {
                // sps is automatically added to rp.ProcedureSteps collection
                ModalityProcedureStep mps = new ModalityProcedureStep(rp, spt, spt.DefaultModality);
            }

            rp.Schedule(scheduledStartTime);

            return rp;
        }

        /// <summary>
        /// Creates a new <see cref="RequestedProcedure"/> of this type, does not schedule.
        /// </summary>
        /// <returns></returns>
        public virtual RequestedProcedure CreateProcedure()
        {
            return CreateProcedure(null);
        }

		/// <summary>
		/// This method is called from the constructor.  Use this method to implement any custom
		/// object initialization.
		/// </summary>
		private void CustomInitialize()
		{
		}

        public virtual void AddModalityProcedureStepType(ModalityProcedureStepType spt)
        {
            if (this.ModalityProcedureStepTypes.Contains(spt))
            {
                throw new HealthcareWorkflowException(
                    string.Format("Requested Procedure Type {0} already contains Scheduled Procedure Step Type {1}",
                    this.Id, spt.Id));
            }

            this.ModalityProcedureStepTypes.Add(spt);
        }
	
		#region Object overrides
		
		public override bool Equals(object that)
		{
            RequestedProcedureType other = that as RequestedProcedureType;
            return other != null && other.Id == this.Id;
		}
		
		public override int GetHashCode()
		{
			return this.Id.GetHashCode();
		}
		
		#endregion

	}
}