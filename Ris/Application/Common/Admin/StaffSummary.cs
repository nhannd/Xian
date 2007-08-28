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
        public StaffSummary(EntityRef staffRef, string staffId, EnumValueInfo staffType, PersonNameDetail personNameDetail)
        {
            this.StaffRef = staffRef;
            this.StaffId = staffId;
            this.StaffType = staffType;
            this.Name = personNameDetail;
        }

        public StaffSummary()
        {
        }

        [DataMember]
        public EntityRef StaffRef;

        [DataMember]
        public EnumValueInfo StaffType;

        [DataMember]
        public string StaffId;

        [DataMember]
        public PersonNameDetail Name;

        #region ICloneable Members

        public object Clone()
        {
            StaffSummary clone = new StaffSummary();
            clone.StaffRef = this.StaffRef;
            clone.StaffId = this.StaffId;
            clone.Name = (PersonNameDetail)this.Name.Clone();
            clone.StaffType = this.StaffType;
            return clone;
        }

        #endregion
    }
}
