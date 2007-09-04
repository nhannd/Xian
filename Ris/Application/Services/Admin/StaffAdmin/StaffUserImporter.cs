using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Enterprise.Authentication;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Authentication.Brokers;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Application.Services.Admin.StaffAdmin
{
    [ExtensionOf(typeof(DataImporterExtensionPoint), Name = "Staff and User Importer")]
    [ExtensionOf(typeof(ApplicationRootExtensionPoint))]
    class StaffUserImporter : DataImporterBase
    {
        private const int _numFields = 9;

        IPersistenceContext _context;

        #region DataImporterBase overrides

        public override bool SupportsCsv
        {
            get { return true; }
        }

        /// <summary>
        /// Import staff and user from CSV format.
        /// </summary>
        /// <param name="lines">
        /// Each string in the list must contain 25 CSV fields, as follows:
        ///     0 - UserName
        ///     1 - Type
        ///     2 - Id
        ///     3 - FamilyName
        ///     4 - GivenName
        ///     5 - MiddleName
        ///     6 - Prefix
        ///     7 - Suffix
        ///     8 - Degree
        /// </param>
        /// <param name="context"></param>
        public override void ImportCsv(List<string> rows, IUpdateContext context)
        {
            _context = context;

            List<User> importedUsers = new List<User>();
            List<Staff> importedStaff = new List<Staff>();

            foreach (string row in rows)
            {
                string[] fields = ParseCsv(row, _numFields);

                string userName = fields[0];

                StaffType staffType = TryParseOrDefault(fields[1], StaffType.X);
                string staffId = fields[2];

                string staffFamilyName = fields[3];
                string staffGivenName = fields[4];
                string staffMiddlename = fields[5];
                string staffPrefix = fields[6];
                string staffSuffix = fields[7];
                string staffDegree = fields[8];

                User user = GetUser(userName, importedUsers);

                if (user == null)
                {
                    user = new User();
                    user.UserName = userName;

                    _context.Lock(user, DirtyState.New);

                    importedUsers.Add(user);
                }

                Staff staff = GetStaff(staffId, importedStaff);

                if (staff == null)
                {
                    staff = new Staff();

                    staff.Id = staffId;
                    staff.Type = staffType;
                    staff.Name = new PersonName(staffFamilyName, staffGivenName, staffMiddlename, staffPrefix, staffSuffix, staffDegree);

                    _context.Lock(staff, DirtyState.New);

                    importedStaff.Add(staff);
                }

                TrySetStaffUser(user, staff);
            }
        }

        #endregion

        #region Private Methods

        private User GetUser(string userName, IList<User> importedUsers)
        {
            User user = null;

            user = CollectionUtils.SelectFirst<User>(importedUsers,
                delegate(User u) { return u.UserName == userName; });

            if (user == null)
            {
                UserSearchCriteria criteria = new UserSearchCriteria();
                criteria.UserName.EqualTo(userName);

                IUserBroker broker = _context.GetBroker<IUserBroker>();
                user = CollectionUtils.FirstElement<User>(broker.Find(criteria));
            }

            return user;
        }

        private Staff GetStaff(string staffId, IList<Staff> importedStaff)
        {
            Staff staff = null;

            staff = CollectionUtils.SelectFirst<Staff>(importedStaff,
                delegate(Staff s) { return s.Id == staffId; });

            if (staff == null)
            {
                StaffSearchCriteria criteria = new StaffSearchCriteria();
                criteria.Id.EqualTo(staffId);

                IStaffBroker broker = _context.GetBroker<IStaffBroker>();
                staff = CollectionUtils.FirstElement<Staff>(broker.Find(criteria));
            }

            return staff;
        }

        private void TrySetStaffUser(User user, Staff staff)
        {
            if(staff.User != null && staff.User != user)
            {

            }
            else
            {
                staff.User = user;
            }
        }

        #endregion

    }
}
