#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

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
    [ExtensionOf(typeof(CsvDataImporterExtensionPoint), Name = "User Importer")]
    [ExtensionOf(typeof(ApplicationRootExtensionPoint))]
    class UserImporter : CsvDataImporterBase
    {
        private const int _numFields = 9;

        private IPersistenceContext _context;
		private readonly AuthenticationSettings _settings = new AuthenticationSettings();

        #region CsvDataImporterBase overrides

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
        public override void Import(List<string> rows, IUpdateContext context)
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
                	UserInfo userInfo =
                		new UserInfo(userName, string.Format("{0} {1}", staffFamilyName, staffGivenName), null, null, null);
					user = User.CreateNewUser(userInfo, _settings.DefaultTemporaryPassword);
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
