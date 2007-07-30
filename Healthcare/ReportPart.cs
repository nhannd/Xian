using System;
using System.Collections;
using System.Text;


namespace ClearCanvas.Healthcare {


    /// <summary>
    /// ReportPart component
    /// </summary>
	public partial class ReportPart
	{
	
		/// <summary>
		/// This method is called from the constructor.  Use this method to implement any custom
		/// object initialization.
		/// </summary>
		private void CustomInitialize()
		{
		}

        public bool IsAddendum
        {
            get { return int.Parse(this.Index) > 0; }
        }

        public virtual void Finalized()
        {
            if (this.Status == ReportPartStatus.P)
            {
                this.Status = ReportPartStatus.F;

                if (this.IsAddendum)
                    this.Report.Corrected();
                else
                    this.Report.Finalized();
            }
            else
                throw new HealthcareWorkflowException("Only report part in the preliminary status can be finalized");

        }

        public virtual void Cancelled()
        {
            this.Status = ReportPartStatus.X;

            if (this.IsAddendum == false)
                this.Report.Cancelled();
        }
    }
}