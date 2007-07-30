using System;
using System.Collections;
using System.Text;

using ClearCanvas.Enterprise.Core;

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
        /// Adds a report part to this report, setting the report's <see cref="Report.ReportParts"/> property
        /// to refer to this object.  Use this method rather than referring to the <see cref="Report.ReportParts"/>
        /// collection directly.
        /// </summary>
        /// <param name="profile"></param>
        public void AddPart(ReportPart part)
        {
            if (part.Report != null)
            {
                //NB: technically we should remove the report part from the other report's collection, but there
                //seems to be a bug with NHibernate where it deletes the part if we do this
                //part.Report.Parts.Remove(part);
            }
            part.Report = this;
            this.Parts.Add(part);
        }

        public ReportPart AddPart(string reportPartContent)
        {
            ReportPart part = new ReportPart(this.Parts.Count.ToString(), reportPartContent, ReportPartStatus.P, this);
            this.AddPart(part);
            return part;
        }

        public virtual void Finalized()
        {
            if (this.Status == ReportStatus.P)
                this.Status = ReportStatus.F;
            else
                throw new HealthcareWorkflowException("Only reports in the preliminary status can be finalized");
        
        }

        public virtual void Corrected()
        {
            this.Status = ReportStatus.C;
        }

        public virtual void Cancelled()
        {
            this.Status = ReportStatus.X;
        }
    }
}
