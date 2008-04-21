using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core.Imex;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Healthcare.Imex
{
    [ExtensionOf(typeof(ApplicationRootExtensionPoint))]
    public class StaffImex : XmlEntityImex<Staff, StaffImex.StaffData>
    {
        [DataContract]
        public class StaffData
        {
            [DataMember]
            public string Id;

            [DataMember]
            public string FamilyName;

            [DataMember]
            public string GivenName;

            [DataMember]
            public string MiddleName;

            [DataMember]
            public string StaffType;

            [DataMember]
            public string UserName;

            [DataMember]
            public string LicenseNumber;

            [DataMember]
            public string BillingNumber;

            [DataMember]
            public Dictionary<string, string> ExtendedProperties;
        }


        #region Overrides

        protected override IEnumerable<Staff> GetItemsForExport(IReadContext context)
        {
            return context.GetBroker<IStaffBroker>().FindAll();
        }

        protected override StaffData Export(Staff entity, IReadContext context)
        {
            StaffData data = new StaffData();
            data.Id = entity.Id;
            data.StaffType = entity.Type.ToString();
            data.FamilyName = entity.Name.FamilyName;
            data.GivenName = entity.Name.GivenName;
            data.MiddleName = entity.Name.MiddleName;
            data.UserName = entity.UserName;
            data.LicenseNumber = entity.LicenseNumber;
            data.BillingNumber = entity.BillingNumber;
            data.ExtendedProperties = new Dictionary<string, string>(entity.ExtendedProperties);

            return data;
        }

        protected override void Import(StaffData data, IUpdateContext context)
        {
            Staff staff = GetStaff(data.Id, context);
            staff.Type = (StaffType)Enum.Parse(typeof(StaffType), data.StaffType);
            staff.Name.FamilyName = data.FamilyName;
            staff.Name.GivenName = data.GivenName;
            staff.Name.MiddleName = data.MiddleName;
            staff.UserName = data.UserName;
            staff.LicenseNumber = data.LicenseNumber;
            staff.BillingNumber = data.BillingNumber;

            if (data.ExtendedProperties != null)
            {
                foreach (KeyValuePair<string, string> kvp in data.ExtendedProperties)
                {
                    staff.ExtendedProperties[kvp.Key] = kvp.Value;
                }
            }
        }

        #endregion


        private Staff GetStaff(string id, IPersistenceContext context)
        {
            Staff staff = null;

            try
            {
                StaffSearchCriteria criteria = new StaffSearchCriteria();
                criteria.Id.EqualTo(id);

                IStaffBroker broker = context.GetBroker<IStaffBroker>();
                staff = broker.FindOne(criteria);
            }
            catch (EntityNotFoundException)
            {
                staff = new Staff();

                // need to populate required fields before we can lock (use dummy values)
                staff.Id = id;
                staff.Type = StaffType.X;
                staff.Name.FamilyName = "";
                staff.Name.GivenName = "";

                context.Lock(staff, DirtyState.New);
            }
            return staff;
        }
    }
}
