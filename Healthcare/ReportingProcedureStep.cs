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
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Workflow;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Healthcare
{
    public abstract class ReportingProcedureStep : ProcedureStep
    {
        private ReportPart _reportPart;

        public ReportingProcedureStep()
        {
        }

        public ReportingProcedureStep(Procedure procedure, ReportPart reportPart)
            :base(procedure)
        {
            _reportPart = reportPart;
        }

        public ReportingProcedureStep(ReportingProcedureStep previousStep)
            : this(previousStep.Procedure, previousStep.ReportPart)
        {
        }

        public override bool IsPreStep
        {
            get { return false; }
        }

		public override List<Procedure> GetLinkedProcedures()
		{
			if(_reportPart != null && _reportPart.Report != null)
			{
				return CollectionUtils.Select(_reportPart.Report.Procedures,
					delegate(Procedure p) { return !Equals(p, this.Procedure); });
			}
			else
			{
				return new List<Procedure>();
			}
		}

		/// <summary>
		/// Gets the <see cref="ReportPart"/> that this step targets, or null if there is no associated report part.
		/// </summary>
        public virtual ReportPart ReportPart
        {
            get { return _reportPart; }
            set { _reportPart = value; }
        }

		/// <summary>
		/// Gets the <see cref="Report"/> that this step is associated with, or null if not associated.
		/// </summary>
    	public virtual Report Report
    	{
			get { return _reportPart == null ? null : _reportPart.Report; }
    	}

		protected override bool IsRelatedStep(ProcedureStep step)
		{
			// can't have relatives if no report
			if(this.Report == null)
				return false;

			// relatives must be reporting steps
			if (!step.Is<ReportingProcedureStep>())
				return false;

			// check if tied to same report
			ReportingProcedureStep that = step.As<ReportingProcedureStep>();
			return that.Report != null && Equals(this.Report, that.Report);
		}
	}
}
