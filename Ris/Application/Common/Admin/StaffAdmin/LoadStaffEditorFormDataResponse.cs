using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.StaffAdmin
{
    [DataContract]
    public class LoadStaffEditorFormDataResponse : DataContractBase
    {
        public LoadStaffEditorFormDataResponse(
            List<EnumValueInfo> addressTypeChoices,
            List<string> addressProvinceChoices,
            List<string> addressCountryChoices,
            List<EnumValueInfo> phoneTypeChoices,
            List<EnumValueInfo> staffTypeChoices)
        {
            this.AddressTypeChoices = addressTypeChoices;
            this.AddressProvinceChoices = addressProvinceChoices;
            this.AddressCountryChoices = addressCountryChoices;
            this.PhoneTypeChoices = phoneTypeChoices;
            this.StaffTypeChoices = staffTypeChoices;
        }

        [DataMember]
        public List<EnumValueInfo> StaffTypeChoices;

        [DataMember]
        public List<EnumValueInfo> AddressTypeChoices;

        [DataMember]
        public List<string> AddressProvinceChoices;

        [DataMember]
        public List<string> AddressCountryChoices;

        [DataMember]
        public List<EnumValueInfo> PhoneTypeChoices;
    }
}
