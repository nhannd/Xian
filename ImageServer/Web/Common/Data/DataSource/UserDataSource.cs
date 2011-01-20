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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common.Admin.AuthorityGroupAdmin;
using ClearCanvas.Enterprise.Common.Admin.UserAdmin;
using ClearCanvas.ImageServer.Enterprise.Admin;

namespace ClearCanvas.ImageServer.Web.Common.Data.DataSource
{
    public class UserDataSource : BaseDataSource
    {
        #region Private Members

    	private int _resultCount;
        private string _displayName;
        private string _userName;
        #endregion Private Members

        #region Public Members
        public delegate void UserFoundSetDelegate(IList<UserRowData> list);
        public UserFoundSetDelegate UserFoundSet;
        #endregion Public Members

        #region Properties
        public int ResultCount
        {
            get { return _resultCount; }
            set { _resultCount = value; }
        }

        public string DisplayName
        {
            get { return _displayName; }
            set { _displayName = value; }
        }

        public string UserName
        {
            get { return _userName; }
            set { _userName = value; }
        }
        #endregion

        #region Private Methods

        private IList<UserRowData> InternalSelect(int startRowIndex, int maximumRows, out int resultCount)
        {
            if (maximumRows == 0)
            {
                resultCount = 0;
                return new List<UserRowData>();
            }

            List<UserRowData> users;
            using (UserManagement service = new UserManagement())
            {
                ListUsersRequest filter = new ListUsersRequest();
                filter.UserName = "%" + UserName.Replace("*","%").Replace("?","_");
				filter.DisplayName = "%" + DisplayName.Replace("*", "%").Replace("?", "_");
                filter.Page.FirstRow = startRowIndex;

                users = CollectionUtils.Map<UserSummary, UserRowData>(
                    service.FindUsers(filter),
                    delegate(UserSummary summary)
                        {
                            UserRowData user = new UserRowData(summary, service.GetUserDetail(summary.UserName));
                            return user;
                        });
            }
            resultCount = users.Count;

            return users;
        }

        #endregion Private Methods

        #region Public Methods
        
        public IEnumerable<UserRowData> Select(int startRowIndex, int maximumRows)
        {
            IList<UserRowData> _list = InternalSelect(startRowIndex, maximumRows, out _resultCount);

            if (UserFoundSet != null)
                UserFoundSet(_list);

            return _list;

        }

        public int SelectCount()
        {
            // Ignore the search results
            InternalSelect(0, 1, out _resultCount);

            return ResultCount;
        }

        #endregion Public Methods
    }

    [Serializable]
    public class UserRowData
    {
        private string _userName;
        private string _displayName;
        private bool _enabled;
        private DateTime? _lastLoginTime;
        private List<UserGroup> _userGroups = new List<UserGroup>();

        public List<UserGroup> UserGroups
        {
            get { return _userGroups; }
            set { _userGroups = value; }
        }

        public string UserName
        {
            get { return _userName; }
            set { _userName = value; }
        }

        public string DisplayName
        {
            get { return _displayName; }
            set { _displayName = value; }
        }

        public bool Enabled
        {
            get { return _enabled; }
            set { _enabled = value; }
        }

        public DateTime? LastLoginTime
        {
            get { return _lastLoginTime; }
            set { _lastLoginTime = value; }
        }

        public UserRowData(UserSummary summary, UserDetail details)
        {
            UserName = summary.UserName;
            DisplayName = summary.DisplayName;
            Enabled = summary.Enabled;
            LastLoginTime = summary.LastLoginTime;

            if (details!=null)
            {
                foreach (AuthorityGroupSummary authorityGroup in details.AuthorityGroups)
                {
                    UserGroups.Add(new UserGroup(
                            authorityGroup.AuthorityGroupRef.Serialize(), authorityGroup.Name));
                }
            }
            
        }

        public UserRowData()
        {
        }
    }

    [Serializable]
    public class UserGroup
    {
        private string _authorityGroupRef;
        private string _name;
        
        public UserGroup(string authorityGroupRef, string name)
        {
            _authorityGroupRef = authorityGroupRef;
            _name = name;
        }

        public string UserGroupRef
        {
            get { return _authorityGroupRef;  }
            set { _authorityGroupRef = value;  }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }       
    }
}