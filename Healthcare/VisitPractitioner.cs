using System;
using System.Collections;
using System.Text;

using Iesi.Collections;
using ClearCanvas.Enterprise;


namespace ClearCanvas.Healthcare {


    /// <summary>
    /// VisitPractitioner component
    /// </summary>
	public partial class VisitPractitioner
	{
        private void CustomInitialize()
        {
        }

        public void CopyFrom(VisitPractitioner vp)
        {
            this.Role = vp.Role;
            this.Practitioner = vp.Practitioner;
            this.StartTime = vp.StartTime;
            this.EndTime = vp.EndTime;
        }
	}
}
