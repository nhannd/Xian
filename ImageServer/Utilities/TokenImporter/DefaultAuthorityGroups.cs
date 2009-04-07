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