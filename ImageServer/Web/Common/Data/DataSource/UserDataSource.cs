#region License

// Copyright (c) 2006-2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

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

            List<UserRowData> users = null;
            using (UserManagement service = new UserManagement())
            {
                ListUsersRequest filter = new ListUsersRequest();
                filter.UserName = UserName;
                filter.DisplayName = DisplayName;
                filter.Page.FirstRow = startRowIndex;
                filter.Page.MaxRows = maximumRows;

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
            if (ResultCount != 0) return ResultCount;

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