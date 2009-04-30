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
using System.Reflection;
using ClearCanvas.Common;
using ClearCanvas.Common.Authorization;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageServer.Enterprise.Authentication;

namespace ClearCanvas.ImageServer.Utilities
{

    class UserGroupDefinitionAttribute : Attribute
    {
        public string Name;
    }

    enum DefaultUserGroup
    {
        [UserGroupDefinitionAttribute(Name = "PACS Administrators")]
        PACSAdministrators,

        [UserGroupDefinitionAttribute(Name = "PACS Technologists")]
        PACSTechnologists,

        [UserGroupDefinitionAttribute(Name = "PACS Read-only Users")]
        PACSUsers
    }

    [ExtensionOf(typeof(DefineAuthorityGroupsExtensionPoint))]
    public class DefaultAuthorityGroups : IDefineAuthorityGroups
    {
        static string GetGroupName(DefaultUserGroup group)
        {
            string fieldName = Enum.GetName(typeof(DefaultUserGroup), group);
            FieldInfo field = typeof(DefaultUserGroup).GetField(fieldName);
            UserGroupDefinitionAttribute definition = AttributeUtils.GetAttribute<UserGroupDefinitionAttribute>(field);
            return definition.Name;
        }

        #region IDefineAuthorityGroups Members

        public AuthorityGroupDefinition[] GetAuthorityGroups()
        {
            //TODO: Load from XML instead

            AuthorityGroupDefinition admins = new AuthorityGroupDefinition(
                GetGroupName(DefaultUserGroup.PACSAdministrators),
                new string[]
                    {
                        #region Tokens
                        ClearCanvas.Enterprise.Common.AuthorityTokens.Admin.Security.User,
                        ClearCanvas.Enterprise.Common.AuthorityTokens.Admin.Security.AuthorityGroup,
                        AuthorityTokens.Admin.Alert.Delete,
                        AuthorityTokens.Admin.Alert.View,
                        AuthorityTokens.Admin.ApplicationLog.Search,
                        AuthorityTokens.Admin.Configuration.Devices,
                        AuthorityTokens.Admin.Configuration.FileSystems,
                        AuthorityTokens.Admin.Configuration.ServerPartitions,
                        AuthorityTokens.Admin.Configuration.ServerRules,
                        AuthorityTokens.Admin.Configuration.ServiceScheduling,
                        AuthorityTokens.Admin.Configuration.PartitionArchive,

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

                        AuthorityTokens.StudyIntegrityQueue.Search,
                        AuthorityTokens.StudyIntegrityQueue.Reconcile,

                        AuthorityTokens.WorkQueue.Delete,
                        AuthorityTokens.WorkQueue.Reprocess,
                        AuthorityTokens.WorkQueue.Reschedule,
                        AuthorityTokens.WorkQueue.Reset,
                        AuthorityTokens.WorkQueue.Search,
                        AuthorityTokens.WorkQueue.View,
                        #endregion
                    });

            AuthorityGroupDefinition technologists = new AuthorityGroupDefinition(
                GetGroupName(DefaultUserGroup.PACSTechnologists),
                new string[]
                    {
                        #region Tokens
                        //AuthorityTokens.Admin.Alert.Delete,
                        //AuthorityTokens.Admin.Alert.View,
                        //AuthorityTokens.Admin.ApplicationLog.Search,
                        //AuthorityTokens.Admin.Configuration.Devices,
                        //AuthorityTokens.Admin.Configuration.FileSystems,
                        //AuthorityTokens.Admin.Configuration.ServerPartitions,
                        //AuthorityTokens.Admin.Configuration.ServerRules,
                        //AuthorityTokens.Admin.Configuration.ServiceScheduling,

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

                        AuthorityTokens.StudyIntegrityQueue.Search,
                        AuthorityTokens.StudyIntegrityQueue.Reconcile,

                        AuthorityTokens.WorkQueue.Delete,
                        AuthorityTokens.WorkQueue.Reprocess,
                        AuthorityTokens.WorkQueue.Reschedule,
                        AuthorityTokens.WorkQueue.Reset,
                        AuthorityTokens.WorkQueue.Search,
                        AuthorityTokens.WorkQueue.View,
                        #endregion
                    });

            AuthorityGroupDefinition users = new AuthorityGroupDefinition(
                GetGroupName(DefaultUserGroup.PACSUsers),
                new string[]
                    {
                        #region Tokens
                        //AuthorityTokens.Admin.Alert.Delete,
                        //AuthorityTokens.Admin.Alert.View,
                        //AuthorityTokens.Admin.ApplicationLog.Search,
                        //AuthorityTokens.Admin.Configuration.Devices,
                        //AuthorityTokens.Admin.Configuration.FileSystems,
                        //AuthorityTokens.Admin.Configuration.ServerPartitions,
                        //AuthorityTokens.Admin.Configuration.ServerRules,
                        //AuthorityTokens.Admin.Configuration.ServiceScheduling,

                        //AuthorityTokens.ArchiveQueue.Delete,
                        AuthorityTokens.ArchiveQueue.Search,

                        //AuthorityTokens.RestoreQueue.Delete,
                        AuthorityTokens.RestoreQueue.Search,

                        //AuthorityTokens.Study.Delete,
                        //AuthorityTokens.Study.Edit,
                        //AuthorityTokens.Study.Move,
                        //AuthorityTokens.Study.Restore,
                        AuthorityTokens.Study.Search,
                        AuthorityTokens.Study.View,

                        AuthorityTokens.StudyIntegrityQueue.Search,
                        //AuthorityTokens.StudyIntegrityQueue.Reconcile,

                        //AuthorityTokens.WorkQueue.Delete,
                        //AuthorityTokens.WorkQueue.Reprocess,
                        //AuthorityTokens.WorkQueue.Reschedule,
                        //AuthorityTokens.WorkQueue.Reset,
                        AuthorityTokens.WorkQueue.Search,
                        AuthorityTokens.WorkQueue.View,
                        #endregion
                    });


            return new AuthorityGroupDefinition[] { admins, technologists, users };
        }

        #endregion
    }
}