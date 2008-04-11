using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Authentication.Brokers;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Core.Imex;

namespace ClearCanvas.Enterprise.Authentication.Imex
{
    [ExtensionOf(typeof(DataImporterExtensionPoint), Name = "User Importer")]
    [ExtensionOf(typeof(ApplicationRootExtensionPoint))]
    class UserImporter : DataImporterBase
    {
        private const int _numFields = 9;

        private IPersistenceContext _context;

        #region DataImporterBase overrides

        public override bool SupportsCsv
        {
            get { return true; }
        }

        /// <summary>
        /// Import user from CSV format.
        /// </summary>
        /// <param name="rows">
        /// Each string in the list must contain 25 CSV fields, as follows:
        ///     0 - UserName
        ///     1 - StaffType
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

            foreach (string row in rows)
            {
                string[] fields = ParseCsv(row, _numFields);

                string userName = fields[0];

                string staffId = fields[2];
                string staffFamilyName = fields[3];
                string staffGivenName = fields[4];

                User user = GetUser(userName, importedUsers);

                if (user == null)
                {
                    user = User.CreateNewUser(new UserInfo(userName, string.Format("{0} {1}", staffFamilyName, staffGivenName), null, null));
                    _context.Lock(user, DirtyState.New);

                    importedUsers.Add(user);
                }
            }
        }

        #endregion

        #region Private Methods

        private User GetUser(string userName, IList<User> importedUsers)
        {
            User user = null;

            user = CollectionUtils.SelectFirst(importedUsers,
                delegate(User u) { return u.UserName == userName; });

            if (user == null)
            {
                UserSearchCriteria criteria = new UserSearchCriteria();
                criteria.UserName.EqualTo(userName);

                IUserBroker broker = _context.GetBroker<IUserBroker>();
                user = CollectionUtils.FirstElement(broker.Find(criteria));
            }

            return user;
        }

        #endregion

    }
}
