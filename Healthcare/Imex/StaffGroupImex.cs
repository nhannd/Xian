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
    [ExtensionOf(typeof(XmlDataImexExtensionPoint))]
    [ImexDataClass("StaffGroup")]
    public class StaffGroupImex : XmlEntityImex<StaffGroup, StaffGroupImex.StaffGroupData>
    {
        [DataContract]
        public class StaffGroupData
        {
            [DataMember]
            public string Name;

            [DataMember]
            public string Description;

            [DataMember]
            public List<StaffMemberData> Members;

        }

        [DataContract]
        public class StaffMemberData
        {
            [DataMember]
            public string Id;
        }

        #region Overrides

        protected override IEnumerable<StaffGroup> GetItemsForExport(IReadContext context)
        {
            return context.GetBroker<IStaffGroupBroker>().FindAll();
        }

        protected override StaffGroupData Export(StaffGroup entity, IReadContext context)
        {
            StaffGroupData data = new StaffGroupData();
            data.Name = entity.Name;
            data.Description = entity.Description;

            data.Members = CollectionUtils.Map<Staff, StaffMemberData>(
                entity.Members,
                delegate(Staff staff)
                {
                    StaffMemberData s = new StaffMemberData();
                    s.Id = staff.Id;
                    return s;
                });

            return data;
        }

        protected override void Import(StaffGroupData data, IUpdateContext context)
        {
            StaffGroup group = LoadOrCreateGroup(data.Name,context);
            group.Description = data.Description;

            if (data.Members != null)
            {
                foreach (StaffMemberData s in data.Members)
                {
                    StaffSearchCriteria where = new StaffSearchCriteria();
                    where.Id.EqualTo(s.Id);
                    Staff staff = CollectionUtils.FirstElement(context.GetBroker<IStaffBroker>().Find(where));
                    if (staff != null)
                        group.Members.Add(staff);
                }
            }
        }

        #endregion


        private StaffGroup LoadOrCreateGroup(string name, IPersistenceContext context)
        {
            StaffGroup group;
            try
            {
                StaffGroupSearchCriteria where = new StaffGroupSearchCriteria();
                where.Name.EqualTo(name);
                group = context.GetBroker<IStaffGroupBroker>().FindOne(where);
            }
            catch (EntityNotFoundException)
            {
                group = new StaffGroup();
                group.Name = name;
                context.Lock(group, DirtyState.New);
            }

            return group;
        }
    }
}
