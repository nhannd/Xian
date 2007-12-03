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
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Enterprise.Core;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Workflow;
using Iesi.Collections.Generic;

namespace ClearCanvas.Healthcare {


    /// <summary>
    /// Report entity
    /// </summary>
	public partial class Report : ClearCanvas.Enterprise.Core.Entity
	{
        /// <summary>
        /// Constructor for creating a new radiology report for the specified procedure.
        /// </summary>
        /// <param name="procedure">The procedure being reported.</param>
        public Report(RequestedProcedure procedure)
        {
            _procedures = new HashedSet<ClearCanvas.Healthcare.RequestedProcedure>();
            _parts = new List<ReportPart>();

            _procedures.Add(procedure);
            procedure.Reports.Add(this);

            // create the main report part
            ReportPart mainReport = new ReportPart();
            mainReport.Index = 0;
            mainReport.Report = this;
            _parts.Add(mainReport);
        }
	
		/// <summary>
		/// This method is called from the constructor.  Use this method to implement any custom
		/// object initialization.
		/// </summary>
		private void CustomInitialize()
		{
		}
		
		#region Object overrides
		
		public override bool Equals(object that)
		{
			// TODO: implement a test for business-key equality
			return base.Equals(that);
		}
		
		public override int GetHashCode()
		{
			// TODO: implement a hash-code based on the business-key used in the Equals() method
			return base.GetHashCode();
		}
		
		#endregion

        /// <summary>
        /// Gets a value indicating whether this report has any addenda.
        /// </summary>
        public virtual bool HasAddenda
        {
            get { return _parts.Count > 1; }
        }

        /// <summary>
        /// Gets the active (modifiable) report part, or null if no report part is active.
        /// </summary>
        public virtual ReportPart ActivePart
        {
            get
            {
                ReportPart lastPart = CollectionUtils.LastElement(_parts);
                return lastPart.IsModifiable ? lastPart : null;
            }
        }

        /// <summary>
        /// Adds a new part to this report.
        /// </summary>
        /// <returns></returns>
        public virtual ReportPart AddAddendum()
        {
            if(this.ActivePart != null)
                throw new WorkflowException("Cannot add an addendum because the report, or a previous addendum, has not been completed.");

            ReportPart part = new ReportPart(_parts.Count, null, ReportPartStatus.P, null, null, null, null, this);
            _parts.Add(part);

            UpdateStatus();
            
            return part;
        }

        /// <summary>
        /// Links a <see cref="RequestedProcedure"/> to this report, meaning that the report covers
        /// this radiology procedure.
        /// </summary>
        /// <param name="procedure"></param>
        protected internal virtual void LinkProcedure(RequestedProcedure procedure)
        {
            if (_procedures.Contains(procedure))
                throw new WorkflowException("The procedure is already associated with this report.");

            // does the procedure already have a report?
            Report otherReport = procedure.ActiveReport;
            if (otherReport != null && !this.Equals(otherReport))
                throw new WorkflowException("Cannot link this procedure because it already has an active report.");

            _procedures.Add(procedure);
            procedure.Reports.Add(this);
        }

        /// <summary>
        /// Called by this report or by a child report part to tell this report to update its status.
        /// </summary>
        protected internal virtual void UpdateStatus()
        {
            // if the main report part is cancelled, the report is cancelled
            // (cancelling an addendum alone does not cancel the report)
            if (_parts.Count > 0 && _parts[0].Status == ReportPartStatus.X)
            {
                _status = ReportStatus.X;
            }
            else
            if (_status == ReportStatus.D)
            {
                bool prelimParts =
                    CollectionUtils.Contains(_parts,
                        delegate(ReportPart part) { return part.Status == ReportPartStatus.P; });

                if (prelimParts)
                    _status = ReportStatus.P;
            }
            else
            if (_status == ReportStatus.P || _status == ReportStatus.F)
            {
                int numCompletedParts =
                    CollectionUtils.Select(_parts,
                        delegate(ReportPart part) { return part.Status == ReportPartStatus.F; }).Count;

                // transition from F to C if addendum completed
                if (_status == ReportStatus.F && numCompletedParts > 1)
                {
                    _status = ReportStatus.C;
                }
                // transition from P to F if initial report part completed
                else if (_status == ReportStatus.P && numCompletedParts > 0)
                {
                    _status = ReportStatus.F;
                }
            }
        }
    }
}
