using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin
{
    [DataContract]
    public class StaffSummary : DataContractBase, ICloneable
    {
        public StaffSummary(EntityRef staffRef, PersonNameDetail personNameDetail, string licenseNumber)
        {
            this.StaffRef = staffRef;
            this.PersonNameDetail = personNameDetail;
            this.LicenseNumber = licenseNumber;
        }

        public StaffSummary()
        {
        }

        [DataMember]
        public EntityRef StaffRef;

        [DataMember]
        public EnumValueInfo StaffType;

        [DataMember]
        public PersonNameDetail PersonNameDetail;

        // Member for Practitioner
        [DataMember]
        public string LicenseNumber;

        #region ICloneable Members

        public object Clone()
        {
            StaffSummary clone = new StaffSummary();
            clone.StaffRef = this.StaffRef;
            clone.PersonNameDetail = (PersonNameDetail)this.PersonNameDetail.Clone();
            clone.LicenseNumber = this.LicenseNumber;
            clone.StaffType = this.StaffType;
            return clone;
        }

        #endregion
    }
}
