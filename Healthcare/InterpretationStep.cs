#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
using ClearCanvas.Enterprise.Core.Modelling;
using Iesi.Collections;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Workflow;


namespace ClearCanvas.Healthcare {


    /// <summary>
    /// InterpretationStep entity
    /// </summary>
	public partial class InterpretationStep : ReportingProcedureStep
	{
    	private ImageAvailability _imageAvailability;

        public InterpretationStep(Procedure procedure)
            :base(procedure, null)
        {
			_imageAvailability = Healthcare.ImageAvailability.U;
		}

        public InterpretationStep(ReportingProcedureStep previousStep)
            :base(previousStep)
        {
			CustomInitialize();

        	_imageAvailability = Healthcare.ImageAvailability.U;
		}


        /// <summary>
        /// Default no-args constructor required by NHibernate
        /// </summary>
		public InterpretationStep()
        {
        }

		/// <summary>
		/// This method is called from the constructor.  Use this method to implement any custom
		/// object initialization.
		/// </summary>
		private void CustomInitialize()
		{
		}

        /// <summary>
        /// Links the procedure associated with this step to the specified report, and discontinues this step.
        /// </summary>
        /// <param name="report"></param>
        public virtual void LinkToReport(Report report)
        {
            if (this.State != ActivityStatus.SC)
                throw new WorkflowException("Cannot link to existing report because this interpretation has already been started.");

            // link the associated procedure to the specified report
            report.LinkProcedure(this.Procedure);

            // discontinue step so we don't show up in any worklists
            this.Discontinue();
        }

		[PersistentProperty]
		public virtual ImageAvailability ImageAvailability
		{
			get { return _imageAvailability; }
			set { _imageAvailability = value; }
		}

        public override string Name
        {
            get { return "Interpretation"; }
        }

        protected override void OnStateChanged(ActivityStatus previousState, ActivityStatus newState)
        {
            if(newState == ActivityStatus.CM)
            {
                if (this.ReportPart == null)
                    throw new WorkflowException("This ReportingStep does not have an associated ReportPart.");

                this.ReportPart.Interpreter = this.PerformingStaff;
            }

            base.OnStateChanged(previousState, newState);
        }

		protected override ProcedureStep CreateScheduledCopy()
		{
			return new InterpretationStep(this);
		}
	}
}
