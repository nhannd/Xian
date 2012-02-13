#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Reflection;
using ClearCanvas.Common;
using ClearCanvas.Common.Authorization;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageServer.Enterprise.Authentication
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
                        AuthorityTokens.Study.Reprocess,

                        AuthorityTokens.StudyIntegrityQueue.Search,
                        AuthorityTokens.StudyIntegrityQueue.Reconcile,

                        AuthorityTokens.WorkQueue.Delete,
                        AuthorityTokens.WorkQueue.Reprocess,
                        AuthorityTokens.WorkQueue.Reschedule,
                        AuthorityTokens.WorkQueue.Reset,
                        AuthorityTokens.WorkQueue.Search,
                        AuthorityTokens.WorkQueue.View,

                        AuthorityTokens.DataAccess.AllStudies,
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
                        AuthorityTokens.Study.Reprocess,

                        AuthorityTokens.StudyIntegrityQueue.Search,
                        AuthorityTokens.StudyIntegrityQueue.Reconcile,

                        AuthorityTokens.WorkQueue.Delete,
                        AuthorityTokens.WorkQueue.Reprocess,
                        AuthorityTokens.WorkQueue.Reschedule,
                        AuthorityTokens.WorkQueue.Reset,
                        AuthorityTokens.WorkQueue.Search,
                        AuthorityTokens.WorkQueue.View,

                        AuthorityTokens.DataAccess.AllStudies,
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

                        AuthorityTokens.DataAccess.AllStudies,
                        #endregion
                    });


            return new AuthorityGroupDefinition[] { admins, technologists, users };
        }

        #endregion
    }
}