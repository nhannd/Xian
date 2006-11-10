using System;
using System.Collections;
using System.Text;

using Iesi.Collections;
using ClearCanvas.Enterprise;


namespace ClearCanvas.Healthcare {


    /// <summary>
    /// VisitLocation entity
    /// </summary>
	public partial class VisitLocation
	{
        private void CustomInitialize()
        {
        }

        public void CopyFrom(VisitLocation vl)
        {
            this.Role = vl.Role;
            this.Location = vl.Location;
            this.StartTime = vl.StartTime;
            this.EndTime = vl.EndTime;
        }
	}
}
