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

using ClearCanvas.Enterprise.Core;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Healthcare {


    /// <summary>
    /// Report entity
    /// </summary>
	public partial class Report : ClearCanvas.Enterprise.Core.Entity
	{
	
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
        /// Adds a <see cref="RequestedProcedure"/> to this report, meaning that the report covers
        /// this radiology procedure.
        /// </summary>
        /// <param name="rp"></param>
        public virtual void AddProcedure(RequestedProcedure rp)
        {
            _procedures.Add(rp);
        }

        /// <summary>
        /// Adds a report part to this report, setting the report's <see cref="Report.Parts"/> property
        /// to refer to this object.  Use this method rather than referring to the <see cref="Report.Parts"/>
        /// collection directly.
        /// </summary>
        /// <param name="part"></param>
        public virtual void AddPart(ReportPart part)
        {
            if (part.Report != null)
            {
                //NB: technically we should remove the report part from the other report's collection, but there
                //seems to be a bug with NHibernate where it deletes the part if we do this
                //part.Report.Parts.Remove(part);
            }
            part.Report = this;
            this.Parts.Add(part);

            UpdateStatus();
        }

        public bool HasAddendum
        {
            get { return _parts != null && _parts.Count > 0; }
        }

        public virtual ReportPart AddPart(string reportPartContent)
        {
            ReportPart part = new ReportPart(_parts.Count, reportPartContent, ReportPartStatus.P, null, null, null, null, this);
            this.AddPart(part);
            return part;
        }

        /// <summary>
        /// Called by this report or by a child report part to tell this report to update its status.
        /// </summary>
        protected internal virtual void UpdateStatus()
        {
            // if all report parts are cancelled, the report is cancelled
            if (CollectionUtils.TrueForAll<ReportPart>(_parts,
                delegate(ReportPart part) { return part.Status == ReportPartStatus.X; }))
            {
                _status = ReportStatus.X;
            }
            else
            // if the report contains any report parts that is still preliminary, the report is prelimiinary
            if (CollectionUtils.Contains<ReportPart>(_parts,
               delegate(ReportPart part) { return part.Status == ReportPartStatus.P; }))
            {
                _status = ReportStatus.P;
            }
            else
            {
                if (HasAddendum)
                    _status = ReportStatus.C;
                else
                    _status = ReportStatus.F;
            }
        }
    }
}
