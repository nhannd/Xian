using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;

namespace ClearCanvas.Ris.Application.Services.Admin.StaffGroupAdmin
{
    [ExtensionOf(typeof(DataImporterExtensionPoint), Name = "Staff Group Importer")]
    [ExtensionOf(typeof(ApplicationRootExtensionPoint))]
    class StaffGroupImporter : DataImporterBase
    {
        private IUpdateContext _context;
        private const string tagValue = "value";
        private const string tagStaffGroups = "staff-groups";
        private const string tagStaffGroup = "staff-group";
        private const string tagStaffMembers = "staff-members";

        private const string attrName = "name";
        private const string attrDescription = "description";
        private const string attrId = "id";

        public override bool SupportsXml
        {
            get { return true; }
        }

        public override void ImportXml(XmlReader reader, IUpdateContext context)
        {
            _context = context;
            while (reader.Read())
            {
                if (reader.IsStartElement(tagStaffGroups))
                {
                    for (bool elementExists = reader.ReadToDescendant(tagStaffGroup);
                        elementExists;
                        elementExists = reader.ReadToNextSibling(tagStaffGroup))
                    {
                        ReadStaffGroup(reader.ReadSubtree());
                    }
                }
            }
        }

        private void ReadStaffGroup(XmlReader reader)
        {
            reader.Read();

            string name = reader.GetAttribute(attrName);
            StaffGroup group = LoadOrCreateGroup(name);
            group.Description = reader.GetAttribute(attrDescription);

            ReadMembers(group.Members, reader);

            while (reader.Read()) ;
        }

        private void ReadMembers(ICollection<Staff> members, XmlReader reader)
        {
            if (reader.ReadToFollowing(tagStaffMembers))
            {
                for (bool elementExists = reader.ReadToDescendant(tagValue);
                    elementExists;
                    elementExists = reader.ReadToNextSibling(tagValue))
                {
                    string staffId = reader.GetAttribute(attrId);
                    Staff value = LoadStaff(staffId);
                    if (value != null)
                    {
                        members.Add(value);
                    }
                }
            }
        }

        private StaffGroup LoadOrCreateGroup(string name)
        {
            StaffGroup group;
            try
            {
                StaffGroupSearchCriteria where = new StaffGroupSearchCriteria();
                where.Name.EqualTo(name);
                group = _context.GetBroker<IStaffGroupBroker>().FindOne(where);
            }
            catch (EntityNotFoundException)
            {
                group = new StaffGroup();
                group.Name = name;
                _context.Lock(group, DirtyState.New);
            }

            return group;
        }

        private Staff LoadStaff(string id)
        {
            StaffSearchCriteria where = new StaffSearchCriteria();
            where.Id.EqualTo(id);
            return CollectionUtils.FirstElement(_context.GetBroker<IStaffBroker>().Find(where));
        }
    }
}
