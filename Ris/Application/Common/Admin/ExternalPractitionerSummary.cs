using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin
{
    [DataContract]
    public class ExternalPractitionerSummary : DataContractBase, ICloneable
    {
        public ExternalPractitionerSummary(EntityRef pracRef, PersonNameDetail personNameDetail, CompositeIdentifierDetail licenseNumber)
        {
            this.PractitionerRef = pracRef;
            this.Name = personNameDetail;
            this.LicenseNumber = licenseNumber;
        }

        public ExternalPractitionerSummary()
        {
        }

        [DataMember]
        public EntityRef PractitionerRef;

        [DataMember]
        public PersonNameDetail Name;

        [DataMember]
        public CompositeIdentifierDetail LicenseNumber;

        #region ICloneable Members

        public object Clone()
        {
            ExternalPractitionerSummary clone = new ExternalPractitionerSummary();
            clone.PractitionerRef = this.PractitionerRef;
            clone.Name = (PersonNameDetail)this.Name.Clone();
            if (this.LicenseNumber != null)
            {
                clone.LicenseNumber = (CompositeIdentifierDetail)this.LicenseNumber.Clone();
            }
            return clone;
        }

        #endregion
    }
}
