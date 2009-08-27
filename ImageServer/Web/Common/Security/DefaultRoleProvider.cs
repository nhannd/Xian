#region License

// Copyright (c) 2009, ClearCanvas Inc.
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
using System.IO;
using System.Security;
using System.Text;
using System.Web.Hosting;
using System.Web.Security;
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.ImageServer.Enterprise.Authentication;

namespace ClearCanvas.ImageServer.Web.Common.Security
{
    class DefaultRoleProvider:RoleProvider
    {
        private string[] _allTokens;
        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        {
            base.Initialize(name, config);

            _allTokens = new string[]
                {
                    AuthorityTokens.Admin.Alert.Delete,
                    AuthorityTokens.Admin.Alert.View,
                    AuthorityTokens.Admin.ApplicationLog.Search,
                    AuthorityTokens.Admin.Configuration.Devices,
                    AuthorityTokens.Admin.Configuration.FileSystems,
                    AuthorityTokens.Admin.Configuration.PartitionArchive,
                    AuthorityTokens.Admin.Configuration.ServerPartitions,
                    AuthorityTokens.Admin.Configuration.ServerRules,
                    AuthorityTokens.Admin.Configuration.ServiceScheduling,
                    AuthorityTokens.Admin.StudyDeleteHistory.Delete,
                    AuthorityTokens.Admin.StudyDeleteHistory.Search,
                    AuthorityTokens.Admin.StudyDeleteHistory.View,
                    AuthorityTokens.Admin.Dashboard.View,
                    AuthorityTokens.ArchiveQueue.Delete,
                    AuthorityTokens.ArchiveQueue.Search,
                    AuthorityTokens.RestoreQueue.Delete,
                    AuthorityTokens.RestoreQueue.Search,
                    AuthorityTokens.Study.Delete,
                    AuthorityTokens.Study.Edit,
                    AuthorityTokens.Study.Move,
                    AuthorityTokens.Study.Restore,
                    AuthorityTokens.Study.Search,
                    AuthorityTokens.Study.View,
                    AuthorityTokens.Study.Reprocess,
                    AuthorityTokens.StudyIntegrityQueue.Reconcile,
                    AuthorityTokens.StudyIntegrityQueue.Search,
                    AuthorityTokens.WorkQueue.Delete,
                    AuthorityTokens.WorkQueue.Reprocess,
                    AuthorityTokens.WorkQueue.Reschedule,
                    AuthorityTokens.WorkQueue.Reset,
                    AuthorityTokens.WorkQueue.Search,
                    AuthorityTokens.WorkQueue.View
                };

        }
        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override string ApplicationName
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        public override void CreateRole(string roleName)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override string[] GetAllRoles()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override string[] GetRolesForUser(string username)
        {
            return _allTokens;
        }

        public override string[] GetUsersInRole(string roleName)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override bool RoleExists(string roleName)
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }
}